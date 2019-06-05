using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;

namespace Sample
{
    public partial class Form1 : SharpDX.Windows.RenderForm
    {
        public Form1()
        {
            InitializeComponent();

            this.Text = "DirectVRM Sample";
            this.ClientSize = new Size( 1280, 1280 * 9 / 16 );
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            // 起動起動が落ち着いたらメインループへ
            Application.Idle += this.Application_Idle;
        }

        private void Application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= this.Application_Idle;


            // 初期化する。

            this._InitializeDirect3D();
            this._InitializeVRM();


            // VRMシーン（モデル）を生成する。

            string exePath = Path.GetDirectoryName( Application.ExecutablePath );
            this._Model = new DirectVRM.Model( this._D3DDevice, Path.Combine( exePath, @"AliciaVRM\AliciaSolid.vrm" ) );

            // 初期状態では +Z 方向を向いているので、Y軸に対して180度反転しておく。
            this._Model.RotationLH = Quaternion.RotationAxis( Vector3.Up, MathUtil.Pi );

            // ブレンドシェイプを指定する。指定しなければ "neutral"。
            if( null != this._Model.VRM?.BlendShapeMaster )
                this._Model.VRM.BlendShapeMaster.CurrentGroupName = "fun";


            // メインループ

            var timer = Stopwatch.StartNew();

            SharpDX.Windows.RenderLoop.Run( this, () => {

                double NowSec = timer.ElapsedMilliseconds / 1000.0; // seconds

                // VRMシーンを更新する。
                this._Model.Update( NowSec );

                // VRMシーンを描画する。
                this._D3DDevice.ImmediateContext.Rasterizer.SetViewport( this._DefaultViewport );
                this._D3DDevice.ImmediateContext.OutputMerger.DepthStencilState = this._DefaultDepthStencilState;
                this._D3DDevice.ImmediateContext.OutputMerger.BlendState = this._DefaultBlendState;
                this._D3DDevice.ImmediateContext.ClearRenderTargetView( this._DefaultRenderTargetView, new Color4( 0.2f, 0.4f, 0.8f, 1.0f ) );
                this._D3DDevice.ImmediateContext.ClearDepthStencilView( this._DefaultDepthStencilView, SharpDX.Direct3D11.DepthStencilClearFlags.Depth | SharpDX.Direct3D11.DepthStencilClearFlags.Stencil, 1.0f, 0 );
                this._D3DDevice.ImmediateContext.OutputMerger.SetTargets( this._DefaultDepthStencilView, this._DefaultRenderTargetView );
                this._Model.Draw( this._D3DDevice.ImmediateContext, ref _ShaderParameters );

                // スワップチェーンを表示する。
                this._SwapChain.Present( 1, SharpDX.DXGI.PresentFlags.None );

            } );


            // アプリを終了する。

            this._Model?.Dispose();
            this._ReleaseVRM();
            this._ReleaseDirect3D();
        }


        
        // VRM


        private DirectVRM.ShaderParameters _ShaderParameters;
        private DirectVRM.Model _Model;


        private void _InitializeVRM()
        {
            // ビュー変換／射影変換行列用のパラメータ
            var cameraPosition = new Vector3( 0f, 1.3f, -4.0f );
            var cameraForward = new Vector3( 0f, 1.0f, 0f );
            var cameraUp = new Vector3( 0f, 1f, 0f );
            float fov = 30f;    // degree
            float nearZ = 0.25f;
            float farZ = 2000f;
            float aspect = 16f / 9f;// 1.618f;

            #region " シェーダーパラメータを初期化する。"
            //----------------
            this._ShaderParameters = new DirectVRM.ShaderParameters();
            this._ShaderParameters.TessellationFactor = 5f;
            this._ShaderParameters.WorldMatrix = Matrix.Identity;
            this._ShaderParameters.ViewMatrix = Matrix.LookAtLH( cameraPosition, cameraForward, cameraUp );
            this._ShaderParameters.ProjectionMatrix = Matrix.PerspectiveFovLH( MathUtil.Pi * fov / 180.0f, aspect, nearZ, farZ );
            //----------------
            #endregion

            // シェーダーを生成する。
            DirectVRM.D3DShaders.Initialize( this._D3DDevice );
        }

        private void _ReleaseVRM()
        {
            // シェーダーを解放する。
            DirectVRM.D3DShaders.Release();
        }



        // Direct3D


        private SharpDX.Direct3D11.Device _D3DDevice;
        private SharpDX.DXGI.SwapChain _SwapChain;

        private SharpDX.Direct3D11.Texture2D _DefaultRenderTarget;
        private SharpDX.Direct3D11.Texture2D _DefaultDepthStencil;
        private SharpDX.Direct3D11.RenderTargetView _DefaultRenderTargetView;
        private SharpDX.Direct3D11.DepthStencilView _DefaultDepthStencilView;

        private SharpDX.Direct3D11.DepthStencilState _DefaultDepthStencilState;
        private SharpDX.Direct3D11.BlendState _DefaultBlendState;
        private SharpDX.ViewportF _DefaultViewport;


