using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DirectVRM
{
    public static class D3DShaders
    {
        public static Dictionary<string, (SharpDX.Direct3D11.VertexShader shader, byte[] bytecode, SharpDX.Direct3D11.InputLayout layout)> VertexShaders { get; private set; }

        public static Dictionary<string, (SharpDX.Direct3D11.HullShader shader, byte[] bytecode)> HullShaders { get; private set; }

        public static Dictionary<string, (SharpDX.Direct3D11.DomainShader shader, byte[] bytecode)> DomainShaders { get; private set; }

        public static Dictionary<string, (SharpDX.Direct3D11.GeometryShader shader, byte[] bytecode)> GeometryShaders { get; private set; }

        public static Dictionary<string, (SharpDX.Direct3D11.PixelShader shader, byte[] bytecode)> PixelShaders { get; private set; }

        public static Dictionary<string, (SharpDX.Direct3D11.ComputeShader shader, byte[] bytecode)> ComputeShaders { get; private set; }

        public static SharpDX.Direct3D11.SamplerState D3DDefaultSamplerState { get; private set; }

        public static SharpDX.Direct3D11.Buffer D3DShaderParametersBuffer;


        static D3DShaders()
        {
            _wrD3DDevice = null;

            VertexShaders = new Dictionary<string, (SharpDX.Direct3D11.VertexShader shader, byte[] bytecode, SharpDX.Direct3D11.InputLayout layout)>();
            HullShaders = new Dictionary<string, (SharpDX.Direct3D11.HullShader shader, byte[] bytecode)>();
            DomainShaders = new Dictionary<string, (SharpDX.Direct3D11.DomainShader shader, byte[] bytecode)>();
            GeometryShaders = new Dictionary<string, (SharpDX.Direct3D11.GeometryShader shader, byte[] bytecode)>();
            PixelShaders = new Dictionary<string, (SharpDX.Direct3D11.PixelShader shader, byte[] bytecode)>();
            ComputeShaders = new Dictionary<string, (SharpDX.Direct3D11.ComputeShader shader, byte[] bytecode)>();
        }

        public static void Initialize( SharpDX.Direct3D11.Device d3dDevice )
        {
            _wrD3DDevice = new WeakReference<SharpDX.Direct3D11.Device>( d3dDevice );

            #region " ShaderParameters 定数バッファを作成する。"
            //----------------
            D3DShaderParametersBuffer = new SharpDX.Direct3D11.Buffer(
                d3dDevice,
                new SharpDX.Direct3D11.BufferDescription {
                    SizeInBytes = ShaderParameters.SizeInBytes,
                    BindFlags = SharpDX.Direct3D11.BindFlags.ConstantBuffer,
                } );
            //----------------
            #endregion

            #region " シェーダーマップ Standard を作成する。"
            //----------------
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.StandardVS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddVertexShader( "Standard", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.StandardPS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddPixelShader( "Standard", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.StandardHS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddHullShader( "Standard", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.StandardDS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddDomainShader( "Standard", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.StandardGS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddGeometryShader( "Standard", buffer );
            }
            //----------------
            #endregion
            #region " シェーダーマップ pbr を作成する。"
            //----------------
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.pbrVS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddVertexShader( "pbr", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.pbrPS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddPixelShader( "pbr", buffer );
            }
            //----------------
            #endregion
            #region " シェーダーマップ VRM/UnlitTexture を作成する。"
            //----------------
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.VRMUnlitTextureVS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddVertexShader( "VRM/UnlitTexture", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.VRMUnlitTexturePS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddPixelShader( "VRM/UnlitTexture", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.VRMUnlitTextureHS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddHullShader( "VRM/UnlitTexture", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.VRMUnlitTextureDS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddDomainShader( "VRM/UnlitTexture", buffer );
            }
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.VRMUnlitTextureGS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddGeometryShader( "VRM/UnlitTexture", buffer );
            }
            //----------------
            #endregion

            #region " 標準スキニングシェーダーを作成する。"
            //----------------
            using( var csoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( typeof( D3DShaders ), "D3D.cso.StandardSkinningCS.cso" ) )
            {
                var buffer = new byte[ csoStream.Length ];
                csoStream.Read( buffer, 0, buffer.Length );
                AddComputeShader( "Standard/Skinning", buffer );
            }
            //----------------
            #endregion

            #region " 既定のサンプラーを作成。"
            //----------------
            D3DDefaultSamplerState = new SharpDX.Direct3D11.SamplerState(
                d3dDevice,
                new SharpDX.Direct3D11.SamplerStateDescription {
                    AddressU = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                    AddressV = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                    AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                    Filter = SharpDX.Direct3D11.Filter.MinMagMipLinear,
                    MinimumLod = float.MinValue,
                    MaximumLod = float.MaxValue,
                    MipLodBias = 0.0f,
                    MaximumAnisotropy = 1,
                    ComparisonFunction = SharpDX.Direct3D11.Comparison.Never,
                    BorderColor = new SharpDX.Color4( 1f, 1f, 1f, 1f ),
                } );
            //----------------
            #endregion
        }

        public static void Release()
        {
            D3DDefaultSamplerState?.Dispose();
            D3DDefaultSamplerState = null;

            foreach( var ps in PixelShaders )
                ps.Value.shader?.Dispose();
            PixelShaders = null;

            foreach( var gs in GeometryShaders )
                gs.Value.shader?.Dispose();
            GeometryShaders = null;

            foreach( var ds in DomainShaders )
                ds.Value.shader?.Dispose();
            DomainShaders = null;

            foreach( var hs in HullShaders )
                hs.Value.shader?.Dispose();
            HullShaders = null;

            foreach( var vs in VertexShaders )
            {
                vs.Value.layout?.Dispose();
                vs.Value.shader?.Dispose();
            }
            VertexShaders = null;

            D3DShaderParametersBuffer?.Dispose();
            D3DShaderParametersBuffer = null;

            _wrD3DDevice = null;
        }


        public static void UpdateGrobalParametersBuffer( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters )
        {
            d3ddc.UpdateSubresource( ref shaderParameters, D3DShaderParametersBuffer );

            // 各シェーダーステージの 定数バッファ b0 へ登録

            d3ddc.VertexShader.SetConstantBuffer( 0, D3DShaderParametersBuffer );
            d3ddc.HullShader.SetConstantBuffer( 0, D3DShaderParametersBuffer );
            d3ddc.DomainShader.SetConstantBuffer( 0, D3DShaderParametersBuffer );
            d3ddc.GeometryShader.SetConstantBuffer( 0, D3DShaderParametersBuffer );
            d3ddc.PixelShader.SetConstantBuffer( 0, D3DShaderParametersBuffer );
            d3ddc.ComputeShader.SetConstantBuffer( 0, D3DShaderParametersBuffer ); // 不要とは思うが一応。
        }

        public static void AddVertexShader( string name, byte[] shaderBytecode )
        {
            // 同名のシェーダーがあれば解放する。

            if( VertexShaders.ContainsKey( name ) )
            {
                VertexShaders[ name ].shader?.Dispose();
                VertexShaders[ name ].layout?.Dispose();
                VertexShaders.Remove( name );
            }

            // シェーダーと入力レイアウトを作成し、マップに追加する。

            if( _wrD3DDevice.TryGetTarget( out var d3dDevice ) )
            {
                var shader = new SharpDX.Direct3D11.VertexShader( d3dDevice, shaderBytecode );
                var layout = new SharpDX.Direct3D11.InputLayout( d3dDevice, shaderBytecode, VS_INPUT.VertexElements );
                VertexShaders.Add( name, (shader, shaderBytecode, layout) );
            }
        }

        public static void AddHullShader( string name, byte[] shaderBytecode )
        {
            // 同名のシェーダーがあれば解放する。

            if( HullShaders.ContainsKey( name ) )
            {
                HullShaders[ name ].shader?.Dispose();
                HullShaders.Remove( name );
            }

            // シェーダーと入力レイアウトを作成し、マップに追加する。

            if( _wrD3DDevice.TryGetTarget( out var d3dDevice ) )
            {
                var shader = new SharpDX.Direct3D11.HullShader( d3dDevice, shaderBytecode );
                HullShaders.Add( name, (shader, shaderBytecode) );
            }
        }

        public static void AddDomainShader( string name, byte[] shaderBytecode )
        {
            // 同名のシェーダーがあれば解放する。

            if( DomainShaders.ContainsKey( name ) )
            {
                DomainShaders[ name ].shader?.Dispose();
                DomainShaders.Remove( name );
            }

            // シェーダーと入力レイアウトを作成し、マップに追加する。

            if( _wrD3DDevice.TryGetTarget( out var d3dDevice ) )
            {
                var shader = new SharpDX.Direct3D11.DomainShader( d3dDevice, shaderBytecode );
                DomainShaders.Add( name, (shader, shaderBytecode) );
            }
        }

        public static void AddGeometryShader( string name, byte[] shaderBytecode )
        {
            // 同名のシェーダーがあれば解放する。

            if( GeometryShaders.ContainsKey( name ) )
            {
                GeometryShaders[ name ].shader?.Dispose();
                GeometryShaders.Remove( name );
            }

            // シェーダーを作成し、追加する。

            if( _wrD3DDevice.TryGetTarget( out var d3dDevice ) )
            {
                var shader = new SharpDX.Direct3D11.GeometryShader( d3dDevice, shaderBytecode );
                GeometryShaders.Add( name, (shader, shaderBytecode) );
            }
        }

        public static void AddPixelShader( string name, byte[] shaderBytecode )
        {
            // 同名のシェーダーがあれば解放する。

            if( PixelShaders.ContainsKey( name ) )
            {
                PixelShaders[ name ].shader?.Dispose();
                PixelShaders.Remove( name );
            }

            // シェーダーを作成し、追加する。

            if( _wrD3DDevice.TryGetTarget( out var d3dDevice ) )
            {
                var shader = new SharpDX.Direct3D11.PixelShader( d3dDevice, shaderBytecode );
                PixelShaders.Add( name, (shader, shaderBytecode) );
            }
        }

        public static void AddComputeShader( string name, byte[] shaderBytecode )
        {
            // 同名のシェーダーがあれば解放する。

            if( ComputeShaders.ContainsKey( name ) )
            {
                ComputeShaders[ name ].shader?.Dispose();
                ComputeShaders.Remove( name );
            }

            // シェーダーを作成し、追加する。

            if( _wrD3DDevice.TryGetTarget( out var d3dDevice ) )
            {
                var shader = new SharpDX.Direct3D11.ComputeShader( d3dDevice, shaderBytecode );
                ComputeShaders.Add( name, (shader, shaderBytecode) );
            }
        }


        private static WeakReference<SharpDX.Direct3D11.Device> _wrD3DDevice;
    }
}
