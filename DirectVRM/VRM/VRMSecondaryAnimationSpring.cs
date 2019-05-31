using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///     VRMBoneSpring
    /// </summary>
    public class VRMSecondaryAnimationSpring : IDisposable
    {
        /// <summary>
        ///     注釈コメント。
        /// </summary>
        public string Comment => this._Native.Comment ?? "";

        /// <summary>
        ///     剛性、弾力性（最初のポーズに戻る力）。
        ///     0.0～4.0。
        /// </summary>
        public float Stiffness => this._Native.Stiffiness;

        /// <summary>
        ///     重力。
        ///     0.0～2.0。
        /// </summary>
        public float GravityPower => this._Native.GravityPower;

        /// <summary>
        ///     重力の働く方向（通常は (0, -1, 0) ）
        /// </summary>
        /// <remarks>
        ///     風などを考慮する場合は斜めにする
        /// </remarks>
        public Vector3 GravityDir => this._Native.GravityDir;

        /// <summary>
        ///     減速力（増やすほど抵抗が増える）。
        ///     0.0～1.0。
        /// </summary>
        public float DragForce => this._Native.DragForce;

        /// <summary>
        ///     当たり判定（コライダーとの衝突検出）用の球の半径。
        ///     0.0～0.5。
        /// </summary>
        public float HitRadius => this._Native.HitRadius;

        /// <summary>
        ///     揺れの中央位置を示すノード。
        ///     未定義ならnull。
        /// </summary>
        /// <remarks>
        ///     揺れ物の基準点は、原点を除く任意の場所に設定できる。
        ///     設定されている場合、すべての揺れ者はそのノードを原点とするローカル空間上での変換となる。
        ///     
        ///     例えば、モデルを瞬間移動させたときの揺れ物への影響が大きいが、
        ///     モデル内のノード（揺れ物と一緒に瞬間移動する）を揺れ物の基準点とすることで、
        ///     その影響をなくすことができる。
        /// </remarks>
        public glTFNode Center { get; protected set; }

        /// <summary>
        ///     モデル内のすべての揺れ物の、ルートノードの配列。
        ///     ひとつも定義されていないなら空配列（null ではない）。
        /// </summary>
        public glTFNode[] RootBones { get; protected set; }

        /// <summary>
        ///     衝突判定に使用するコライダーのグループの配列。
        ///     未定義時は空配列（nullではない）。
        /// </summary>
        public VRMSecondaryAnimationColliderGroup[] ColliderGroups { get; protected set; }



        // 生成と終了


        public VRMSecondaryAnimationSpring( glTF_VRM_SecondaryAnimationSpring native )
        {
            this._Native = native;

            this.RootBones = ( null != this._Native.Bones ) ?
                new glTFNode[ this._Native.Bones.Length ] :
                new glTFNode[ 0 ]; // 未定義時は空配列（not null）

            this.Center = null;

            this.ColliderGroups = ( null != this._Native.ColliderGroups ) ?
                new VRMSecondaryAnimationColliderGroup[ this._Native.ColliderGroups.Length ] :
                new VRMSecondaryAnimationColliderGroup[ 0 ];  // 未定義時は空配列（not null）
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm, VRMSecondaryAnimation animation )
        {
            // Bones
            for( int i = 0; i < this.RootBones.Length; i++ )
                this.RootBones[ i ] = gltf.Nodes[ this._Native.Bones[ i ] ];

            // Center
            this.Center = ( this._Native.Center.HasValue && 0 <= this._Native.Center.Value ) ?  // Center は -1 の場合がある（互換性のため）
                gltf.Nodes[ this._Native.Center.Value ] :
                null;

            // ColliderGroups
            for( int i = 0; i < this.ColliderGroups.Length; i++ )
                this.ColliderGroups[ i ] = animation.ColliderGroups[ this._Native.ColliderGroups[ i ] ];
        }

        public virtual void Dispose()
        {
            this.Center = null;         // disposeしない
            this.RootBones = null;          // disposeしない
            this.ColliderGroups = null; // disposeしない
        }


        /// <summary>
        ///     現在の揺れボーンを反映し、リセットする。
        /// </summary>
        private void _Setup( bool force = false )
        {
            if( 0 == this.RootBones.Length )
                return;

            // m_initialLocalRotationMap の生成、または適用してクリア。
            if( force || null == this.m_initialLocalRotationMap )
            {
                // 生成
                this.m_initialLocalRotationMap = new Dictionary<glTFNode, Quaternion>();
            }
            else
            {
                // 二回目以降の Setup 呼び出しなら、key のローカル回転を、現在のマップのローカル回転（初期ローカル回転）にリセット。
                foreach( var kvp in m_initialLocalRotationMap )
                    kvp.Key.LocalRotationRH = kvp.Value;

                m_initialLocalRotationMap.Clear();
            }

            // 揺れボーンリストをクリア。
            m_verlet.Clear();

            // すべてのルートボーンについて……
            foreach( var bone in RootBones )
            {
                if( null != bone )
                {
                    // ルートボーンとそのすべての子について、ローカル回転マップを設定する。
                    foreach( var x in bone.Traverse() )
                    {
                        // [ Transform ] =  Transform.localRotaion
                        this.m_initialLocalRotationMap[ x ] = x.LocalRotationRH;
                    }

                    // ルートボーンについて、再帰的に揺れボーンとして登録する。
                    this._SetupRecursive( this.Center, bone );
                }
            }
        }

        private void _SetupRecursive( glTFNode center, glTFNode parent )
        {
            // (1) 自分（parent）を追加する。
            if( 0 == parent.Children.Length )
            {
                // (1-A) 子がいない場合

                // delta = 親（parent.parent）から自分（parent）への方向（ワールド空間；単位ベクトル）
                var delta = Vector3.Normalize( parent.PositionRH - ( parent.Parent?.PositionRH ?? Vector3.Zero ) );

                // 仮想の子のワールド位置 = 自分のワールド位置＋親方向×0.07
                var childPosition = parent.PositionRH + delta * 0.07f;

                // 仮想の子を使って、自分を揺れボーンとして登録する。
                this.m_verlet.Add(
                    new VRMSpringBoneLogic( 
                        center, 
                        parent, 
                        Vector3.TransformCoordinate( childPosition, parent.WorldToLocalMatrixRH ) ) );
            }
            else
            {
                // (1-B) 子がいる場合

                var firstChild = parent.Children[ 0 ];              // 子が複数あったとしても、先頭の子を揺れボーンの尻尾とみなす。
                var localPosition = firstChild.LocalPositionRH;     // 先頭の子のローカル位置
                var scale = firstChild.LossyScale;                  // 先頭の子のlossyScale

                // lossyScale倍した先頭の子のローカル位置を使って、自分を揺れボーンとして登録。
                this.m_verlet.Add(
                    new VRMSpringBoneLogic(
                        center,
                        parent,
                        new Vector3(
                            localPosition.X * scale.X,
                            localPosition.Y * scale.Y,
                            localPosition.Z * scale.Z ) ) );
            }

            // (2) すべての子について再帰的に追加する。
            foreach( var child in parent.Children )
            {
                this._SetupRecursive( center, child );
            }
        }



        // 進行と描画


        public void Update( double 経過時間sec )
        {
            // 初めての更新なら先にSetupする。

            if( null == this.m_verlet || 0 == this.m_verlet.Count )
                this._Setup();


            // 衝突判定用球リストをコライダーグループのリストから再構築する。

            this.m_colliderList.Clear();

            if( null != this.ColliderGroups )
            {
                // すべてのコライダーグループについて……
                foreach( var group in this.ColliderGroups )
                {
                    if( null != group )
                    {
                        // グループ内のすべてのコライダーについて……
                        foreach( var collider in group.Colliders )
                        {
                            this.m_colliderList.Add( 
                                new SphereCollider {
                                    Position = Vector3.TransformCoordinate( collider.Offset, group.Node.LocalToWorldMatrixRH ), // Offsetは、グループのワールド空間内での位置に変換。
                                    Radius = collider.Radius,
                                } );
                        }
                    }
                }
            }

            // stiffness と external は、経過時間に比例した大きさに修正する。

            var stiffness = (float)( this.Stiffness * 経過時間sec );
            var external = this.GravityDir * (float)( this.GravityPower * 経過時間sec );


            // 揺れ物内のすべての揺れボーンを更新する。

            foreach( var verlet in this.m_verlet )
            {
                verlet.Radius = this.HitRadius;
                verlet.Update( this.Center, stiffness, this.DragForce, external, m_colliderList );
            }
        }



        // ローカル


        private glTF_VRM_SecondaryAnimationSpring _Native;

        private Dictionary<glTFNode, Quaternion> m_initialLocalRotationMap;

        /// <summary>
        ///     揺れ物（揺れボーンのリスト）。
        /// </summary>
        private List<VRMSpringBoneLogic> m_verlet = new List<VRMSpringBoneLogic>();

        private List<SphereCollider> m_colliderList = new List<SphereCollider>();
    }
}
