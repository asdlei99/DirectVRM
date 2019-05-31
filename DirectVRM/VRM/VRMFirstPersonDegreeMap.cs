using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMFirstPersonDegreeMap : IDisposable
    {
        public float[] Curve => this._Native.Curve;

        public float XRange => this._Native.XRange;

        public float YRange => this._Native.YRange;



        // 生成と終了


        public VRMFirstPersonDegreeMap( glTF_VRM_FirstPersonDegreeMap native )
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


        private glTF_VRM_FirstPersonDegreeMap _Native;
    }
}
