using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DirectVRM
{
    public class glTFTexture : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public glTFSampler Sampler { get; protected set; }

        public glTFImage Source { get; protected set; }

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }


        public SharpDX.Direct3D11.Texture2D D3DTexture { get; protected set; }

        public SharpDX.Direct3D11.ShaderResourceView D3DTextureSRV { get; protected set; }



        // 生成と終了


        public glTFTexture( int objectIndex, glTFLoader.Schema.Texture native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            this.Sampler = null;
            this.Source = null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf, SharpDX.Direct3D11.Device d3dDevice )
        {
            // Sampler
            this.Sampler = ( this._Native.Sampler.HasValue ) ?
                gltf.Samplers[ this._Native.Sampler.Value ] :
                null;

            // Source
            this.Source = ( this._Native.Source.HasValue ) ?
                gltf.Images[ this._Native.Source.Value ] :
                null;

            #region " Texture2D とその SRV を作成する。"
            //----------------
            if( null != this.Source )
            {
                using( var image = new System.Drawing.Bitmap( this.Source.BufferView.DataStream ) )
                {
                    var imageRect = new System.Drawing.Rectangle( 0, 0, image.Width, image.Height );
                    using( var bitmap = image.Clone( imageRect, System.Drawing.Imaging.PixelFormat.Format32bppArgb ) )
                    {
                        var locks = bitmap.LockBits( imageRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat );
                        var dataBox = new[] { new SharpDX.DataBox( locks.Scan0, bitmap.Width * 4, bitmap.Height ) };
                        var textureDesc = new SharpDX.Direct3D11.Texture2DDescription() {
                            ArraySize = 1,
                            BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                            CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                            Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                            Height = bitmap.Height,
                            Width = bitmap.Width,
                            MipLevels = 1,
                            OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
                            SampleDescription = new SharpDX.DXGI.SampleDescription( 1, 0 ),
                            Usage = SharpDX.Direct3D11.ResourceUsage.Default
                        };

                        this.D3DTexture = new SharpDX.Direct3D11.Texture2D( d3dDevice, textureDesc, dataBox );
                        bitmap.UnlockBits( locks );

                        this.D3DTextureSRV = new SharpDX.Direct3D11.ShaderResourceView( d3dDevice, this.D3DTexture );
                    }
                }
            }
            //----------------
            #endregion
        }


        public virtual void Dispose()
        {
            this.D3DTextureSRV?.Dispose();
            this.D3DTextureSRV = null;
            this.D3DTexture?.Dispose();
            this.D3DTexture = null;

            this.Sampler = null;    // disposeしない
            this.Source = null;     // disposeしない
            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Texture _Native;
    }
}
