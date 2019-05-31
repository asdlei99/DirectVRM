// スキニング用コンピュートシェーダーが扱えるジョイントの最大数
#define MAX_JOINT    768

// スキニング用コンピュートシェーダーの入力（構造化バッファ）

StructuredBuffer<float3> g_VertexPositionBuffer : register( t0 );		// 頂点の POSITION 配列
StructuredBuffer<float3> g_VertexNormalBuffer : register( t1 );			// 頂点の NORMAL 配列
StructuredBuffer<float2> g_VertexTexCoord0Buffer : register( t2 );		// 頂点の TEXCOORD0 配列
StructuredBuffer<float4> g_VertexWeights0Buffer : register( t3 );		// 頂点の WEIGHTS_0 配列
StructuredBuffer<uint2> g_VertexJoints0Buffer : register( t4 );			// 頂点の JOINTS_0 配列を Skin内の行列順に直したインデックスの配列(ushort) 
StructuredBuffer<matrix> g_InvBindMatrixBuffer : register( t5 );		// スキンの逆バインド行列の配列
StructuredBuffer<matrix> g_JointWorldMatrixBuffer : register( t6 );		// ジョイントのワールド変換行列の配列

// スキニング用コンピュートシェーダーの出力（頂点バッファ; RWバイトバッファ）
// → このバッファは、そのまま頂点シェーダの入力（頂点バッファ; VS_INPUT）として使用される。
RWByteAddressBuffer g_VSBuffer : register( u0 );