        private void _InitializeDirect3D()
        {
#if DEBUG
            SharpDX.Configuration.EnableReleaseOnFinalizer = true;          // ファイナライザの実行中、未解放のCOMを見つけたら解放を試みる。
            SharpDX.Configuration.EnableTrackingReleaseOnFinalizer = true;  // その際には Trace にメッセージを出力する。
#endif
            #region " D3Dデバイスとスワップチェーンを作成する。"
            //----------------
            SharpDX.Direct3D11.Device.CreateWithSwapChain(
                SharpDX.Direct3D.DriverType.Hardware,
                SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport,
                new SharpDX.Direct3D.FeatureLevel[] { SharpDX.Direct3D.FeatureLevel.Level_11_0 },
                new SharpDX.DXGI.SwapChainDescription {
                    BufferCount = 1,
                    Flags = SharpDX.DXGI.SwapChainFlags.AllowModeSwitch,
                    IsWindowed = true,
                    ModeDescription = new SharpDX.DXGI.ModeDescription {
                        Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                        Width = this.ClientSize.Width,
                        Height = this.ClientSize.Height,
                        Scaling = SharpDX.DXGI.DisplayModeScaling.Stretched,
                    },
                    OutputHandle = this.Handle,
                    SampleDescription = new SharpDX.DXGI.SampleDescription( 4, 0 ), // MSAA x4
                    SwapEffect = SharpDX.DXGI.SwapEffect.Discard,
                    Usage = SharpDX.DXGI.Usage.RenderTargetOutput,
                },
                out this._D3DDevice,
                out this._SwapChain );
            //----------------
            #endregion

            #region " 既定のレンダーターゲットと既定の深度ステンシルを作成する。"
            //----------------
            this._DefaultRenderTarget = this._SwapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D>( 0 );
            this._DefaultDepthStencil = new SharpDX.Direct3D11.Texture2D(
                this._D3DDevice,
                new SharpDX.Direct3D11.Texture2DDescription {
                    Width = this._DefaultRenderTarget.Description.Width,
                    Height = this._DefaultRenderTarget.Description.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                    SampleDescription = this._DefaultRenderTarget.Description.SampleDescription,
                    Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                    BindFlags = SharpDX.Direct3D11.BindFlags.DepthStencil,
                    CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                    OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
                } );
            this._DefaultRenderTargetView = new SharpDX.Direct3D11.RenderTargetView( this._D3DDevice, this._DefaultRenderTarget );
            this._DefaultDepthStencilView = new SharpDX.Direct3D11.DepthStencilView( this._D3DDevice, this._DefaultDepthStencil );
            //----------------
            #endregion

            #region " 既定の深度ステンシルステートを作成する。"
            //----------------
            this._DefaultDepthStencilState = null;
            //----------------
            #endregion

            #region " 既定のブレンドステート（通常合成）を作成する。"
            //----------------
            var blendStateNorm = new SharpDX.Direct3D11.BlendStateDescription() {
                AlphaToCoverageEnable = false,  // アルファマスクで透過する（するならZバッファ必須）
                IndependentBlendEnable = false, // 個別設定。false なら BendStateDescription.RenderTarget[0] だけが有効で、[1～7] は無視される。
            };
            blendStateNorm.RenderTarget[ 0 ].IsBlendEnabled = true; // true ならブレンディングが有効。
            blendStateNorm.RenderTarget[ 0 ].RenderTargetWriteMask = SharpDX.Direct3D11.ColorWriteMaskFlags.All;

            blendStateNorm.RenderTarget[ 0 ].SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha;                  // 色値のブレンディング
            blendStateNorm.RenderTarget[ 0 ].DestinationBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha;
            blendStateNorm.RenderTarget[ 0 ].BlendOperation = SharpDX.Direct3D11.BlendOperation.Add;

            blendStateNorm.RenderTarget[ 0 ].SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.One;                     // アルファ値のブレンディング
            blendStateNorm.RenderTarget[ 0 ].DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.Zero;
            blendStateNorm.RenderTarget[ 0 ].AlphaBlendOperation = SharpDX.Direct3D11.BlendOperation.Add;

            this._DefaultBlendState = new SharpDX.Direct3D11.BlendState( this._D3DDevice, blendStateNorm );
            //----------------
            #endregion

            #region " 既定のビューポートを作成する。"
            //----------------
            this._DefaultViewport = new ViewportF( 0f, 0f, this.ClientSize.Width, this.ClientSize.Height, 0f, 1f );
            //----------------
            #endregion
        }

        private void _ReleaseDirect3D()
        {
            this._DefaultBlendState?.Dispose();
            this._DefaultDepthStencilState?.Dispose();
            this._DefaultRenderTargetView?.Dispose();
            this._DefaultDepthStencilView?.Dispose();
            this._DefaultRenderTarget?.Dispose();
            this._DefaultDepthStencil?.Dispose();
            this._SwapChain?.Dispose();
            this._D3DDevice?.Dispose();
#if DEBUG
            Console.WriteLine( "ReportActiveObjects:" );
            Console.WriteLine( SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects() );
#endif
        }
    }
}
