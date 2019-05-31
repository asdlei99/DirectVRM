#include "Standard.hlsli"

// パッチ定数関数
CONSTANT_HS_OUT ConstantsHS_Object( InputPatch<VS_OUTPUT_PS_INPUT, 3> ip, uint PatchID : SV_PrimitiveID )
{
	CONSTANT_HS_OUT Out;

	// 定数バッファの値をそのまま渡す
	Out.Edges[ 0 ] = Out.Edges[ 1 ] = Out.Edges[ 2 ] = g_TessellationFactor; // パッチのエッジのテッセレーション係数
	Out.Inside = ( Out.Edges[ 0 ] + Out.Edges[ 1 ] + Out.Edges[ 2 ] ) / 3.0f; // パッチ内部のテッセレーション係数

	// コントロール ポイントを追加

	float3 B003 = ip[ 2 ].position.xyz;
	float3 B030 = ip[ 1 ].position.xyz;
	float3 B300 = ip[ 0 ].position.xyz;
	float3 N002 = ip[ 2 ].normal;
	float3 N020 = ip[ 1 ].normal;
	float3 N200 = ip[ 0 ].normal;

	Out.B210 = ( ( 2.0f * B003 ) + B030 - ( dot( ( B030 - B003 ), N002 ) * N002 ) ) / 3.0f;
	Out.B120 = ( ( 2.0f * B030 ) + B003 - ( dot( ( B003 - B030 ), N020 ) * N020 ) ) / 3.0f;
	Out.B021 = ( ( 2.0f * B030 ) + B300 - ( dot( ( B300 - B030 ), N020 ) * N020 ) ) / 3.0f;
	Out.B012 = ( ( 2.0f * B300 ) + B030 - ( dot( ( B030 - B300 ), N200 ) * N200 ) ) / 3.0f;
	Out.B102 = ( ( 2.0f * B300 ) + B003 - ( dot( ( B003 - B300 ), N200 ) * N200 ) ) / 3.0f;
	Out.B201 = ( ( 2.0f * B003 ) + B300 - ( dot( ( B300 - B003 ), N002 ) * N002 ) ) / 3.0f;
	float3 E = ( Out.B210 + Out.B120 + Out.B021 + Out.B012 + Out.B102 + Out.B201 ) / 6.0f;
	float3 V = ( B003 + B030 + B300 ) / 3.0f;
	Out.B111 = E + ( ( E - V ) / 2.0f );

	float V12 = 2.0f * dot( B030 - B003, N002 + N020 ) / dot( B030 - B003, B030 - B003 );
	Out.N110 = normalize( N002 + N020 - V12 * ( B030 - B003 ) );
	float V23 = 2.0f * dot( B300 - B030, N020 + N200 ) / dot( B300 - B030, B300 - B030 );
	Out.N011 = normalize( N020 + N200 - V23 * ( B300 - B030 ) );
	float V31 = 2.0f * dot( B003 - B300, N200 + N002 ) / dot( B003 - B300, B003 - B300 );
	Out.N101 = normalize( N200 + N002 - V31 * ( B003 - B300 ) );

	return Out;
}

[ domain( "tri" ) ]
[partitioning( "integer" )]
[outputtopology( "triangle_ccw" )]
[outputcontrolpoints( 3 )]
[patchconstantfunc( "ConstantsHS_Object" )]
VS_OUTPUT_PS_INPUT main( InputPatch<VS_OUTPUT_PS_INPUT, 3> In, uint i : SV_OutputControlPointID, uint PatchID : SV_PrimitiveID )
{
	VS_OUTPUT_PS_INPUT output = (VS_OUTPUT_PS_INPUT) 0;

	output.position = In[ i ].position;
	output.normal = In[ i ].normal;
	output.texcoord = In[ i ].texcoord;

	return output;
}
