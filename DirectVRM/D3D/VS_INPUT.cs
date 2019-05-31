using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DirectVRM
{
    /// <summary>
    ///     頂点シェーダーの入力。
    /// </summary>
    /// <remarks>
    ///     Common.hlsli の VS_INPUT に合わせてあるので、迂闊に変更しないこと。
    /// </remarks>
    public struct VS_INPUT
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texcoord0;

        public static readonly InputElement[] VertexElements = {
            new InputElement { SemanticName = "POSITION", Format = Format.R32G32B32_Float },
            new InputElement { SemanticName = "NORMAL",   Format = Format.R32G32B32_Float, AlignedByteOffset = InputElement.AppendAligned },
            new InputElement { SemanticName = "TEXCOORD", Format = Format.R32G32_Float, SemanticIndex = 0, AlignedByteOffset = InputElement.AppendAligned },  // UV
		};

        public static int SizeInBytes => Marshal.SizeOf( typeof( VS_INPUT ) );
    }
}
