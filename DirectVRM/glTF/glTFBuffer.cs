using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace DirectVRM
{
    public class glTFBuffer : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public string Uri => this._Native.Uri ?? "";

        public byte[] Buffer { get; protected set; }

        public IntPtr BufferAddress => this._gchBinaryBuffer.AddrOfPinnedObject();

        public int ByteLength => this._Native.ByteLength;

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFBuffer( int objectIndex, glTFLoader.Schema.Buffer native )
        {
            this.ObjectIndex = objectIndex;
            this._Native = native;

            this.Buffer = null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( byte[] buffer )
        {
            // Buffer
            this.Buffer = buffer;

            // Buffer を pinned
            this._gchBinaryBuffer = GCHandle.Alloc( this.Buffer, GCHandleType.Pinned );
        }

        public virtual void Dispose()
        {
            // バッファの pinned を解放する。
            if( this._gchBinaryBuffer.IsAllocated )
                this._gchBinaryBuffer.Free();

            // Buffer
            this.Buffer = null;

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Buffer _Native;

        private GCHandle _gchBinaryBuffer;    // これで Pinned されている。
    }
}
