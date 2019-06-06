using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAnimationChannelTarget : IDisposable
    {
        public glTFNode Node { get; protected set; }

        public glTFLoader.Schema.AnimationChannelTarget.PathEnum Path => this._Native.Path;

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFAnimationChannelTarget( glTFLoader.Schema.AnimationChannelTarget native )
        {
            this._Native = native;
            this.Node = null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf )
        {
            // Node
            this.Node = ( this._Native.Node.HasValue ) ?
                gltf.Nodes[ this._Native.Node.Value ] :
                null;
        }

        public virtual void Dispose()
        {
            this.Node = null;   // disposeしない

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.AnimationChannelTarget _Native;
    }
}
