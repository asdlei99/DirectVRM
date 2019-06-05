using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMFirstPersonMeshAnnotation : IDisposable
    {
        public glTFMesh Mesh { get; protected set; }

        public VRMFirstPersonFlag FirstPersonFlag => this._Native.FirstPersonFlag;



        // 生成と終了


        public VRMFirstPersonMeshAnnotation( glTF_VRM_FirstPersonMeshAnnotation native )
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


        private glTF_VRM_FirstPersonMeshAnnotation _Native;
    }
}
