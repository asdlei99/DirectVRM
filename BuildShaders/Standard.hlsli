#include "Common.hlsli"

struct CONSTANT_HS_OUT
{
	float Edges[ 3 ] : SV_TessFactor; // �p�b�`�̃G�b�W�̃e�b�Z���[�V�����W��
	float Inside : SV_InsideTessFactor; // �p�b�`�����̃e�b�Z���[�V�����W��
	float3 B210 : POSITION3;
	float3 B120 : POSITION4;
	float3 B021 : POSITION5;
	float3 B012 : POSITION6;
	float3 B102 : POSITION7;
	float3 B201 : POSITION8;
	float3 B111 : CENTER;
	float3 N110 : NORMAL3;
	float3 N011 : NORMAL4;
	float3 N101 : NORMAL5;
};
