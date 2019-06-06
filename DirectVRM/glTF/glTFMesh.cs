using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFMesh : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public glTFMeshPrimitive[] Primitives { get; }

        public float[] Weights { get; protected set; }

        /// <summary>
        ///     このメッシュで使用するモーフターゲット。
        ///     見つからない場合は null 。
        /// </summary>
        /// <remarks>
        ///     このメッシュの<see cref="Primitives"/>のうち、<see cref="glTFMeshPrimitive.Targets"/>が非nullの
        ///     ものを見つけ出してこれに格納する。
        /// </remarks>
        public Dictionary<string, glTFAccessor>[] Targets
        {
            get
            {
                foreach( var mp in this.Primitives )
                {
                    if( 0 < mp.Targets.Length )
                    {
                        return mp.Targets;  //非nullである最初のものを採用。
                    }
                }

                return null;
            }
        }

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTFMesh( glTFLoader.Schema.Mesh native )
        {
            this._Native = native;

            #region " Primitive "
            //----------------
            if( null != this._Native.Primitives )
            {
                this.Primitives = new glTFMeshPrimitive[ this._Native.Primitives.Length ];

                for( int i = 0; i < this._Native.Primitives.Length; i++ )
                {
                    this.Primitives[ i ] = new glTFMeshPrimitive( this._Native.Primitives[ i ] );
                }
            }
            else
            {
                this.Primitives = new glTFMeshPrimitive[ 0 ];   // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Weights "
            //----------------
            if( 0 < this.Primitives.Length )
            {
                // すべてのプリミティブでTargetsの要素数は同じであること
                int targetNum = this.Primitives[ 0 ].Targets?.Length ?? 0;   // 代表して Primitive[0]
                if( 0 < targetNum )
                {
                    this.Weights = new float[ targetNum ];
                    this._OriginalWeights = new float[ targetNum ];

                    if( 0 < ( this._Native.Weights?.Length ?? 0 ) )
                    {
                        // (A) Mesh に weights 配列の指定があるので取得
                        for( int i = 0; i < this.Weights.Length; i++ )
                        {
                            this.Weights[ i ] = this._Native.Weights[ i ];
                            this._OriginalWeights[ i ] = this._Native.Weights[ i ];
                        }
                    }
                    else
                    {
                        // (B) Mesh に weights の指定がないのでゼロクリア
                        Array.Clear( this.Weights, 0, this.Weights.Length );
                        Array.Clear( this._OriginalWeights, 0, this.Weights.Length );
                    }
                }
                else
                {
                    this.Weights = new float[ 0 ];
                    this._OriginalWeights = new float[ 0 ];
                }
            }
            //----------------
            #endregion

            #region " Extensions "
            //----------------
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf, SharpDX.Direct3D11.Device d3dDevice )
        {
            // Primitives
            foreach( var mp in this.Primitives )
                mp.LateBinding( gltf, d3dDevice );
        }

        public virtual void Dispose()
        {
            // Primitives
            foreach( var mp in this.Primitives )
                mp?.Dispose();

            this._Native = null;
        }



        // 進行と描画

        public void ResetAllWeights()
        {
            for( int i = 0; i < this._OriginalWeights.Length; i++ )
                this.Weights[ i ] = this._OriginalWeights[ i ];
        }

        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters, glTFSkin skin )
        {
            // すべてのプリミティブを描画する。
            foreach( var primitive in this.Primitives )
                primitive.Draw( d3ddc, ref shaderParameters, skin, this.Weights, this.Targets );
        }



        // ローカル


        private glTFLoader.Schema.Mesh _Native;

        private float[] _OriginalWeights;
    }
}
