#include "Common.hlsli"

// エントリ
VS_OUTPUT_PS_INPUT main( VS_INPUT input )
{
	VS_OUTPUT_PS_INPUT output;

	// 右手系を左手系に変換
	float4 position = float4( input.position.xy, -input.position.z, 1 );
	float3 normal = float3( input.normal.xy, -input.normal.z );

	// glTFは三角形が反時計回りなので法線を反転する（ラスタライザステートで表裏も反転しておくこと）
	normal = float3( -normal.x, -normal.y, -normal.z );

	position = mul( position, g_WorldMatrix );
	position = mul( position, g_ViewMatrix );
	position = mul( position, g_ProjectionMatrix );
	output.position = position;
	output.normal = normalize( mul( float4( input.normal.xyz, 1 ), g_WorldMatrix ) ).xyz;
	output.texcoord = input.texcoord;

	return output;
}
