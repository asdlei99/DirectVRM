using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAsset : IDisposable
    {
        public string Copyright => this._Native.Copyright ?? "";

        public string Generator => this._Native.Generator ?? "";

        public string Version => this._Native.Version ?? "";

        public string MinVersion => this._Native.MinVersion ?? "";

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFAsset( glTFLoader.Schema.Asset native )
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


        private glTFLoader.Schema.Asset _Native;
    }
}
