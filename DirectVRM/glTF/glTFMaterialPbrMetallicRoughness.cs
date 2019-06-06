using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFMaterialPbrMetallicRoughness : IDisposable
    {
        /// <summary>
        ///     float[4]。
        /// </summary>
        public float[] BaseColorFactor => this._Native.BaseColorFactor ?? new float[] { 1f, 1f, 1f, 1f };

        public glTFTextureInfo BaseColorTexture { get; }

        public float MetallicFactor => this._Native.MetallicFactor;

        public float RoughnessFactor => this._Native.RoughnessFactor;

        public glTFTextureInfo MetallicRoughnessTexture { get; }

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFMaterialPbrMetallicRoughness( glTFLoader.Schema.MaterialPbrMetallicRoughness native )
        {
            this._Native = native;

            // BaseColorTexture
            this.BaseColorTexture = ( null != this._Native.BaseColorTexture ) ?
                new glTFTextureInfo( this._Native.BaseColorTexture ) :
                null;

            // MetallicRoughnessTexture
            this.MetallicRoughnessTexture = ( null != this._Native.MetallicRoughnessTexture ) ?
                new glTFTextureInfo( this._Native.MetallicRoughnessTexture ) :
                null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf )
        {
            this.BaseColorTexture?.LateBinding( gltf );
            this.MetallicRoughnessTexture?.LateBinding( gltf );
        }

        public virtual void Dispose()
        {
            this.MetallicRoughnessTexture?.Dispose();
            this.BaseColorTexture?.Dispose();

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.MaterialPbrMetallicRoughness _Native;
    }
}
