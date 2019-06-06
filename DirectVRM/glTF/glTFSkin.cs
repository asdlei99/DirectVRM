using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFSkin : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public glTFAccessor InverseBindMatricesRH { get; protected set; }

        /// <summary>
        ///     未定義なら null。
        /// </summary>
        public glTFNode Skelton { get; protected set; }

        /// <summary>
        ///     ジョイントのワールド変換行列。
        ///     未定義なら空配列（nullではない）。
        /// </summary>
        public glTFNode[] Joints { get; protected set; }

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }


        /// <summary>
        ///     逆バインド行列の配列。列優先。
        ///     要素数は <see cref="Joints"/> の要素数と同じ。
        /// </summary>
        public SharpDX.Direct3D11.Buffer D3DInvertBindMatricesRHBuffer { get; protected set; }

        public SharpDX.Direct3D11.ShaderResourceView D3DInvertBindMatricesRHBufferSRV { get; protected set; }

        public SharpDX.Direct3D11.Buffer D3DJointWorldMatricesRHBuffer { get; protected set; }

        public SharpDX.Direct3D11.ShaderResourceView D3DJointWorldMatricesRHBufferSRV { get; protected set; }



        // 生成と終了


        public glTFSkin( int objectIndex, glTFLoader.Schema.Skin native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            // Joints
            this.Joints = new glTFNode[ this._Native.Joints?.Length ?? 0 ]; // 未定義時は空配列（not null）

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();

            // 以下 late binding
            this.InverseBindMatricesRH = null;
            this.Skelton = null;
            this.D3DInvertBindMatricesRHBuffer = null;
            this.D3DInvertBindMatricesRHBufferSRV = null;
            this.D3DJointWorldMatricesRHBuffer = null;
            this.D3DJointWorldMatricesRHBufferSRV = null;
        }

        public void LateBinding( glTF gltf, SharpDX.Direct3D11.Device d3dDevice )
        {
            // InverseBindMatrix
            this.InverseBindMatricesRH = ( this._Native.InverseBindMatrices.HasValue ) ?
                gltf.Accessors[ this._Native.InverseBindMatrices.Value ] :
                null;

            // Skelton
            this.Skelton = ( this._Native.Skeleton.HasValue ) ?
                gltf.Nodes[ this._Native.Skeleton.Value ] :
                null;

            // Joints
            for( int i = 0; i < this.Joints.Length; i++ )
                this.Joints[ i ] = gltf.Nodes[ this._Native.Joints[ i ] ];


            #region " InverseBindMatrix から構造化バッファを作成する。"
            //----------------
            if( null != this.InverseBindMatricesRH )
            {
                var bufferView = this.InverseBindMatricesRH.BufferView;

                if( null != bufferView )
                {
                    int 要素サイズ = sizeof( float ) * 16;   // sizoef(Matrix)
                    int 要素数 = bufferView.ByteLength / 要素サイズ;

                    // 逆バインド行列の構造化バッファを作成。
                    bufferView.DataStream.Position = 0;
                    this.D3DInvertBindMatricesRHBuffer = new SharpDX.Direct3D11.Buffer(
                        d3dDevice,
                        bufferView.DataStream,
                        new SharpDX.Direct3D11.BufferDescription {
                            BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                            CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                            OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,  // 構造化バッファ
                            SizeInBytes = bufferView.ByteLength,
                            StructureByteStride = 要素サイズ,
                            Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                        } );

                    // シェーダーリソースビューを作成。
                    this.D3DInvertBindMatricesRHBufferSRV = new SharpDX.Direct3D11.ShaderResourceView(
                        d3dDevice,
                        this.D3DInvertBindMatricesRHBuffer,
                        new SharpDX.Direct3D11.ShaderResourceViewDescription {
                            Format = SharpDX.DXGI.Format.Unknown,
                            Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.ExtendedBuffer,
                            BufferEx = new SharpDX.Direct3D11.ShaderResourceViewDescription.ExtendedBufferResource {
                                FirstElement = 0,
                                ElementCount = 要素数,
                            },
                        } );
                }
                else
                {
                    // bufferView が未定義
                }
            }
            else
            {
                // InverseBindMatrices アクセサが未設定
            }
            //----------------
            #endregion

            #region " Joints[] から構造化バッファを作成する。"
            //----------------
            if( 0 < this.Joints.Length )
            {
                int 要素サイズ = sizeof( float ) * 16;   // sizoef(Matrix)

                this.D3DJointWorldMatricesRHBuffer = new SharpDX.Direct3D11.Buffer(
                    d3dDevice,
                    new SharpDX.Direct3D11.BufferDescription {
                        BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                        CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                        OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,  // 構造化バッファ
                        SizeInBytes = this.Joints.Length * 要素サイズ,
                        StructureByteStride = 要素サイズ,
                        Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                    } );

                this.D3DJointWorldMatricesRHBufferSRV = new SharpDX.Direct3D11.ShaderResourceView(
                    d3dDevice,
                    this.D3DJointWorldMatricesRHBuffer,
                    new SharpDX.Direct3D11.ShaderResourceViewDescription {
                        Format = SharpDX.DXGI.Format.Unknown,
                        Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.ExtendedBuffer,
                        BufferEx = new SharpDX.Direct3D11.ShaderResourceViewDescription.ExtendedBufferResource {
                            FirstElement = 0,
                            ElementCount = this.Joints.Length,
                        },
                    } );
            }
            //----------------
            #endregion
        }

        public virtual void Dispose()
        {
            this.D3DJointWorldMatricesRHBufferSRV?.Dispose();
            this.D3DJointWorldMatricesRHBufferSRV = null;
            this.D3DJointWorldMatricesRHBuffer?.Dispose();
            this.D3DJointWorldMatricesRHBuffer = null;

            this.D3DInvertBindMatricesRHBufferSRV?.Dispose();
            this.D3DInvertBindMatricesRHBufferSRV = null;
            this.D3DInvertBindMatricesRHBuffer?.Dispose();
            this.D3DInvertBindMatricesRHBuffer = null;

            this.InverseBindMatricesRH = null;    // disposeしない
            this.Skelton = null;                // disposeしない
            this.Joints = null;                 // disposeしない

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Skin _Native;
    }
}
