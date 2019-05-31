using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class VRMSecondaryAnimationCollider : IDisposable
    {
        public Vector3 Offset => this._Native.Offset;

        public float Radius => this._Native.Radius;



        // 生成と終了


        public VRMSecondaryAnimationCollider( glTF_VRM_SecondaryAnimationCollider native )
        {
            this._Native = native;
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
        }

        public virtual void Dispose()
        {
        }



        // ローカル


        private glTF_VRM_SecondaryAnimationCollider _Native;
    }
}
