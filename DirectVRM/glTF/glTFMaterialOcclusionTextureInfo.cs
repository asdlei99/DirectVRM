using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFMaterialOcclusionTextureInfo : IDisposable
    {
        public glTFTexture Texture { get; protected set; }

        /// <summary>
        ///     TEXCOORDn の n 値。
        /// </summary>
        public int TexCoord => this._Native.TexCoord;

        /// <summary>
        ///     適用されるオクルージョンの量を制御するスカラ乗数。
        /// </summary>
        public float Strength => this._Native.Strength;

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFMaterialOcclusionTextureInfo( glTFLoader.Schema.MaterialOcclusionTextureInfo native )
        {
            this._Native = native;

            this.Texture = null;
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


        private glTFLoader.Schema.MaterialOcclusionTextureInfo _Native;
    }
}
