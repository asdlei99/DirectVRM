using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMBlendShapeMaterialValueBind : IDisposable
    {
        public string MaterialName => this._Native.MaterialName ?? "";

        public string PropertyName => this._Native.PropertyName ?? "";

        /// <summary>
        ///     未定義時はnull
        /// </summary>
        public float[] TargetValue => this._Native.TargetValue;



        // 生成と終了


        public VRMBlendShapeMaterialValueBind( glTF_VRM_BlendShapeMaterialValueBind native )
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


        private glTF_VRM_BlendShapeMaterialValueBind _Native;
    }
}
