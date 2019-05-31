using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///     cbuffer ShaderParameters �ɑΉ�����\���́B
    /// </summary>
    /// <remarks>
    ///     cbuffer �ɂ��ẮAShaderParameters.hlsli ���Q�ƁB
    ///     cbuffer �ł́A�e�����o�� 16byte ���E�ɍ����悤�ɔz�u�����̂ŁA
    ///     ������̍\���̂ł��A<see cref="FieldOffsetAttribute"/> ���g���� 16byte ���E�ɂȂ�悤�I�t�Z�b�g���w�肷��B
    /// </remarks>
    /// <seealso cref="https://docs.microsoft.com/en-us/windows/desktop/direct3dhlsl/dx-graphics-hlsl-packing-rules"/>
    [StructLayout( LayoutKind.Explicit )]
    public struct ShaderParameters
    {
        /// <summary>
        ///     ���[���h�ϊ��s��B
        /// </summary>
        [FieldOffset( 0 )]
        public Matrix WorldMatrix;              // 64 bytes

        /// <summary>
        ///     �r���[�ϊ��s��B
        /// </summary>
        [FieldOffset( 64 )]
        public Matrix ViewMatrix;               // 64 bytes

        /// <summary>
        ///     �ˉe�ϊ��s��B
        /// </summary>
        [FieldOffset( 128 )]
        public Matrix ProjectionMatrix;         // 64 bytes

        /// <summary>
        ///     �e�b�Z���[�V�����W���B
        ///     ���f���P�ʁB
        /// </summary>
        [FieldOffset( 192 )]
        public float TessellationFactor;        // 4 bytes

        //[FieldOffset( 196 )]
        //public float dummy;                   // 4 bytes

        //[FieldOffset( 200 )]
        //public float dummy;                   // 4 bytes

        //[FieldOffset( 204 )]
        //public float dummy;                   // 4 bytes


        public static int SizeInBytes => 208;
    }
}
