#include "Skinning.hlsli"
#include "Common.hlsli"

//
// �@X��������Ȃ��̂ŁADispach �� (���_��/256+1, 1, 1) �Ƃ��邱�ƁB
// �@��: ���_���� 260 �Ȃ� Dispach( 2, 1, 1 )
//
[numthreads(256, 1, 1)]
void main( uint3 id : SV_DispatchThreadID )
{
	// ���_�ԍ��i0�`���_��-1�j�Əo�͈ʒu�ibyte�P�ʁi�K��4�̔{���ł��邱�Ɓj�j�̌���B
	uint csIndex = id.x;
	uint vsIndex = csIndex * VS_INPUT_SIZE;

	// �e�\�����o�b�t�@������͒l���擾����B

	float3 position = g_VertexPositionBuffer[ csIndex ];	// �E��n
	float3 normal = g_VertexNormalBuffer[ csIndex ];		// �E��n
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


	matrix invBindMatrix0 = transpose( g_InvBindMatrixBuffer[ joints00 ] );	// �E��n
	matrix invBindMatrix1 = transpose( g_InvBindMatrixBuffer[ joints01 ] );	//
	matrix invBindMatrix2 = transpose( g_InvBindMatrixBuffer[ joints02 ] );	//
	matrix invBindMatrix3 = transpose( g_InvBindMatrixBuffer[ joints03 ] );	//

	matrix jointWorldMatrix0 = g_JointWorldMatrixBuffer[ joints00 ];	// �E��n
	matrix jointWorldMatrix1 = g_JointWorldMatrixBuffer[ joints01 ];	//
	matrix jointWorldMatrix2 = g_JointWorldMatrixBuffer[ joints02 ];	//
	matrix jointWorldMatrix3 = g_JointWorldMatrixBuffer[ joints03 ];	//


	// �X�L�j���O����B

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


	// ���_�o�b�t�@�֏o�͂���B�iVS_INPUT�ɏ]���āj

	g_VSBuffer.Store3( vsIndex + 0, asuint( position ) );
	g_VSBuffer.Store3( vsIndex + 12, asuint( normal ) );
	g_VSBuffer.Store2( vsIndex + 24, asuint( texCoord0 ) );
}