using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class glTFNode : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        /// <summary>
        ///     未定義時は null
        /// </summary>
        public float[] Weights => this._Native.Weights;

        /// <summary>
        ///     未定義時は null
        /// </summary>
        public glTFMesh Mesh { get; protected set; }

        /// <summary>
        ///     未定義時は null
        /// </summary>
        public glTFSkin Skin { get; protected set; }

        /// <summary>
        ///     未定義時は null
        /// </summary>
        public glTFCamera Camera { get; protected set; }

        /// <summary>
        ///     未定義時は空配列（nullではない）
        /// </summary>
        public glTFNode[] Children { get; protected set; }

        /// <summary>
        ///     親ノード。ルートノードである場合は null。
        /// </summary>
        public glTFNode Parent { get; protected set; }

        public Dictionary<string, object> Extensions => this._Native.Extensions ?? new Dictionary<string, object>();

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // ローカル Transform 関連


        /// <summary>
        ///     ローカル位置。
        /// </summary>
        public Vector3 LocalPositionRH { get; set; }

        /// <summary>
        ///     ローカル回転。
        /// </summary>
        public Quaternion LocalRotationRH { get; set; }

        /// <summary>
        ///     ローカル拡大率。
        /// </summary>
        public Vector3 LocalScale { get; set; }

        /// <summary>
        ///     ローカル(TRS)変換行列。
        /// </summary>
        public Matrix LocalTRSMatrixRH
            => Matrix.Transformation( Vector3.Zero, Quaternion.Identity, this.LocalScale, Vector3.Zero, this.LocalRotationRH, this.LocalPositionRH );

        /// <summary>
        ///     ローカル空間からワールド空間への変換行列。
        /// </summary>
        public Matrix LocalToWorldMatrixRH
            => this.LocalTRSMatrixRH * ( this.Parent?.LocalToWorldMatrixRH ?? Matrix.Identity );



        // ワールド Transform 関連


        /// <summary>
        ///     ワールド空間での位置。
        /// </summary>
        /// <remarks>
        ///     ルートから自分までの、すべてのローカル平行移動の和に等しい。
        /// </remarks>
        public Vector3 PositionRH
        {
            get => this.LocalPositionRH + ( this.Parent?.PositionRH ?? Vector3.Zero );
            set => this.LocalPositionRH = value - ( this.Parent?.PositionRH ?? Vector3.Zero );
        }

        /// <summary>
        ///     ワールド空間での回転。
        /// </summary>
        /// <remarks>
        ///     ルートから自分までの、すべてのローカル回転の積に等しい。
        /// </remarks>
        public Quaternion RotationRH
        {
            get => this.LocalRotationRH * ( this.Parent?.RotationRH ?? Quaternion.Identity );
            set => this.LocalRotationRH = value * Quaternion.Conjugate( this.Parent?.RotationRH ?? Quaternion.Identity );
        }

        /// <summary>
        ///     ワールド空間における最終的な拡大率。
        /// </summary>
        public Vector3 LossyScale
            => ( this.Parent?.LossyScale ?? Vector3.One ) * this.LocalScale;

        /// <summary>
        ///     ワールド(TRS)変換行列。
        /// </summary>
        public Matrix WorldTRSMatrixRH
            => Matrix.Transformation( Vector3.Zero, Quaternion.Identity, this.LossyScale, Vector3.Zero, this.RotationRH, this.PositionRH );

        /// <summary>
        ///     ワールド空間からローカル空間への変換行列。
        /// </summary>
        public Matrix WorldToLocalMatrixRH
            => Matrix.Invert( this.LocalToWorldMatrixRH );



        // 生成と終了


        public glTFNode( int objectIndex, glTFLoader.Schema.Node native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            // 先に TRS をチェック
            this.LocalPositionRH = ( null != this._Native.Translation ) ? new Vector3( this._Native.Translation ) : Vector3.Zero;       // 右手系
            this.LocalRotationRH = ( null != this._Native.Rotation ) ? new Quaternion( this._Native.Rotation ) : Quaternion.Identity;   // 右手系
            this.LocalScale = ( null != this._Native.Scale ) ? new Vector3( this._Native.Scale ) : Vector3.One;

            // 次に Matrix をチェック
            if( null != this._Native.Matrix )
            {
                var matrix = new Matrix( this._Native.Matrix );
                matrix.Transpose();    // glTF は列優先なので行優先に戻す

                // Matrix が初期値じゃないならこちらを TRS より優先（上書き）する。
                if( !matrix.IsIdentity )
                {
                    this.LocalPositionRH = matrix.TranslationVector;
                    this.LocalRotationRH = matrix.ExtractRotation();
                    this.LocalScale = matrix.ScaleVector;
                }
                else
                {
                    // Matrix が初期値なら TRS を優先
                }
            }
            else
            {
                // Matrix 未定義時は TRS を優先
            }

            // Children
            this.Children = ( null != this._Native.Children ) ?
                new glTFNode[ this._Native.Children.Length ] :
                new glTFNode[ 0 ];  // 未定義時は空配列（not null）

            this.Parent = null;
            this.Camera = null;
            this.Mesh = null;
            this.Skin = null;
        }

        public void LateBinding( glTF gltf )
        {
            // Mesh
            this.Mesh = ( this._Native.Mesh.HasValue ) ?
                gltf.Meshes[ this._Native.Mesh.Value ] :
                null;

            // Skin
            this.Skin = ( this._Native.Skin.HasValue ) ?
                gltf.Skins[ this._Native.Skin.Value ] :
                null;

            // Children
            for( int i = 0; i < this.Children.Length; i++ )
                this.Children[ i ] = gltf.Nodes[ this._Native.Children[ i ] ];

            // Camera
            this.Camera = ( this._Native.Camera.HasValue ) ?
                gltf.Cameras[ this._Native.Camera.Value ] :
                null;

            // Parent
            foreach( var node in gltf.Nodes )
            {
                if( node.Children.Contains( this ) )
                {
                    this.Parent = node;
                    break;
                }
            }
        }

        public virtual void Dispose()
        {
            this.Mesh = null;       // disposeしない
            this.Skin = null;       // disposeしない
            this.Children = null;   // disposeしない
            this.Camera = null;     // disposeしない
            this.Parent = null;     // disposeしない

            this._Native = null;
        }



        // 進行と描画


        /// <summary>
        ///     自分と子ノードを描画する。
        /// </summary>
        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters, VRMMaterialProperty[] vrmMaterials )
        {
            // ノードにメッシュがあれば描画する。
            this.Mesh?.Draw( d3ddc, ref shaderParameters, this.Skin, vrmMaterials );

            // 子ノードがあれば再帰的に描画する。
            foreach( var child in this.Children )
                child.Draw( d3ddc, ref shaderParameters, vrmMaterials );
        }



        // ローカル


        private glTFLoader.Schema.Node _Native;
    }
}
