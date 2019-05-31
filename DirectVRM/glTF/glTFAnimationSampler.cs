using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAnimationSampler : IDisposable
    {
        public glTFAccessor Input { get; protected set; }

        public glTFAccessor Output { get; protected set; }

        public glTFLoader.Schema.AnimationSampler.InterpolationEnum Interpolation => this._Native.Interpolation;

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFAnimationSampler( glTFLoader.Schema.AnimationSampler native )
        {
            this._Native = native;

            this.Input = null;
            this.Output = null;
        }

        public void LateBinding( glTF gltf )
        {
            this.Input = gltf.Accessors[ this._Native.Input ];
            this.Output = gltf.Accessors[ this._Native.Output ];
        }

        public virtual void Dispose()
        {
            this.Input = null;  // disposeしない
            this.Output = null; // disposeしない

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.AnimationSampler _Native;
    }
}
