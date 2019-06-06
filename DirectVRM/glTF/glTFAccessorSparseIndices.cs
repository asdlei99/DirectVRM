using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAccessorSparseIndices : IDisposable
    {
        public glTFBufferView BufferView { get; protected set; }

        public int ByteOffset => this._Native.ByteOffset;

        public glTFLoader.Schema.AccessorSparseIndices.ComponentTypeEnum ComponentType => this._Native.ComponentType;

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFAccessorSparseIndices( glTFLoader.Schema.AccessorSparseIndices native )
        {
            this._Native = native;
            this.BufferView = null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
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

        private glTFLoader.Schema.AccessorSparseIndices _Native;
    }
}
