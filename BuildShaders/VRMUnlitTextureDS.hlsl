#include "VRMUnlitTexture.hlsli"

#define NUM_CONTROL_POINTS 3

[domain("tri")]
VS_OUTPUT_PS_INPUT main(
	HS_CONSTANT_DATA_OUTPUT input,
	float3 domain : SV_DomainLocation,
	const OutputPatch<VS_OUTPUT_PS_INPUT, NUM_CONTROL_POINTS> patch)
{
	VS_OUTPUT_PS_INPUT Output;

	float4 pos = float4( 
		patch[ 0 ].position.xyz * domain.x + 
		patch[ 1 ].position.xyz * domain.y + 
		patch[ 2 ].position.xyz * domain.z, 1 );
	pos = mul( pos, g_ViewMatrix );
	pos = mul( pos, g_ProjectionMatrix );
	Output.position = pos;

	float3 nor = float3(
		patch[ 0 ].normal * domain.x + 
		patch[ 1 ].normal * domain.y + 
		patch[ 2 ].normal * domain.z );
	Output.normal = normalize( nor );

	Output.texcoord = float2( 
		patch[ 0 ].texcoord * domain.x + 
		patch[ 1 ].texcoord * domain.y + 
		patch[ 2 ].texcoord * domain.z );

	return Output;
}
