using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class VRMHumanoidBone : IDisposable
    {
        public VRMBoneType Bone => this._Native.Bone;

        public glTFNode Node { get; protected set; }

        public bool UseDefaultValues => this._Native.UseDefaultValues;

        public Vector3? Min => this._Native.Min;

        public Vector3? Max => this._Native.Max;

        public Vector3? Center => this._Native.Center;

        public float? AxisLength => this._Native.AxisLength;



        // 生成と終了


        public VRMHumanoidBone( glTF_VRM_HumanoidBone native )
        {
            this._Native = native;
            this.Node = null;
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // Node
            this.Node = ( this._Native.Node.HasValue && 0 <= this._Native.Node.Value ) ?
                gltf.Nodes[ this._Native.Node.Value ] :
                null;
        }

        public virtual void Dispose()
        {
            this.Node = null;   // disposeしない
        }



        // ローカル


        private glTF_VRM_HumanoidBone _Native;
    }
}
