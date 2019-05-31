#include "Common.hlsli"

// �G���g��
VS_OUTPUT_PS_INPUT main( VS_INPUT input )
{
	VS_OUTPUT_PS_INPUT output;

	// �E��n������n�ɕϊ�
	float4 position = float4( input.position.xy, -input.position.z, 1 );
	float3 normal = float3( input.normal.xy, -input.normal.z );

	// glTF�͎O�p�`�������v���Ȃ̂Ŗ@���𔽓]����i���X�^���C�U�X�e�[�g�ŕ\�������]���Ă������Ɓj
	normal = float3( -normal.x, -normal.y, -normal.z );

	position = mul( position, g_WorldMatrix );
	position = mul( position, g_ViewMatrix );
	position = mul( position, g_ProjectionMatrix );
	output.position = position;
	output.normal = normalize( mul( float4( input.normal.xyz, 1 ), g_WorldMatrix ) ).xyz;
	output.texcoord = input.texcoord;

	return output;
}
