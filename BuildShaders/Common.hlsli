#include "ShaderParameters.hlsli"

// 頂点シェーダの入力（頂点バッファ）。
// VS_INPUT.cs に合わせてあるので注意。
struct VS_INPUT
{
	float3 position : POSITION;
	float3 normal : NORMAL;
	float2 texcoord : TEXCOORD0;
};

#define VS_INPUT_SIZE  ((3+3+2)*4)

struct VS_OUTPUT_PS_INPUT
{
	float4 position : SV_POSITION;
	float3 normal : NORMAL;
	float2 texcoord : TEXCOORD0;
};


