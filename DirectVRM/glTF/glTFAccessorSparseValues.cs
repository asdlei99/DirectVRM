using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAccessorSparseValues : IDisposable
    {
        public glTFBufferView BufferView { get; protected set; }

        public int ByteOffset => this._Native.ByteOffset;

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFAccessorSparseValues( glTFLoader.Schema.AccessorSparseValues native )
        {
            this._Native = native;
            this.BufferView = null;
        }

        public void LateBinding( glTF gltf )
        {
            this.BufferView = gltf.BufferViews[ this._Native.BufferView ];
        }

        public virtual void Dispose()
        {
            this.BufferView = null; // disposeしない
        }



        // ローカル


        private glTFLoader.Schema.AccessorSparseValues _Native;
    }
}
