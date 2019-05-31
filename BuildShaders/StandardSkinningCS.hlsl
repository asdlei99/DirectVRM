#include "Skinning.hlsli"
#include "Common.hlsli"

//
// 　Xしか扱わないので、Dispach は (頂点数/256+1, 1, 1) とすること。
// 　例: 頂点数が 260 なら Dispach( 2, 1, 1 )
//
[numthreads(256, 1, 1)]
void main( uint3 id : SV_DispatchThreadID )
{
	// 頂点番号（0～頂点数-1）と出力位置（byte単位（必ず4の倍数であること））の決定。
	uint csIndex = id.x;
	uint vsIndex = csIndex * VS_INPUT_SIZE;

	// 各構造化バッファから入力値を取得する。

	float3 position = g_VertexPositionBuffer[ csIndex ];	// 右手系
	float3 normal = g_VertexNormalBuffer[ csIndex ];		// 右手系
	float2 texCoord0 = g_VertexTexCoord0Buffer[ csIndex ];
	
	float4 weights = g_VertexWeights0Buffer[ csIndex ];
	float weights00 = weights.x;
	float weights01 = weights.y;
	float weights02 = weights.z;
	float weights03 = weights.w;

	uint2 indices = g_VertexJoints0Buffer[ csIndex ];
	int joints00 = indices.x & 0x0000FFFF; 
	int joints01 = indices.x >> 16;
	int joints02 = indices.y & 0x0000FFFF;
	int joints03 = indices.y >> 16; 


	matrix invBindMatrix0 = transpose( g_InvBindMatrixBuffer[ joints00 ] );	// 右手系
	matrix invBindMatrix1 = transpose( g_InvBindMatrixBuffer[ joints01 ] );	//
	matrix invBindMatrix2 = transpose( g_InvBindMatrixBuffer[ joints02 ] );	//
	matrix invBindMatrix3 = transpose( g_InvBindMatrixBuffer[ joints03 ] );	//

	matrix jointWorldMatrix0 = g_JointWorldMatrixBuffer[ joints00 ];	// 右手系
	matrix jointWorldMatrix1 = g_JointWorldMatrixBuffer[ joints01 ];	//
	matrix jointWorldMatrix2 = g_JointWorldMatrixBuffer[ joints02 ];	//
	matrix jointWorldMatrix3 = g_JointWorldMatrixBuffer[ joints03 ];	//


	// スキニングする。

	matrix jointMatrix[ 4 ];
	jointMatrix[ 0 ] = mul( invBindMatrix0, jointWorldMatrix0 );
	jointMatrix[ 1 ] = mul( invBindMatrix1, jointWorldMatrix1 );
	jointMatrix[ 2 ] = mul( invBindMatrix2, jointWorldMatrix2 );
	jointMatrix[ 3 ] = mul( invBindMatrix3, jointWorldMatrix3 );

	matrix skinningMatrix =
		jointMatrix[ 0 ] * weights00 +
		jointMatrix[ 1 ] * weights01 +
		jointMatrix[ 2 ] * weights02 +
		jointMatrix[ 3 ] * weights03;

	position = mul( float4( position, 1 ), skinningMatrix ).xyz;
	normal = normalize( mul( float4( normal, 1 ), skinningMatrix ).xyz );


	// 頂点バッファへ出力する。（VS_INPUTに従って）

	g_VSBuffer.Store3( vsIndex + 0, asuint( position ) );
	g_VSBuffer.Store3( vsIndex + 12, asuint( normal ) );
	g_VSBuffer.Store2( vsIndex + 24, asuint( texCoord0 ) );
}