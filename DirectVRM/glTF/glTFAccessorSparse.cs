using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAccessorSparse : IDisposable
    {
        public int Count => this._Native.Count;

        public glTFAccessorSparseIndices Indices { get; }

        public glTFAccessorSparseValues Values { get; }

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFAccessorSparse( glTFLoader.Schema.AccessorSparse native )
        {
            this._Native = native;

            // Indices
            this.Indices = ( null != this._Native.Indices ) ?
                new glTFAccessorSparseIndices( this._Native.Indices ) :
                null;

            // Values
            this.Values = ( null != this._Native.Values ) ?
                new glTFAccessorSparseValues( this._Native.Values ) :
                null;
        }

        public void LateBinding( glTF gltf )
        {
            this.Indices?.LateBinding( gltf );
            this.Values?.LateBinding( gltf );
        }

        public virtual void Dispose()
        {
            this.Values?.Dispose();
            this.Indices?.Dispose();
        }



        // ローカル


        private glTFLoader.Schema.AccessorSparse _Native;
    }
}
