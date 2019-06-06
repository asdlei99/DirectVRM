using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAnimationChannel : IDisposable
    {
        public glTFAnimationSampler Sampler { get; protected set; }

        public glTFAnimationChannelTarget Target { get; protected set; }

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFAnimationChannel( glTFLoader.Schema.AnimationChannel native )
        {
            this._Native = native;

            this.Sampler = null;

            // Target
            this.Target = new glTFAnimationChannelTarget( this._Native.Target );

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf, glTFAnimation animation )
        {
            this.Sampler = animation.Samplers[ this._Native.Sampler ];
            this.Target?.LateBinding( gltf );
        }

        public virtual void Dispose()
        {
            this.Sampler = null;    // disposeしない

            this.Target?.Dispose();
            this.Target = null;

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.AnimationChannel _Native;
    }
}
