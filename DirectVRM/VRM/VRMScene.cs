using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class VRMScene : IDisposable
    {

        // glTF パート


        public glTF glTF { get; protected set; }



        // VRM パート


        public VRMMeta Meta { get; }

        public VRMMaterialProperty[] MaterialProperties { get; }

        public VRMSecondaryAnimation SecondaryAnimation { get; protected set; }

        public VRMBlendShapeMaster BlendShapeMaster { get; protected set; }

        public VRMFirstPerson FirstPerson { get; protected set; }

        public VRMHumanoid Humanoid { get; protected set; }

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


        public VRMScene( SharpDX.Direct3D11.Device d3dDevice, string vrmFilePath )
        {
            this._Native = null;

            // モデルの読み込み
            this.LoadModel( d3dDevice, vrmFilePath );

            #region " Meta "
            //----------------
            this.Meta = ( null != this._Native?.Meta ) ?
                new VRMMeta( this._Native.Meta ) :
                null;
            //----------------
            #endregion

            #region " MaterialProperties "
            //----------------
            if( null != this._Native?.MaterialProperties )
            {
                this.MaterialProperties = new VRMMaterialProperty[ this._Native.MaterialProperties.Length ];

                for( int i = 0; i < this._Native.MaterialProperties.Length; i++ )
                {
                    this.MaterialProperties[ i ] = new VRMMaterialProperty( this._Native.MaterialProperties[ i ] );
                }
            }
            else
            {
                this.MaterialProperties = new VRMMaterialProperty[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " SecondaryAnimation "
            //----------------
            this.SecondaryAnimation = ( null != this._Native?.SecondaryAnimation ) ?
                new VRMSecondaryAnimation( this._Native.SecondaryAnimation ) :
                null;
            //----------------
            #endregion

            #region " BlendShapeMaster "
            //----------------
            this.BlendShapeMaster = ( null != this._Native?.BlendShapeMaster ) ?
                new VRMBlendShapeMaster( this._Native.BlendShapeMaster ) :
                null;
            //----------------
            #endregion

            #region " FirstPerson "
            //----------------
            this.FirstPerson = ( null != this._Native?.FirstPerson ) ?
                new VRMFirstPerson( this._Native.FirstPerson ) :
                null;
            //----------------
            #endregion

            #region " Humanoid "
            //----------------
            this.Humanoid = ( null != this._Native?.Humanoid ) ?
                new VRMHumanoid( this._Native.Humanoid ) :
                null;
            //----------------
            #endregion

            // 参照等の遅延バインディング
            this.VRMLateBinding();
        }

        public void LoadModel( SharpDX.Direct3D11.Device d3dDevice, string vrmFilePath )
        {
            // Gltf(JSON)
            using( var fs = new FileStream( vrmFilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                // ストリームから glTF(JSON) 部を読み込む。
                var gltfNative = glTFLoader.Interface.LoadModel( fs );
                this.glTF = new glTF( gltfNative );

                // VRM 拡張があれば読み込む。
                if( this.glTF.Extensions.ContainsKey( glTF_VRM.ExtensionName ) )
                {
                    var jObject = this.glTF.Extensions[ glTF_VRM.ExtensionName ] as Newtonsoft.Json.Linq.JObject;
                    var jsonData = jObject.ToString( Newtonsoft.Json.Formatting.None );
                    this._Native = Newtonsoft.Json.JsonConvert.DeserializeObject<glTF_VRM>( jsonData );
                }
            }

            // バイナリバッファは
            byte[] binaryBuffer;
            using( var fs = new FileStream( vrmFilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            { 
                binaryBuffer = glTFLoader.Interface.LoadBinaryBuffer( fs ); // １つだけに対応（公式仕様？）
            }

            // 遅延バインディング
            this.glTF.LateBinding( binaryBuffer, d3dDevice );
        }

        private void VRMLateBinding()
        {
            // Meta
            this.Meta?.LateBinding( this.glTF, this._Native );

            // MaterialProperties
            foreach( var mp in this.MaterialProperties )
                mp.LateBinding( this.glTF, this._Native );

            // SecondaryAnimation
            this.SecondaryAnimation?.LateBinding( this.glTF, this._Native );

            // BlendShapeMaster
            this.BlendShapeMaster?.LateBinding( this.glTF, this._Native );

            // FirstPerson
            this.FirstPerson?.LateBinding( this.glTF, this._Native );

            // Humanoid
            this.Humanoid?.LateBinding( this.glTF, this._Native );
        }

        public virtual void Dispose()
        {
            // Humanoid
            this.Humanoid?.Dispose();

            // FirstPerson
            this.FirstPerson?.Dispose();

            // BlendShapeMaster
            this.BlendShapeMaster?.Dispose();

            // SecondaryAnimation
            this.SecondaryAnimation?.Dispose();

            // MaterialProperties
            foreach( var mp in this.MaterialProperties )
                mp.Dispose();

            // Meta
            this.Meta?.Dispose();

            this._Native = null;

            this.glTF?.Dispose();
            this.glTF = null;
        }



        // 進行と描画


        /// <summary>
        ///     モデルを進行する。
        /// </summary>
        /// <remarks>
        ///     ノード／ジョイントのワールド変換、揺れ骨、衝突判定など。
        /// </remarks>
        public void Update( double 現在時刻sec )
        {
            var scene = this.glTF.Scene ?? this.glTF.Scenes[ 0 ];

            // シーンの各ルートノードから順番に進行する。
            foreach( var rootNode in scene.Nodes )
                rootNode.Update();

            // SecondaryAnimation（揺れボーン等）を適用する。
            this.SecondaryAnimation.Update( 現在時刻sec );
        }


        /// <summary>
        ///     モデルを描画する。
        /// </summary>
        /// <remarks>
        ///     ブレンドシェイプ（モーフ）、描画など。
        /// </remarks>
        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters )
        {
            var scene = this.glTF.Scene ?? this.glTF.Scenes[ 0 ];

            // すべてのモーフターゲットを初期状態にリセットする。
            foreach( var mesh in this.glTF.Meshes )
                mesh.ResetAllWeights();

            // 現在のブレンドシェイプを適用する。
            this.BlendShapeMaster?.Apply();

            // シーンの各ルートノードから順番に描画する。
            shaderParameters.WorldMatrix = Matrix.Transformation( Vector3.Zero, Quaternion.Identity, this.Scale, Vector3.Zero, this.RotationLH, this.PositionLH );
            shaderParameters.WorldMatrix.Transpose();   // HLSLは列優先

            foreach( var rootNode in scene.Nodes )
                rootNode.Draw( d3ddc, ref shaderParameters, this.MaterialProperties );
        }



        // ローカル


        private glTF_VRM _Native;
    }
}
