using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMMeshPrimitive : IDisposable
    {

        // 生成と終了


        public VRMMeshPrimitive( glTFMeshPrimitive gltfMeshPrimitive )
        {
            this._glTFMeshPrimitive = gltfMeshPrimitive;
        }

        public virtual void Dispose()
        {
            this._glTFMeshPrimitive = null; // disposeしない
        }



        // 進行と描画


        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters, glTFSkin skin, float[] weights, Dictionary<string, glTFAccessor>[] targets, VRMMaterialProperty[] vrmMaterials )
        {
            if( 0 == this._glTFMeshPrimitive.VertexNum ||
                !this._glTFMeshPrimitive.D3DSkinningBufferMap.TryGetValue( "POSITION", out var positionBuffer ) ||
                !this._glTFMeshPrimitive.D3DSkinningBufferMap.TryGetValue( "NORMAL", out var normalBuffer ) ||
                !this._glTFMeshPrimitive.D3DSkinningBufferMap.TryGetValue( "TEXCOORD_0", out var texcoord0Buffer ) )
            {
                return; // この3つの属性がないものはサポートしない
            }

            #region " シェーダーパラメータ定数バッファを更新し、各シェーダーステージの b0 スロットに設定する。"
            //----------------
            D3DShaders.UpdateGrobalParametersBuffer( d3ddc, ref shaderParameters );
            //----------------
            #endregion

            #region " モーフィングする。"
            //----------------
            if( null != targets )
            {
                #region " POSITION モーフ "
                //----------------
                if( this._glTFMeshPrimitive.Attributes.TryGetValue( "POSITION", out var positionAccessor ) )
                {
                    // モーフ適用前のデータを初期値として取得する。
                    positionAccessor.BufferView.DataStream.Position = 0;
                    float[] モーフ後データ = positionAccessor.BufferView.DataStream.ReadRange<float>( positionAccessor.Count * positionAccessor.TypeSize );

                    // すべてのモーフターゲットについて、モーフを適用する。
                    for( int t = 0; t < targets.Length; t++ )
                    {
                        if( 0f != weights[ t ] && targets[ t ].TryGetValue( "POSITION", out var targetAccessor ) )
                        {
                            targetAccessor.BufferView.DataStream.Position = 0;

                            for( int d = 0; d < targetAccessor.Count; d++ )
                            {
                                float dm = weights[ t ] * targetAccessor.BufferView.DataStream.Read<float>();  // x
                                モーフ後データ[ d * 3 + 0 ] = Math.Max( positionAccessor.Min[ 0 ], Math.Min( positionAccessor.Max[ 0 ], モーフ後データ[ d * 3 + 0 ] + dm ) );

                                dm = weights[ t ] * targetAccessor.BufferView.DataStream.Read<float>();  // y
                                モーフ後データ[ d * 3 + 1 ] = Math.Max( positionAccessor.Min[ 1 ], Math.Min( positionAccessor.Max[ 1 ], モーフ後データ[ d * 3 + 1 ] + dm ) );

                                dm = weights[ t ] * targetAccessor.BufferView.DataStream.Read<float>();  // z
                                モーフ後データ[ d * 3 + 2 ] = Math.Max( positionAccessor.Min[ 2 ], Math.Min( positionAccessor.Max[ 2 ], モーフ後データ[ d * 3 + 2 ] + dm ) );
                            }
                        }
                    }

                    // モーフ適用後の位置データを構造化バッファに書き込む。
                    d3ddc.UpdateSubresource( モーフ後データ, positionBuffer.buffer );
                }
                //----------------
                #endregion

                #region " NORMAL モーフ "
                //----------------
                if( this._glTFMeshPrimitive.Attributes.TryGetValue( "NORMAL", out var normalAccessor ) )
                {
                    // モーフ適用前のデータを初期値として取得する。
                    normalAccessor.BufferView.DataStream.Position = 0;
                    float[] モーフ後データ = normalAccessor.BufferView.DataStream.ReadRange<float>( normalAccessor.Count * normalAccessor.TypeSize );

                    // すべてのモーフターゲットについて、モーフを適用する。
                    for( int t = 0; t < targets.Length; t++ )
                    {
                        if( 0f != weights[ t ] && targets[ t ].TryGetValue( "NORMAL", out var targetAccessor ) )
                        {
                            targetAccessor.BufferView.DataStream.Position = 0;

                            for( int d = 0; d < targetAccessor.Count; d++ )
                            {
                                モーフ後データ[ d * 3 + 0 ] += weights[ t ] * targetAccessor.BufferView.DataStream.Read<float>();  // x
                                モーフ後データ[ d * 3 + 1 ] += weights[ t ] * targetAccessor.BufferView.DataStream.Read<float>();  // y
                                モーフ後データ[ d * 3 + 2 ] += weights[ t ] * targetAccessor.BufferView.DataStream.Read<float>();  // z
                            }
                        }
                    }

                    // モーフ適用後の法線データを構造化バッファに書き込む。
                    d3ddc.UpdateSubresource( モーフ後データ, normalBuffer.buffer );
                }
                //----------------
                #endregion

                // todo: TANGENT モーフの実装
            }
            //----------------
            #endregion

            #region " スキニングする。"
            //----------------
            if( null != skin &&
                this._glTFMeshPrimitive.D3DSkinningBufferMap.TryGetValue( "WEIGHTS_0", out var weights0Buffer ) &&
                this._glTFMeshPrimitive.D3DSkinningBufferMap.TryGetValue( "JOINTS_0", out var joints0Buffer ) )
            {
                bool コンピュートシェーダーを使う = true;

                if( コンピュートシェーダーを使う )
                {
                    #region " コンピュートシェーダーによるスキニング "
                    //----------------
                    // スキニング用コンピュートシェーダーを取得する。
                    if( D3DShaders.ComputeShaders.TryGetValue( "Standard/Skinning", out var skinning ) )
                    {
                        d3ddc.ComputeShader.Set( skinning.shader );


                        // ジョイントのワールド変換行列の構造化バッファを更新する。

                        var jointsWorldMatrix = new SharpDX.Matrix[ skin.Joints.Length ];

                        for( int i = 0; i < jointsWorldMatrix.Length; i++ )
                        {
                            jointsWorldMatrix[ i ] = skin.Joints[ i ].LocalToWorldMatrixRH;
                            jointsWorldMatrix[ i ].Transpose();
                        }

                        d3ddc.UpdateSubresource( jointsWorldMatrix, skin.D3DJointWorldMatricesRHBuffer );


                        // コンピュートシェーダーの入力（構造化バッファ）を設定する。

                        var srvs = new[] {
                            positionBuffer.srv,                     // t0   float3[], 右手系
                            normalBuffer.srv,                       // t1   float3[], 右手系
                            texcoord0Buffer.srv,                    // t2   float2[]
                            weights0Buffer.srv,                     // t3   float4[]
                            joints0Buffer.srv,                      // t4   ushort4[] (ushort=UInt16)
                            skin.D3DInvertBindMatricesRHBufferSRV,  // t5   Matrix[], 右手系
                            skin.D3DJointWorldMatricesRHBufferSRV,  // t6   Matrix[], 右手系
                        };
                        d3ddc.ComputeShader.SetShaderResources( 0, srvs );

                        // コンピュートシェーダーの出力（頂点バッファ）を設定する。
                        d3ddc.ComputeShader.SetUnorderedAccessView( 0, this._glTFMeshPrimitive.D3DVertexBufferUAV );

                        // コンピュートシェーダーを実行してスキニングを行い、結果を頂点バッファに格納する。
                        d3ddc.Dispatch( ( this._glTFMeshPrimitive.VertexNum / 256 ) + 1, 1, 1 ); // 既定のシェーダー（StandardSkinningCS.hlsl）に合わせてある

                        // UAVを外す（このあと頂点シェーダーが使えるように）
                        d3ddc.ComputeShader.SetUnorderedAccessView( 0, null );
                    }
                    else
                    {
                        // Standard/Skinning コンピュートシェーダーが存在しない。
                    }
                    //----------------
                    #endregion
                }
                else
                {
                    #region " CPUによるスキニング（※モーフィングは反映されない）"
                    //----------------
                    if( this._glTFMeshPrimitive.Attributes.TryGetValue( "POSITION", out var positionAccessor ) &&
                        this._glTFMeshPrimitive.Attributes.TryGetValue( "NORMAL", out var normalAccessor ) &&
                        this._glTFMeshPrimitive.Attributes.TryGetValue( "TEXCOORD_0", out var texcoord0Accessor ) &&
                        this._glTFMeshPrimitive.Attributes.TryGetValue( "WEIGHTS_0", out var weights0Accessor ) &&
                        this._glTFMeshPrimitive.Attributes.TryGetValue( "JOINTS_0", out var joints0Accessor ) )
                    {
                        var スキニング後の入力頂点リスト = new VS_INPUT[ this._glTFMeshPrimitive.VertexNum ];

                        positionAccessor.BufferView.DataStream.Position = 0;
                        normalAccessor.BufferView.DataStream.Position = 0;
                        texcoord0Accessor.BufferView.DataStream.Position = 0;
                        weights0Accessor.BufferView.DataStream.Position = 0;
                        joints0Accessor.BufferView.DataStream.Position = 0;

                        var jointsWorldMatrix = new SharpDX.Matrix[ skin.Joints.Length ];
                        var invBindMatrix = new SharpDX.Matrix[ skin.Joints.Length ];
                        skin.InverseBindMatricesRH.BufferView.DataStream.Position = 0;
                        for( int i = 0; i < jointsWorldMatrix.Length; i++ )
                        {
                            jointsWorldMatrix[ i ] = skin.Joints[ i ].LocalToWorldMatrixRH;

                            for( int r = 0; r < 4; r++ )
                                for( int c = 0; c < 4; c++ )    // 列優先
                                    invBindMatrix[ i ][ r, c ] = skin.InverseBindMatricesRH.BufferView.DataStream.Read<float>();
                        }

                        for( int i = 0; i < this._glTFMeshPrimitive.VertexNum; i++ )
                        {
                            SharpDX.Vector3 position;
                            position.X = positionAccessor.BufferView.DataStream.Read<float>();  // 右手系
                            position.Y = positionAccessor.BufferView.DataStream.Read<float>();
                            position.Z = positionAccessor.BufferView.DataStream.Read<float>();

                            SharpDX.Vector3 normal;
                            normal.X = normalAccessor.BufferView.DataStream.Read<float>();  // 右手系
                            normal.Y = normalAccessor.BufferView.DataStream.Read<float>();
                            normal.Z = normalAccessor.BufferView.DataStream.Read<float>();

                            SharpDX.Vector2 texcoord0;
                            texcoord0.X = texcoord0Accessor.BufferView.DataStream.Read<float>();
                            texcoord0.Y = texcoord0Accessor.BufferView.DataStream.Read<float>();

                            float[] weights0 = new float[ 4 ];
                            weights0[ 0 ] = weights0Accessor.BufferView.DataStream.Read<float>();
                            weights0[ 1 ] = weights0Accessor.BufferView.DataStream.Read<float>();
                            weights0[ 2 ] = weights0Accessor.BufferView.DataStream.Read<float>();
                            weights0[ 3 ] = weights0Accessor.BufferView.DataStream.Read<float>();

                            ushort[] joints0 = new ushort[ 4 ];
                            joints0[ 0 ] = joints0Accessor.BufferView.DataStream.Read<ushort>();
                            joints0[ 1 ] = joints0Accessor.BufferView.DataStream.Read<ushort>();
                            joints0[ 2 ] = joints0Accessor.BufferView.DataStream.Read<ushort>();
                            joints0[ 3 ] = joints0Accessor.BufferView.DataStream.Read<ushort>();

                            var skeltonmat =
                                invBindMatrix[ joints0[ 0 ] ] * jointsWorldMatrix[ joints0[ 0 ] ] * weights0[ 0 ] +
                                invBindMatrix[ joints0[ 1 ] ] * jointsWorldMatrix[ joints0[ 1 ] ] * weights0[ 1 ] +
                                invBindMatrix[ joints0[ 2 ] ] * jointsWorldMatrix[ joints0[ 2 ] ] * weights0[ 2 ] +
                                invBindMatrix[ joints0[ 3 ] ] * jointsWorldMatrix[ joints0[ 3 ] ] * weights0[ 3 ];

                            var pos = SharpDX.Vector4.Transform( new SharpDX.Vector4( position, 1f ), skeltonmat );
                            position = new SharpDX.Vector3( pos.X, pos.Y, pos.Z );
                            normal = SharpDX.Vector3.TransformCoordinate( normal, skeltonmat );

                            スキニング後の入力頂点リスト[ i ].Position = position;
                            スキニング後の入力頂点リスト[ i ].Normal = normal;
                            スキニング後の入力頂点リスト[ i ].Texcoord0 = texcoord0;
                        }

                        d3ddc.UpdateSubresource( スキニング後の入力頂点リスト, this._glTFMeshPrimitive.D3DVertexBuffer );
                    }
                    //----------------
                    #endregion
                }
            }
            else
            {
                // todo: スキニングしない場合の実装
            }
            //----------------
            #endregion


            // D3Dで描画する。


            #region " ラスタライザステートを設定する。"
            //----------------
            d3ddc.Rasterizer.State = this._glTFMeshPrimitive.裏面描画RasterizerState;
            //----------------
            #endregion

            #region " 入力アセンブラに頂点バッファ、インデックスバッファ、トポロジを設定する。"
            //----------------

            // 頂点バッファ

            d3ddc.InputAssembler.SetVertexBuffers( 0, new SharpDX.Direct3D11.VertexBufferBinding( this._glTFMeshPrimitive.D3DVertexBuffer, VS_INPUT.SizeInBytes, 0 ) );

            // インデックスバッファ（ある場合）

            if( null != this._glTFMeshPrimitive.D3DVertexIndexBuffer )
            {
                // インデックスバッファの ComponentType から DXGIフォーマットを決める
                var format = SharpDX.DXGI.Format.R32_UInt;

                switch( this._glTFMeshPrimitive.ElementSizeOfIndexBuffer )
                {
                    case 1: format = SharpDX.DXGI.Format.R8_UInt; break;
                    case 2: format = SharpDX.DXGI.Format.R16_UInt; break;
                    default: format = SharpDX.DXGI.Format.R32_UInt; break;
                }

                // アクセサが示すバッファビューを丸ごとセット
                d3ddc.InputAssembler.SetIndexBuffer( this._glTFMeshPrimitive.D3DVertexIndexBuffer, format, 0 );
            }

            // トポロジ

            if( _TopoMap.TryGetValue( this._glTFMeshPrimitive.Mode, out var d3dTopology ) )
            {
                d3ddc.InputAssembler.PrimitiveTopology = d3dTopology;
            }
            else
            {
                //throw new Exception( "サポートされていない Mode が使用されました。" );
                return;
            }
            //----------------
            #endregion

            #region " マテリアルと入力レイアウトを設定する。"
            //----------------
            if( this._glTFMeshPrimitive.MaterialIndex.HasValue )
            {
                int materialIndex = this._glTFMeshPrimitive.MaterialIndex.Value;

                if( 0 <= materialIndex && materialIndex < vrmMaterials.Length )
                {
                    // (A) VRMマテリアルを使う場合

                    var vrmMaterial = vrmMaterials[ materialIndex ];
                    bool useTesselator = ( d3dTopology == SharpDX.Direct3D.PrimitiveTopology.PatchListWith3ControlPoints );

                    if( !this._VRMマテリアルを設定する( d3ddc, vrmMaterial, vrmMaterial.Shader, useTesselator ) )
                    {
                        // 失敗したら Standard マテリアルを使う。
                        this._VRMマテリアルを設定する( d3ddc, vrmMaterial, "Standard", useTesselator );
                    }
                }
                else if( null != this._glTFMeshPrimitive.Material )
                {
                    // (B) glTFマテリアルを使う場合

                    this._glTFマテリアルを設定する( d3ddc, this._glTFMeshPrimitive.Material );
                }
                else
                {
                    // (C) マテリアルの指定が無効である場合

                    this._既定のマテリアルを設定する( d3ddc );
                }
            }
            else
            {
                // (D) マテリアルの指定がない場合

                this._既定のマテリアルを設定する( d3ddc );
            }
            //----------------
            #endregion

            #region " Draw/DrawIndexed する。"
            //----------------
            if( null != this._glTFMeshPrimitive.D3DVertexIndexBuffer )
            {
                int elementSize = this._glTFMeshPrimitive.ElementSizeOfIndexBuffer;
                if( null != this._glTFMeshPrimitive.Indices && this._glTFMeshPrimitive.Indices.BufferView.ByteStride.HasValue )
                    elementSize = this._glTFMeshPrimitive.Indices.BufferView.ByteStride.Value;    // ByteStride が null じゃないならそれを使う

                d3ddc.DrawIndexed( this._glTFMeshPrimitive.ElementNumOfIndexBuffer, this._glTFMeshPrimitive.Indices.ByteOffset / elementSize, 0 );
            }
            else
            {
                d3ddc.Draw( this._glTFMeshPrimitive.VertexNum, 0 );
            }
            //----------------
            #endregion
        }



        // ローカル


        private glTFMeshPrimitive _glTFMeshPrimitive;

        // トポロジの変換表 GL to D3D11
        private static readonly Dictionary<glTFLoader.Schema.MeshPrimitive.ModeEnum, SharpDX.Direct3D.PrimitiveTopology> _TopoMap = new Dictionary<glTFLoader.Schema.MeshPrimitive.ModeEnum, SharpDX.Direct3D.PrimitiveTopology> {
            { glTFLoader.Schema.MeshPrimitive.ModeEnum.POINTS,          SharpDX.Direct3D.PrimitiveTopology.PointList },
            { glTFLoader.Schema.MeshPrimitive.ModeEnum.LINES,           SharpDX.Direct3D.PrimitiveTopology.LineList },
            //{ glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_LOOP,      SharpDX.Direct3D.PrimitiveTopology.LineList },     // Direct3D11 には LINE_LOOP は存在しない。
            { glTFLoader.Schema.MeshPrimitive.ModeEnum.LINE_STRIP,      SharpDX.Direct3D.PrimitiveTopology.LineStrip },
            { glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLES,       SharpDX.Direct3D.PrimitiveTopology.PatchListWith3ControlPoints }, // TRIANGLES のときは HS, DS, GS を有効にするため PatchList を使う。
            //{ glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLES,       SharpDX.Direct3D.PrimitiveTopology.TriangleList },              // HS, DS, GS を使わないならこっち。
            //{ glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_FAN,   SharpDX.Direct3D.PrimitiveTopology.TriangleList },
            { glTFLoader.Schema.MeshPrimitive.ModeEnum.TRIANGLE_STRIP,  SharpDX.Direct3D.PrimitiveTopology.TriangleStrip },
        };

        private void _既定のマテリアルを設定する( SharpDX.Direct3D11.DeviceContext d3ddc )
        {
            // todo: pbr シェーダをちゃんと実装する（今は真っ白ポリゴン）

            if( d3ddc.InputAssembler.PrimitiveTopology == SharpDX.Direct3D.PrimitiveTopology.PatchListWith3ControlPoints )
                d3ddc.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            if( D3DShaders.VertexShaders.TryGetValue( "pbr", out var vs ) )
            {
                d3ddc.VertexShader.Set( vs.shader );
                d3ddc.InputAssembler.InputLayout = vs.layout;
            }

            d3ddc.HullShader.Set( null );
            d3ddc.DomainShader.Set( null );
            d3ddc.GeometryShader.Set( null );

            if( D3DShaders.PixelShaders.TryGetValue( "pbr", out var ps ) )
            {
                d3ddc.PixelShader.Set( ps.shader );
            }
        }

        private bool _VRMマテリアルを設定する( SharpDX.Direct3D11.DeviceContext d3ddc, VRMMaterialProperty vrmMaterial, string shaderName, bool useTesselation )
        {
            // todo: glTFLoader.Schema.Material.AlphaModeEnum を反映する。

            if( string.IsNullOrEmpty( shaderName ) )
                shaderName = vrmMaterial.Shader;

            #region " 頂点シェーダーを設定し、入力アセンブラに入力レイアウトを設定する。"
            //----------------
            if( D3DShaders.VertexShaders.TryGetValue( shaderName, out var vs ) )
            {
                d3ddc.VertexShader.Set( vs.shader );
                d3ddc.InputAssembler.InputLayout = vs.layout;
            }
            else
            {
                return false;
            }
            //----------------
            #endregion

            #region " ハルシェーダー、ドメインシェーダー、ジオメトリシェーダーを設定する。"
            //----------------
            if( useTesselation )
            {
                if( D3DShaders.HullShaders.TryGetValue( shaderName, out var hs ) )
                    d3ddc.HullShader.Set( hs.shader );

                if( D3DShaders.DomainShaders.TryGetValue( shaderName, out var ds ) )
                    d3ddc.DomainShader.Set( ds.shader );

                if( D3DShaders.GeometryShaders.TryGetValue( shaderName, out var gs ) )
                    d3ddc.GeometryShader.Set( gs.shader );
            }
            else
            {
                d3ddc.HullShader.Set( null );
                d3ddc.DomainShader.Set( null );
                d3ddc.GeometryShader.Set( null );
            }
            //----------------
            #endregion

            #region " ピクセルシェーダーを設定する。"
            //----------------
            if( D3DShaders.PixelShaders.TryGetValue( shaderName, out var ps ) )
            {
                d3ddc.PixelShader.Set( ps.shader );
            }
            else
            {
                return false;
            }
            //----------------
            #endregion

            #region " ピクセルシェーダーにテクスチャとサンプラを設定する。"
            //----------------
            // テクスチャの指定がある場合、名前に関係なく並び順にslotへ割り当てる。
            for( int slot = 0; slot < vrmMaterial.TextureProperties.Count; slot++ )
            {
                var texture = vrmMaterial.TextureProperties.ElementAt( slot ).Value;
                d3ddc.PixelShader.SetShaderResource( slot, texture.D3DTextureSRV );

                var d3dSamplerState = texture.Sampler?.D3DSamplerState ?? D3DShaders.D3DDefaultSamplerState;    // 未定義なら既定のものを使う。
                d3ddc.PixelShader.SetSampler( slot, d3dSamplerState );
            }
            //----------------
            #endregion

            return true;    // 成功
        }

        private bool _glTFマテリアルを設定する( SharpDX.Direct3D11.DeviceContext d3ddc, glTFMaterial material )
        {
            // todo: glTFマテリアルを実装する

            this._既定のマテリアルを設定する( d3ddc );
            return true;    // 成功
        }
    }
}
