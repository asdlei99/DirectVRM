using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;

namespace DirectVRM
{
    public class ShaderParameters
    {
        public Matrix WorldMatrix
        {
            get => Matrix.Transpose( this._parameters.WorldMatrix );            // SharpDX は行優先
            set => this._parameters.WorldMatrix = Matrix.Transpose( value );    // HLSL は列優先
        }

        public Matrix ViewMatrix
        {
            get => Matrix.Transpose( this._parameters.ViewMatrix );             // SharpDX は行優先
            set => this._parameters.ViewMatrix = Matrix.Transpose( value );     // HLSL は列優先
        }

        public Matrix ProjectionMatrix
        {
            get => Matrix.Transpose( this._parameters.ProjectionMatrix );               // SharpDX は行優先
            set => this._parameters.ProjectionMatrix = Matrix.Transpose( value );       // HLSL は列優先
        }

        public float TessellationFactor
        {
            get => this._parameters.TessellationFactor;
            set => this._parameters.TessellationFactor = ( 0.0f < value ) ? value : throw new ArgumentException( "テッセレーション係数には負数を設定できません。" );
        }

        public static int SizeInBytes => HLSLParameters.SizeInBytes;


        public ShaderParameters()
        {
            var cameraPosition = new Vector3( 0f, 1.3f, -4.0f );
            var cameraForward = new Vector3( 0f, 1.0f, 0f );
            var cameraUp = new Vector3( 0f, 1f, 0f );
            float fov = 30f;    // degree
            float nearZ = 0.25f;
            float farZ = 2000f;
            float aspect = 16f / 9f;// 1.618f;

            this._parameters = new HLSLParameters {
                WorldMatrix = Matrix.Identity,
                ViewMatrix = Matrix.LookAtLH( cameraPosition, cameraForward, cameraUp ),
                ProjectionMatrix = Matrix.PerspectiveFovLH( MathUtil.Pi * fov / 180.0f, aspect, nearZ, farZ ),
                TessellationFactor = 5.0f,
            };
        }

        /// <summary>
        ///     HLSL用に、行列が転置されたパラメータを返す。
        /// </summary>
        public HLSLParameters GetParametersForHLSL()
            => this._parameters;


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
        public struct HLSLParameters
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



        // ローカル


        /// <summary>
        ///     HLSL用に、行列が転置されたパラメータ。
        /// </summary>
        private HLSLParameters _parameters;
    }
}
