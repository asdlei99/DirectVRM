#include "Standard.hlsli"

[ maxvertexcount( 3 ) ]
void main(
	triangle VS_OUTPUT_PS_INPUT input[ 3 ],
	inout TriangleStream<VS_OUTPUT_PS_INPUT> output )
{
	for( uint i = 0; i < 3; i++ )
	{
		VS_OUTPUT_PS_INPUT element = input[ i ];
		output.Append( element );
	}
}
