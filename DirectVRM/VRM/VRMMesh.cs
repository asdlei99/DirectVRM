using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMMesh : IDisposable
    {

        // 生成と終了


        public VRMMesh( glTFMesh gltfMesh )
        {
            this._glTFMesh = gltfMesh;
            this._Primitives = new VRMMeshPrimitive[ this._glTFMesh.Primitives.Length ];
        }

        internal void LateBinding()
        {
            for( int i = 0; i < this._Primitives.Length; i++ )
                this._Primitives[ i ] = new VRMMeshPrimitive( this._glTFMesh.Primitives[ i ] );
        }

        public virtual void Dispose()
        {
            foreach( var primitive in this._Primitives )
                primitive.Dispose();
            this._Primitives = null;

            this._glTFMesh = null;  // disposeしない
        }



        // 進行と描画


        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters, glTFSkin skin, VRMMaterialProperty[] vrmMaterials )
        {
            // すべてのプリミティブを描画する。
            foreach( var primitive in this._Primitives )
                primitive.Draw( d3ddc, ref shaderParameters, skin, this._glTFMesh.Weights, this._glTFMesh.Targets, vrmMaterials );
        }


        // ローカル


        private glTFMesh _glTFMesh;

        public VRMMeshPrimitive[] _Primitives;
    }
}
