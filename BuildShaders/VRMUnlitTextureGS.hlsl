#include "VRMUnlitTexture.hlsli"

[maxvertexcount(3)]
void main(
	triangle VS_OUTPUT_PS_INPUT input[3] : SV_POSITION, 
	inout TriangleStream<VS_OUTPUT_PS_INPUT> output
)
{
	for (uint i = 0; i < 3; i++)
	{
		VS_OUTPUT_PS_INPUT element = input[ i ];
		output.Append( element );
	}
}