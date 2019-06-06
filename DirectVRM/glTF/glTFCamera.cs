using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTFCamera : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public glTFCameraOrthographic Orthographic { get; }

        public glTFCameraPerspective Perspective { get; }

        public glTFLoader.Schema.Camera.TypeEnum Type => this._Native.Type;

        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;

        public int ObjectIndex { get; }



        // 生成と終了


        public glTFCamera( int objectIndex, glTFLoader.Schema.Camera native )
        {
            this._Native = native;
            this.ObjectIndex = objectIndex;

            // Orthographic
            this.Orthographic = ( null != this._Native.Orthographic ) ?
                new glTFCameraOrthographic( this._Native.Orthographic ) :
                null;

            // Perspective
            this.Perspective = ( null != this._Native.Perspective ) ?
                new glTFCameraPerspective( this._Native.Perspective ) :
                null;

            // Extensions
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
        }

        public void LateBinding( glTF gltf )
        {
            this.Orthographic?.LateBinding( gltf );
            this.Perspective?.LateBinding( gltf );
        }

        public virtual void Dispose()
        {
            this.Orthographic?.Dispose();
            this.Perspective?.Dispose();

            this._Native = null;
        }



        // ローカル


        private glTFLoader.Schema.Camera _Native;
    }
}
