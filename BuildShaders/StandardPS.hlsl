#include "Standard.hlsli"

// ���̓e�N�X�`��
Texture2D MainTex : register( t0 );
SamplerState MainSampler : register( s0 );

// �G���g��
float4 main( VS_OUTPUT_PS_INPUT input ) : SV_TARGET
{
	// sample the texture
	float4 col = MainTex.Sample( MainSampler, input.texcoord );

	// apply fog
	//UNITY_APPLY_FOG( i.fogCoord, col );

	return col;
}
