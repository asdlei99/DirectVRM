using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFScene : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        /// <summary>
        ///     ルートノードの配列
        /// </summary>
        public glTFNode[] Nodes { get; protected set; }

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFScene( int objectIndex, glTFLoader.Schema.Scene native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            // Nodes
            this.Nodes = ( null != this._Native.Nodes ) ?
                new glTFNode[ this._Native.Nodes.Length ] :
                new glTFNode[ 0 ]; // 未定義時は空配列（not null）
        }

        public void LateBinding( glTF gltf )
        {
            // Nodes
            for( int i = 0; i < this.Nodes.Length; i++ )
                this.Nodes[ i ] = gltf.Nodes[ this._Native.Nodes[ i ] ];
        }

        public virtual void Dispose()
        {
            this.Nodes = null;  // disposeしない
        }



        // ローカル


        private glTFLoader.Schema.Scene _Native;
    }
}
