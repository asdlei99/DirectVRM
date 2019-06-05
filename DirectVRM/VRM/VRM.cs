using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class VRM : IDisposable
    {
        public VRMMeta Meta { get; }

        public VRMMaterialProperty[] MaterialProperties { get; }

        public VRMSecondaryAnimation SecondaryAnimation { get; protected set; }

        public VRMBlendShapeMaster BlendShapeMaster { get; protected set; }

        public VRMFirstPerson FirstPerson { get; protected set; }

        public VRMHumanoid Humanoid { get; protected set; }

        public VRMScene Scene { get; protected set; }



        // 生成と終了


        public VRM( glTF_VRM native, glTF gltf )
        {
            this._Native = native;
            this._glTF = gltf;

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

            #region " Scene "
            //----------------
            this.Scene = new VRMScene( gltf.Scene ?? gltf.Scenes[ 0 ] );
            //----------------
            #endregion
        }

        internal void LateBinding()
        {
            // Meta
            this.Meta?.LateBinding( this._glTF, this._Native );

            // MaterialProperties
            foreach( var mp in this.MaterialProperties )
                mp.LateBinding( this._glTF, this._Native );

            // SecondaryAnimation
            this.SecondaryAnimation?.LateBinding( this._glTF, this._Native );

            // BlendShapeMaster
            this.BlendShapeMaster?.LateBinding( this._glTF, this._Native );

            // FirstPerson
            this.FirstPerson?.LateBinding( this._glTF, this._Native );

            // Humanoid
            this.Humanoid?.LateBinding( this._glTF, this._Native );

            // Scene
            this.Scene.LateBinding();
        }

        public virtual void Dispose()
        {
            // Scene
            this.Scene.Dispose();

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

            this._glTF = null;  // disposeしない

            this._Native = null;
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
            // シーンを更新する。
            this.Scene.Update( 現在時刻sec );

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
            // すべてのモーフターゲットを初期状態にリセットする。
            foreach( var mesh in this._glTF.Meshes )
                mesh.ResetAllWeights();

            // 現在のブレンドシェイプを適用する。
            this.BlendShapeMaster?.Apply();

            // シーンを描画する。
            this.Scene.Draw( d3ddc, ref shaderParameters, this.MaterialProperties );
        }



        // ローカル


        private glTF_VRM _Native;

        private glTF _glTF;
    }
}
