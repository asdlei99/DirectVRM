using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFCameraOrthographic : IDisposable
    {
        public float Xmag => this._Native.Xmag;

        public float Ymag => this._Native.Ymag;

        public float Zfar => this._Native.Zfar;

        public float Znear => this._Native.Znear;

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFCameraOrthographic( glTFLoader.Schema.CameraOrthographic native )
        {
            this._Native = native;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf )
        {
        }

        public virtual void Dispose()
        {
            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.CameraOrthographic _Native;
    }
}
