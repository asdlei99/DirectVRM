using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFImage : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public string Uri => this._Native.Uri ?? "";

        public glTFLoader.Schema.Image.MimeTypeEnum? MimeType => this._Native.MimeType;

        public glTFBufferView BufferView { get; protected set; }

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFImage( int objectIndex, glTFLoader.Schema.Image native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            this.BufferView = null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf )
        {
            // BufferView
            this.BufferView = ( this._Native.BufferView.HasValue ) ?
                gltf.BufferViews[ this._Native.BufferView.Value ] :
                null;
        }

        public virtual void Dispose()
        {
            this.BufferView = null; // disposeはしない

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Image _Native;
    }
}
