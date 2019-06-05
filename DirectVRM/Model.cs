using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class Model : IDisposable
    {
        /// <summary>
        ///     glTF2.0 パート。
        /// </summary>
        public glTF glTF { get; protected set; }

        /// <summary>
        ///     VRM 拡張パート。
        ///     モデルに VRM 拡張がなければ null 。
        /// </summary>
        public VRM VRM { get; protected set; }

        /// <summary>
        ///     シーンの平行移動。
        /// </summary>
        public Vector3 PositionLH { get; set; } = Vector3.Zero;
        
        /// <summary>
        ///     シーンの回転。
        /// </summary>
        public Quaternion RotationLH { get; set; } = Quaternion.Identity;

        /// <summary>
        ///     シーンの拡大率。
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.One;



        // 生成と終了


        public Model( SharpDX.Direct3D11.Device d3dDevice, string glbFilePath )
        {
            // ストリームから glTF(JSON) 部を読み込む。
            using( var fs = new FileStream( glbFilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                var gltfNative = glTFLoader.Interface.LoadModel( fs );
                this.glTF = new glTF( gltfNative );
            }

            // VRM 拡張があれば読み込む。
            if( this.glTF.Extensions.ContainsKey( glTF_VRM.ExtensionName ) )
            {
                var jObject = this.glTF.Extensions[ glTF_VRM.ExtensionName ] as Newtonsoft.Json.Linq.JObject;
                var jsonData = jObject.ToString( Newtonsoft.Json.Formatting.None );
                var vrmNative = Newtonsoft.Json.JsonConvert.DeserializeObject<glTF_VRM>( jsonData );
                this.VRM = new VRM( vrmNative, this.glTF );
            }

            // バイナリバッファは１つだけ対応（公式仕様？）
            byte[] binaryBuffer;
            using( var fs = new FileStream( glbFilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                binaryBuffer = glTFLoader.Interface.LoadBinaryBuffer( fs );
            }

            // 遅延バインディング。
            this.glTF.LateBinding( binaryBuffer, d3dDevice );
            this.VRM?.LateBinding();
        }

        public virtual void Dispose()
        {
            this.VRM?.Dispose();
            this.VRM = null;

            this.glTF?.Dispose();
            this.glTF = null;
        }



        // 進行と描画


        public void Update( double 現在時刻sec )
        {
            if( null != this.VRM )
            {
                this.VRM.Update( 現在時刻sec );
            }
            else
            {
                this.glTF.Update( 現在時刻sec );
            }
        }

        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters )
        {
            // シーンの現在の変形をシェーダーパラメーターに設定する。
            shaderParameters.WorldMatrix = Matrix.Transformation( Vector3.Zero, Quaternion.Identity, this.Scale, Vector3.Zero, this.RotationLH, this.PositionLH );

            if( null != this.VRM )
            {
                this.VRM.Draw( d3ddc, ref shaderParameters );
            }
            else
            {
                this.glTF.Draw( d3ddc, ref shaderParameters );
            }
        }
    }
}
