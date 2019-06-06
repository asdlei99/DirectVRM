using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFAnimation : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public glTFAnimationChannel[] Channels { get; protected set; }

        public glTFAnimationSampler[] Samplers { get; protected set; }

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFAnimation( int objectIndex, glTFLoader.Schema.Animation native )
        {
            this.ObjectIndex = objectIndex;
            this._Native = native;

            #region " Channels "
            //----------------
            if( null != this._Native.Channels )
            {
                this.Channels = new glTFAnimationChannel[ this._Native.Channels.Length ];

                for( int i = 0; i < this._Native.Channels.Length; i++ )
                {
                    this.Channels[ i ] = new glTFAnimationChannel( this._Native.Channels[ i ] );
                }
            }
            else
            {
                this.Channels = new glTFAnimationChannel[ 0 ];  // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Samplers "
            //----------------
            if( null != this._Native.Samplers )
            {
                this.Samplers = new glTFAnimationSampler[ this._Native.Samplers.Length ];

                for( int i = 0; i < this._Native.Samplers.Length; i++ )
                {
                    this.Samplers[ i ] = new glTFAnimationSampler( this._Native.Samplers[ i ] );
                }
            }
            else
            {
                this.Samplers = new glTFAnimationSampler[ 0 ];  // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Extensions "
            //----------------
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf )
        {
            // Channels
            foreach( var channel in this.Channels )
                channel.LateBinding( gltf, this );

            // Samplers
            foreach( var sampler in this.Samplers )
                sampler.LateBinding( gltf );
        }

        public virtual void Dispose()
        {
            // Channels
            foreach( var channel in this.Channels )
                channel.Dispose();

            // Samplers
            foreach( var sampler in this.Samplers )
                sampler.Dispose();
        }



        // ローカル


        private glTFLoader.Schema.Animation _Native;
    }
}
