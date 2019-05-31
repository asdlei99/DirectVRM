using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFCameraPerspective : IDisposable
    {
        public float? AspectRatio => this._Native.AspectRatio;

        public float Yfov => this._Native.Yfov;

        public float? Zfar => this._Native.Zfar;

        public float Znear => this._Native.Znear;

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFCameraPerspective( glTFLoader.Schema.CameraPerspective native )
        {
            this._Native = native;
        }

        public void LateBinding( glTF gltf )
        {
        }

        public virtual void Dispose()
        {
            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.CameraPerspective _Native;
    }
}
