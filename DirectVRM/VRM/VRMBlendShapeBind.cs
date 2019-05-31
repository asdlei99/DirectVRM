using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMBlendShapeBind : IDisposable
    {
        public glTFMesh Mesh { get; protected set; }

        public int? Index => this._Native.Index;

        public float Weight => this._Native.Weight;



        // 生成と終了


        public VRMBlendShapeBind( glTF_VRM_BlendShapeBind native )
        {
            this._Native = native;
            this.Mesh = null;
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // Mesh
            this.Mesh = ( this._Native.Mesh.HasValue && 0 <= this._Native.Mesh.Value ) ?
                gltf.Meshes[ this._Native.Mesh.Value ] :
                null;
        }

        public virtual void Dispose()
        {
            this.Mesh = null;       // disposeしない
        }



        // ローカル


        private glTF_VRM_BlendShapeBind _Native;
    }
}
