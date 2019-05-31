using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///     cbuffer ShaderParameters に対応する構造体。
    /// </summary>
    /// <remarks>
    ///     cbuffer については、ShaderParameters.hlsli を参照。
    ///     cbuffer では、各メンバが 16byte 境界に合うように配置されるので、
    ///     こちらの構造体でも、<see cref="FieldOffsetAttribute"/> を使って 16byte 境界になるようオフセットを指定する。
    /// </remarks>
    /// <seealso cref="https://docs.microsoft.com/en-us/windows/desktop/direct3dhlsl/dx-graphics-hlsl-packing-rules"/>
    [StructLayout( LayoutKind.Explicit )]
    public struct ShaderParameters
    {
        /// <summary>
        ///     ワールド変換行列。
        /// </summary>
        [FieldOffset( 0 )]
        public Matrix WorldMatrix;              // 64 bytes

        /// <summary>
        ///     ビュー変換行列。
        /// </summary>
        [FieldOffset( 64 )]
        public Matrix ViewMatrix;               // 64 bytes

        /// <summary>
        ///     射影変換行列。
        /// </summary>
        [FieldOffset( 128 )]
        public Matrix ProjectionMatrix;         // 64 bytes

        /// <summary>
        ///     テッセレーション係数。
        ///     モデル単位。
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
