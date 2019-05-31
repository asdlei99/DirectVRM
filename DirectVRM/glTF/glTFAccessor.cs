using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAccessor : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        /// <summary>
        ///     アクセサ型のサイズ（例: "VEC3" → 3）
        /// </summary>
        public int TypeSize => _TypeTo要素数[ this._Native.Type ];

        /// <summary>
        ///     コンポーネント型のサイズ（例: FLOAT → 4）
        /// </summary>
        public int ComponentTypeSize => _ComponentToバイト数[ this._Native.ComponentType ];

        public glTFLoader.Schema.Accessor.TypeEnum Type => this._Native.Type;

        public glTFLoader.Schema.Accessor.ComponentTypeEnum ComponentType => this._Native.ComponentType;

        /// <summary>
        ///     アクセス対象のバッファビュー。
        /// </summary>
        public glTFBufferView BufferView { get; protected set; }

        /// <summary>
        ///     <see cref="BufferView"/>の先頭からのオフセット。
        /// </summary>
        public int ByteOffset => this._Native.ByteOffset;

        /// <summary>
        ///     要素数。
        /// </summary>
        public int Count => this._Native.Count;

        /// <summary>
        ///     要素の最小値。未定義時は null。
        /// </summary>
        public float[] Min => this._Native.Min;

        /// <summary>
        ///     要素の最大値。未定義時は null。
        /// </summary>
        public float[] Max => this._Native.Max;

        public bool Normalized => this._Native.Normalized;

        /// <summary>
        ///     未定義時は null
        /// </summary>
        public glTFAccessorSparse Sparse { get; }

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFAccessor( int objectIndex, glTFLoader.Schema.Accessor native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            // BufferView
            this.BufferView = null;

            // Sparse
            this.Sparse = ( null != this._Native.Sparse ) ?
                new glTFAccessorSparse( this._Native.Sparse ) :
                null;
        }

        public void LateBinding( glTF gltf )
        {
            // BufferView
            if( this._Native.BufferView.HasValue )
            {
                this.BufferView = gltf.BufferViews[ this._Native.BufferView.Value ];
            }
            else
            {
                // todo: sparse アクセサの ByteBuffer の実装
                // sparse アクセサの場合、bufferView は省略される。
                // sparse アクセサは、サイズが (size of the accessor element) * (accessor.count)バイトのゼロの配列として初期化される。
                this.BufferView = null;
            }

            // Sparse
            this.Sparse?.LateBinding( gltf );
        }

        public virtual void Dispose()
        {
            this.BufferView = null; // disposeしない
            this.Sparse?.Dispose();

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Accessor _Native;

        private static readonly Dictionary<glTFLoader.Schema.Accessor.TypeEnum, int> _TypeTo要素数 = new Dictionary<glTFLoader.Schema.Accessor.TypeEnum, int>() {
            [ glTFLoader.Schema.Accessor.TypeEnum.SCALAR ] = 1,
            [ glTFLoader.Schema.Accessor.TypeEnum.VEC2 ] = 2,
            [ glTFLoader.Schema.Accessor.TypeEnum.VEC3 ] = 3,
            [ glTFLoader.Schema.Accessor.TypeEnum.VEC4 ] = 4,
            [ glTFLoader.Schema.Accessor.TypeEnum.MAT2 ] = 4,
            [ glTFLoader.Schema.Accessor.TypeEnum.MAT3 ] = 9,
            [ glTFLoader.Schema.Accessor.TypeEnum.MAT4 ] = 16,
        };

        private static readonly Dictionary<glTFLoader.Schema.Accessor.ComponentTypeEnum, int> _ComponentToバイト数 = new Dictionary<glTFLoader.Schema.Accessor.ComponentTypeEnum, int>() {
            [ glTFLoader.Schema.Accessor.ComponentTypeEnum.BYTE ] = 1,
            [ glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE ] = 1,
            [ glTFLoader.Schema.Accessor.ComponentTypeEnum.SHORT ] = 2,
            [ glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT ] = 2,
            [ glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT ] = 4,
            [ glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT ] = 4,
        };
    }
}
