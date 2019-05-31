#include "Standard.hlsli"

// 入力テクスチャ
Texture2D MainTex : register( t0 );
SamplerState MainSampler : register( s0 );

// エントリ
float4 main( VS_OUTPUT_PS_INPUT input ) : SV_TARGET
{
	// sample the texture
	float4 col = MainTex.Sample( MainSampler, input.texcoord );

	// apply fog
	//UNITY_APPLY_FOG( i.fogCoord, col );

	return col;
}
