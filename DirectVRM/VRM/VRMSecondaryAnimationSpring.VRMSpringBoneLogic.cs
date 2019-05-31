using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///     ひとつの揺れボーン（揺れ骨）を表すクラス。
    ///     このインスタンスを複数連ねることで、一連の揺れ物が形成される。(<see cref="m_verlet"/> 参照)
    /// </summary>
    /// <remarks>
    ///     original from
    ///     http://rocketjump.skr.jp/unity3d/109/
    /// </remarks>
    public class VRMSpringBoneLogic
    {
        /// <summary>
        ///     揺れボーンの頭となるノード。
        /// </summary>
        public glTFNode Head { get; protected set; }

        /// <summary>
        ///     揺れボーンの頭ノードのローカル回転の初期値。
        /// </summary>
        public Quaternion InitialLocalHeadRotationRH { get; protected set; }

        /// <summary>
        ///     揺れボーンの方向（頭から尻尾への方向）を示す単位ベクトル。
        /// </summary>
        public Vector3 BoneAxis { get; set; }

        /// <summary>
        ///     揺れボーンの長さ（頭から尻尾までの長さ）
        /// </summary>
        private float BoneLength;

        /// <summary>
        ///     揺れボーンの尻尾における当たり判定用の球の半径。
        /// </summary>
        public float Radius { get; set; } = 0.02f;



        // 生成と終了


        public VRMSpringBoneLogic( glTFNode center, glTFNode head, Vector3 localTailPositionRH )
        {
            this.Head = head;
            this.InitialLocalHeadRotationRH = head.LocalRotationRH;
            this.BoneAxis = Vector3.Normalize( localTailPositionRH );
            this.BoneLength = localTailPositionRH.Length();

            // 尻尾の初期位置
            var worldTailPositionRH = Vector3.TransformCoordinate( localTailPositionRH, this.Head.LocalToWorldMatrixRH );
            this._現在の尻尾の位置 = ( center != null ) ?
                Vector3.TransformCoordinate( worldTailPositionRH, center.WorldToLocalMatrixRH ) : // center がある場合は center からのローカル位置
                worldTailPositionRH;                                                              // center がない場合はワールド位置

            // 尻尾の１つ前の位置（＝初期位置）
            this._1つ前の尻尾の位置 = this._現在の尻尾の位置;
        }



        // 進行と描画


        public void Update( glTFNode center, float stiffnessForce, float dragForce, Vector3 external, List<SphereCollider> colliders )
        {
            Vector3 次の尻尾の位置;

            #region " (1) 「１つ前の尻尾」と「現在の尻尾」（と引数）から、「次の尻尾」のワールド位置を算出する。"
            //----------------
            // 「現在の尻尾」のワールド位置を算出する。
            var 現在の尻尾の位置 = ( center != null ) ?
                Vector3.TransformCoordinate( this._現在の尻尾の位置, center.LocalToWorldMatrixRH ) :    // center がある場合は center のワールド変換行列でワールド位置へ変換
                this._現在の尻尾の位置;                                                                 // center がない場合は最初からワールド位置なので何もしない

            // 「１つ前の尻尾」のワールド位置を算出する。
            var ひとつ前の尻尾の位置 = ( center != null ) ? 
                Vector3.TransformCoordinate( this._1つ前の尻尾の位置, center.LocalToWorldMatrixRH ) :   // 同上
                this._1つ前の尻尾の位置;                                                                //

            // verlet積分で「次の尻尾」のワールド位置を算出する。
            var 頭の親のワールド回転 = this.Head.Parent?.RotationRH ?? Quaternion.Identity;
            var 惰性と減衰 = ( 現在の尻尾の位置 - ひとつ前の尻尾の位置 ) * ( 1.0f - dragForce );
            var 頭の回転による移動 = Vector3.Transform( this.BoneAxis, this.InitialLocalHeadRotationRH * 頭の親のワールド回転 ) * stiffnessForce;
            var 外力による移動 = external;

            次の尻尾の位置 = 現在の尻尾の位置 + 惰性と減衰 + 頭の回転による移動 + 外力による移動;

            // 現在の頭のワールド位置に合わせて「次の尻尾」の位置を修正する。
            // 併せて、頭から「次の尻尾」までの長さを BoneLength にする。
            次の尻尾の位置 = this.Head.PositionRH + Vector3.Normalize( 次の尻尾の位置 - this.Head.PositionRH ) * this.BoneLength;
            //----------------
            #endregion

            #region " (2) 衝突判定を行って、「次の尻尾」の位置を修正する。"
            //----------------
            // すべてのコライダー（衝突器）ついて……
            foreach( var collider in colliders )
            {
                var 衝突距離 = this.Radius + collider.Radius;

                // コライダーと「次の尻尾」との距離 ≦ 衝突距離 なら、両者は衝突している。
                if( Vector3.DistanceSquared( 次の尻尾の位置, collider.Position ) <= ( 衝突距離 * 衝突距離 ) )
                {
                    // 衝突したので、「次の尻尾」の位置を、コライダーの中心から「次の尻尾」方向へ距離 r まで押し戻す。
                    var normal = Vector3.Normalize( 次の尻尾の位置 - collider.Position );
                    var posFromCollider = collider.Position + normal * 衝突距離;

                    次の尻尾の位置 = this.Head.PositionRH + Vector3.Normalize( posFromCollider - this.Head.PositionRH ) * this.BoneLength;    // 揺れボーンの長さは BoneLength に固定
                }
            }
            //----------------
            #endregion

            #region " (3) 尻尾を世代シフト。"
            //----------------
            // 「現在の尻尾」は「１つ前の尻尾」へシフト
            this._1つ前の尻尾の位置 = ( center != null ) ?
                Vector3.TransformCoordinate( 現在の尻尾の位置, center.WorldToLocalMatrixRH ) :  // center がある場合は center からのローカル位置
                現在の尻尾の位置;                                                               // center がない場合はワールド位置

            // 「１つ前の尻尾」は「現在の尻尾」へシフト
            this._現在の尻尾の位置 = ( center != null ) ?
                Vector3.TransformCoordinate( 次の尻尾の位置, center.WorldToLocalMatrixRH ) :   // 同上
                次の尻尾の位置;                                                                //
            //----------------
            #endregion

            #region " (4) 「次の尻尾」の位置から、頭のワールド回転を計算して設定する。"
            //----------------
            var 頭の初期のワールド回転 = this.InitialLocalHeadRotationRH * ( this.Head.Parent?.RotationRH ?? Quaternion.Identity );

            Vector3 from = Vector3.Transform( this.BoneAxis, 頭の初期のワールド回転 );
            Vector3 to = 次の尻尾の位置 - this.Head.PositionRH;
            var 頭が次の尻尾を向いた場合へのワールド回転差分 = from.ToRotation( to );

            // 頭の現在のワールド回転
            this.Head.RotationRH = 頭の初期のワールド回転 * 頭が次の尻尾を向いた場合へのワールド回転差分;

            //if( this.Head.Name == "hair1_L" )//&& this.Head.RotationRH.Axis.X == 1f )
            //    Debug.WriteLine( $"{this.Head.RotationRH.Axis}, {this.Head.RotationRH.Angle}" );
            //----------------
            #endregion
        }



        // ローカル


        /// <summary>
        ///     揺れボーンの尻尾の現在のワールド位置（またはcenterからのローカル位置）
        /// </summary>
        private Vector3 _現在の尻尾の位置;

        /// <summary>
        ///     揺れボーンの尻尾の１つ前のワールド位置（またはcenterからのローカル位置）
        /// </summary>
        private Vector3 _1つ前の尻尾の位置;
    }
}
