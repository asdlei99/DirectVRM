using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFMaterial : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public glTFMaterialNormalTextureInfo NormalTexture { get; }

        public glTFMaterialPbrMetallicRoughness PbrMetallicRoughness { get; }

        public bool DoubleSided => this._Native.DoubleSided;

        public glTFMaterialOcclusionTextureInfo OcclusionTexture { get; }

        public glTFTextureInfo EmissiveTexture { get; }

        public float[] EmissiveFactor => this._Native.EmissiveFactor ?? new float[ 0 ];

        public glTFLoader.Schema.Material.AlphaModeEnum AlphaMode => this._Native.AlphaMode;

        public float AlphaCutoff => this._Native.AlphaCutoff;

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFMaterial( int objectIndex, glTFLoader.Schema.Material native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            // NormalTexture
            this.NormalTexture = ( null != this._Native.NormalTexture ) ?
                new glTFMaterialNormalTextureInfo( this._Native.NormalTexture ) :
                null;

            // PbrMetallicRoughness
            this.PbrMetallicRoughness = ( null != this._Native.PbrMetallicRoughness ) ?
                new glTFMaterialPbrMetallicRoughness( this._Native.PbrMetallicRoughness ) :
                null;

            // OcclusionTexture
            this.OcclusionTexture = ( null != this._Native.OcclusionTexture ) ?
                new glTFMaterialOcclusionTextureInfo( this._Native.OcclusionTexture ) :
                null;

            // EmissiveTexture
            this.EmissiveTexture = ( null != this._Native.EmissiveTexture ) ?
                new glTFTextureInfo( this._Native.EmissiveTexture ) :
                null;
        }

        public void LateBinding( glTF gltf )
        {
            this.NormalTexture?.LateBinding( gltf );
            this.PbrMetallicRoughness?.LateBinding( gltf );
            this.OcclusionTexture?.LateBinding( gltf );
            this.EmissiveTexture?.LateBinding( gltf );
        }

        public virtual void Dispose()
        {
            this.NormalTexture?.Dispose();
            this.PbrMetallicRoughness?.Dispose();
            this.OcclusionTexture?.Dispose();
            this.EmissiveTexture?.Dispose();

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Material _Native;
    }
}
