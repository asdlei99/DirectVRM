#include "Standard.hlsli"

[ domain( "tri" ) ]
VS_OUTPUT_PS_INPUT main( CONSTANT_HS_OUT In, float3 uvw : SV_DomainLocation, const OutputPatch<VS_OUTPUT_PS_INPUT, 3> patch )
{
	VS_OUTPUT_PS_INPUT Out = (VS_OUTPUT_PS_INPUT) 0;

	float U = uvw.x;
	float V = uvw.y;
	float W = uvw.z;
	float UU = U * U;
	float VV = V * V;
	float WW = W * W;
	float UUU = UU * U;
	float VVV = VV * V;
	float WWW = WW * W;

	// 頂点座標
	float3 Position = patch[ 2 ].position.xyz * WWW +
		patch[ 1 ].position.xyz * UUU +
		patch[ 0 ].position.xyz * VVV +
		In.B210 * WW * 3 * U +
		In.B120 * W * UU * 3 +
		In.B201 * WW * 3 * V +
		In.B021 * UU * 3 * V +
		In.B102 * W * VV * 3 +
		In.B012 * U * VV * 3 +
		In.B111 * 6 * W * U * V;
	Out.position = mul( float4( Position, 1 ), mul( g_ViewMatrix, g_ProjectionMatrix ) );

	// 法線ベクトル
	float3 Normal = patch[ 2 ].normal * WW +
		patch[ 1 ].normal * UU +
		patch[ 0 ].normal * VV +
		In.N110 * W * U +
		In.N011 * U * V +
		In.N101 * W * V;
	Out.normal = normalize( Normal );

	// テクスチャ座標
	Out.texcoord = patch[ 2 ].texcoord * W + patch[ 1 ].texcoord * U + patch[ 0 ].texcoord * V;

	return Out;
}
