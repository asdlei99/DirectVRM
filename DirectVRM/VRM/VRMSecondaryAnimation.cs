using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class VRMSecondaryAnimation : IDisposable
    {
        public VRMSecondaryAnimationSpring[] BoneGroups { get; protected set; }

        public VRMSecondaryAnimationColliderGroup[] ColliderGroups { get; protected set; }



        // 生成と終了


        public VRMSecondaryAnimation( glTF_VRM_SecondaryAnimation native )
        {
            this._Native = native;

            #region " BoneGroups "
            //----------------
            if( null != this._Native.BoneGroups )
            {
                this.BoneGroups = new VRMSecondaryAnimationSpring[ this._Native.BoneGroups.Length ];

                for( int i = 0; i < this._Native.BoneGroups.Length; i++ )
                {
                    this.BoneGroups[ i ] = new VRMSecondaryAnimationSpring( this._Native.BoneGroups[ i ] );
                }
            }
            else
            {
                this.BoneGroups = new VRMSecondaryAnimationSpring[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " ColliderGroups "
            //----------------
            if( null != this._Native.ColliderGroups )
            {
                this.ColliderGroups = new VRMSecondaryAnimationColliderGroup[ this._Native.ColliderGroups.Length ];

                for( int i = 0; i < this._Native.ColliderGroups.Length; i++ )
                {
                    this.ColliderGroups[ i ] = new VRMSecondaryAnimationColliderGroup( this._Native.ColliderGroups[ i ] );
                }
            }
            else
            {
                this.ColliderGroups = new VRMSecondaryAnimationColliderGroup[ 0 ];  // 未定義時は空配列（not null）
            }
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // BoneGroups
            foreach( var bg in this.BoneGroups )
                bg.LateBinding( gltf, vrm, this );

            // ColliderGroups
            foreach( var cg in this.ColliderGroups )
                cg.LateBinding( gltf, vrm );
        }

        public virtual void Dispose()
        {
            // BoneGroups
            foreach( var bg in this.BoneGroups )
                bg.Dispose();

            // ColliderGroups
            foreach( var cg in this.ColliderGroups )
                cg.Dispose();
        }



        // 進行と描画


        public void Update( double 現在時刻sec )
        {
            if( 0.0 > this._前回の時刻sec )
                this._前回の時刻sec = 現在時刻sec;

            double 経過時間sec = 現在時刻sec - this._前回の時刻sec;
            this._前回の時刻sec = 現在時刻sec;

            // すべての揺れボーンを更新する。
            foreach( var spring in this.BoneGroups )
            {
                spring.Update( 経過時間sec );
            }
        }



        // ローカル


        private glTF_VRM_SecondaryAnimation _Native;

        private double _前回の時刻sec = -1.0;
    }
}
