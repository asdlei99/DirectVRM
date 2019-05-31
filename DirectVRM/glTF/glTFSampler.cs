using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFSampler : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public glTFLoader.Schema.Sampler.MinFilterEnum? MinFilter => this._Native.MinFilter;

        public glTFLoader.Schema.Sampler.MagFilterEnum? MagFilter => this._Native.MagFilter;

        public glTFLoader.Schema.Sampler.WrapSEnum WrapS => this._Native.WrapS;

        public glTFLoader.Schema.Sampler.WrapTEnum WrapT => this._Native.WrapT;

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }


        public SharpDX.Direct3D11.SamplerState D3DSamplerState { get; protected set; }



        // 生成と終了


        public glTFSampler( int objectIndex, glTFLoader.Schema.Sampler native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            this.D3DSamplerState = null;
        }

        public void LateBinding( glTF gltf, SharpDX.Direct3D11.Device d3dDevice )
        {
            // サンプラーステートを作成する。
            this.D3DSamplerState = new SharpDX.Direct3D11.SamplerState(
                d3dDevice,
                new SharpDX.Direct3D11.SamplerStateDescription {
                    AddressU = this._WrapSMap[ this.WrapS ],
                    AddressV = this._WrapTMap[ this.WrapT ],
                    AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                    Filter = this._FilterMap[ this.MinFilter ?? glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST ][ this.MagFilter ?? glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST ],
                    MinimumLod = float.MinValue,
                    MaximumLod = float.MaxValue,
                    MipLodBias = 0.0f,
                    MaximumAnisotropy = 1,
                    ComparisonFunction = SharpDX.Direct3D11.Comparison.Never,
                    BorderColor = new SharpDX.Color4( 1f, 1f, 1f, 1f ),
                } );
        }

        public virtual void Dispose()
        {
            this.D3DSamplerState?.Dispose();
            this.D3DSamplerState = null;

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Sampler _Native;

        // WrapS/V Map (GL to D3D11)
        private readonly Dictionary<glTFLoader.Schema.Sampler.WrapSEnum, SharpDX.Direct3D11.TextureAddressMode> _WrapSMap = new Dictionary<glTFLoader.Schema.Sampler.WrapSEnum, SharpDX.Direct3D11.TextureAddressMode>() {
            [ glTFLoader.Schema.Sampler.WrapSEnum.CLAMP_TO_EDGE ] = SharpDX.Direct3D11.TextureAddressMode.Clamp,
            [ glTFLoader.Schema.Sampler.WrapSEnum.MIRRORED_REPEAT ] = SharpDX.Direct3D11.TextureAddressMode.Mirror,
            [ glTFLoader.Schema.Sampler.WrapSEnum.REPEAT ] = SharpDX.Direct3D11.TextureAddressMode.Wrap,
        };
        // WrapT/U Map (GL to D3D11)
        private readonly Dictionary<glTFLoader.Schema.Sampler.WrapTEnum, SharpDX.Direct3D11.TextureAddressMode> _WrapTMap = new Dictionary<glTFLoader.Schema.Sampler.WrapTEnum, SharpDX.Direct3D11.TextureAddressMode>() {
            [ glTFLoader.Schema.Sampler.WrapTEnum.CLAMP_TO_EDGE ] = SharpDX.Direct3D11.TextureAddressMode.Clamp,
            [ glTFLoader.Schema.Sampler.WrapTEnum.MIRRORED_REPEAT ] = SharpDX.Direct3D11.TextureAddressMode.Mirror,
            [ glTFLoader.Schema.Sampler.WrapTEnum.REPEAT ] = SharpDX.Direct3D11.TextureAddressMode.Wrap,
        };
        // Filter Map (GL to D3D11)
        private readonly Dictionary<glTFLoader.Schema.Sampler.MinFilterEnum, Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter>> _FilterMap = new Dictionary<glTFLoader.Schema.Sampler.MinFilterEnum, Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter>>() {
            [ glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR ] = new Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter> {
                [ glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR ] = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint,
                [ glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST ] = SharpDX.Direct3D11.Filter.MinLinearMagMipPoint,
            },
            [ glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR ] = new Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter> {
                [ glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR ] = SharpDX.Direct3D11.Filter.MinMagMipLinear,
                [ glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST ] = SharpDX.Direct3D11.Filter.MinLinearMagPointMipLinear,
            },
            [ glTFLoader.Schema.Sampler.MinFilterEnum.LINEAR_MIPMAP_NEAREST ] = new Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter> {
                [ glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR ] = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint,
                [ glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST ] = SharpDX.Direct3D11.Filter.MinLinearMagMipPoint,
            },
            [ glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST ] = new Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter> {
                [ glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR ] = SharpDX.Direct3D11.Filter.MinPointMagLinearMipPoint,
                [ glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST ] = SharpDX.Direct3D11.Filter.MinMagMipPoint,
            },
            [ glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_LINEAR ] = new Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter> {
                [ glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR ] = SharpDX.Direct3D11.Filter.MinPointMagMipLinear,
                [ glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST ] = SharpDX.Direct3D11.Filter.MinMagPointMipLinear,
            },
            [ glTFLoader.Schema.Sampler.MinFilterEnum.NEAREST_MIPMAP_NEAREST ] = new Dictionary<glTFLoader.Schema.Sampler.MagFilterEnum, SharpDX.Direct3D11.Filter> {
                [ glTFLoader.Schema.Sampler.MagFilterEnum.LINEAR ] = SharpDX.Direct3D11.Filter.MinPointMagLinearMipPoint,
                [ glTFLoader.Schema.Sampler.MagFilterEnum.NEAREST ] = SharpDX.Direct3D11.Filter.MinMagMipPoint,
            },
        };
    }
}
