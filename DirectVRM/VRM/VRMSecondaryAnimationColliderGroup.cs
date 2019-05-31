using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMSecondaryAnimationColliderGroup : IDisposable
    {
        public glTFNode Node { get; protected set; }

        public VRMSecondaryAnimationCollider[] Colliders { get; protected set; }



        // 生成と終了


        public VRMSecondaryAnimationColliderGroup( glTF_VRM_SecondaryAnimationColliderGroup native )
        {
            this._Native = native;

            this.Node = null;

            #region " Colliders "
            //----------------
            if( null != this._Native.Colliders )
            {
                this.Colliders = new VRMSecondaryAnimationCollider[ this._Native.Colliders.Length ];

                for( int i = 0; i < this._Native.Colliders.Length; i++ )
                {
                    this.Colliders[ i ] = new VRMSecondaryAnimationCollider( this._Native.Colliders[ i ] );
                }
            }
            else
            {
                this.Colliders = new VRMSecondaryAnimationCollider[ 0 ];    // 未定義時は空配列（not null）
            }
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // Node
            this.Node = ( this._Native.Node.HasValue && 0 <= this._Native.Node.Value ) ?
                gltf.Nodes[ this._Native.Node.Value ] :
                null;

            // Colliders
            foreach( var col in this.Colliders )
                col.LateBinding( gltf, vrm );
        }

        public virtual void Dispose()
        {
            // Node
            this.Node = null;       // disposeしない

            // Colliders
            foreach( var coll in this.Colliders )
                coll.Dispose();
        }



        // ローカル


        private glTF_VRM_SecondaryAnimationColliderGroup _Native;
    }
}
