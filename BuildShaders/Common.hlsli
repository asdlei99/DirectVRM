#include "ShaderParameters.hlsli"

// ���_�V�F�[�_�̓��́i���_�o�b�t�@�j�B
// VS_INPUT.cs �ɍ��킹�Ă���̂Œ��ӁB
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


