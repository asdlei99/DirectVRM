using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFMaterialNormalTextureInfo : IDisposable
    {
        public glTFTexture Texture { get; protected set; }

        /// <summary>
        ///     TEXCOORDn の n 値。
        /// </summary>
        public int TexCoord => this._Native.TexCoord;

        /// <summary>
        ///     法線テクスチャの各法線ベクトルに適用されるスカラ乗数。
        /// </summary>
        public float Scale => this._Native.Scale;

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFMaterialNormalTextureInfo( glTFLoader.Schema.MaterialNormalTextureInfo native )
        {
            this._Native = native;

            this.Texture = null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf )
        {
            this.Texture = gltf.Textures[ this._Native.Index ];
        }

        public virtual void Dispose()
        {
            this.Texture = null;    // disposeしない

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.MaterialNormalTextureInfo _Native;
    }
}
