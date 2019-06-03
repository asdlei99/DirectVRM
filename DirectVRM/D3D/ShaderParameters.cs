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
            get => Matrix.Transpose( this._parameters.WorldMatrix );            // SharpDX �͍s�D��
            set => this._parameters.WorldMatrix = Matrix.Transpose( value );    // HLSL �͗�D��
        }

        public Matrix ViewMatrix
        {
            get => Matrix.Transpose( this._parameters.ViewMatrix );             // SharpDX �͍s�D��
            set => this._parameters.ViewMatrix = Matrix.Transpose( value );     // HLSL �͗�D��
        }

        public Matrix ProjectionMatrix
        {
            get => Matrix.Transpose( this._parameters.ProjectionMatrix );               // SharpDX �͍s�D��
            set => this._parameters.ProjectionMatrix = Matrix.Transpose( value );       // HLSL �͗�D��
        }

        public float TessellationFactor
        {
            get => this._parameters.TessellationFactor;
            set => this._parameters.TessellationFactor = ( 0.0f < value ) ? value : throw new ArgumentException( "�e�b�Z���[�V�����W���ɂ͕�����ݒ�ł��܂���B" );
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
        ///     HLSL�p�ɁA�s�񂪓]�u���ꂽ�p�����[�^��Ԃ��B
        /// </summary>
        public HLSLParameters GetParametersForHLSL()
            => this._parameters;


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
        public struct HLSLParameters
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



        // ���[�J��


        /// <summary>
        ///     HLSL�p�ɁA�s�񂪓]�u���ꂽ�p�����[�^�B
        /// </summary>
        private HLSLParameters _parameters;
    }
}
