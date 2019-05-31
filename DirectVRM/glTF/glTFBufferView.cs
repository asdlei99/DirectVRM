using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public partial class glTFBufferView : IDisposable
    {
        public glTFBuffer Buffer { get; protected set; }

        /// <summary>
        ///     BufferView のデータ部分のストリーム。
        ///     Closeせずに、何度も使いまわす。
        /// </summary>
        public SharpDX.DataStream DataStream { get; protected set; }

        public int ByteOffset => this._Native.ByteOffset;

        public int ByteLength => this._Native.ByteLength;

        public int? ByteStride => this._Native.ByteStride;

        public glTFLoader.Schema.BufferView.TargetEnum? Target => this._Native.Target;

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFBufferView( int objectIndex, glTFLoader.Schema.BufferView native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            this.Buffer = null;
            this.DataStream = null;
        }

        public void LateBinding( glTF gltf )
        {
            // Buffer
            this.Buffer = gltf.Buffers[ this._Native.Buffer ];

            // DataStream
            this.DataStream = new SharpDX.DataStream(
                new IntPtr( this.Buffer.BufferAddress.ToInt64() + this._Native.ByteOffset ),
                this._Native.ByteLength,
                canRead: true, canWrite: false );
        }

        public virtual void Dispose()
        {
            this.Buffer = null; // disposeしない
            this.DataStream?.Dispose();

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.BufferView _Native;
    }
}
