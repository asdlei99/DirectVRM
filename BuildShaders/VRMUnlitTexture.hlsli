#include "Common.hlsli"

struct HS_CONSTANT_DATA_OUTPUT
{
	float EdgeTessFactor[ 3 ] : SV_TessFactor;
	float InsideTessFactor : SV_InsideTessFactor;
};
