using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMNode : IDisposable
    {

        // 生成と終了


        public VRMNode( glTFNode gltfNode )
        {
            this._glTFNode = gltfNode;

            this._Mesh = null;
            this._Children = new VRMNode[ this._glTFNode.Children.Length ];
        }

        internal void LateBinding()
        {
            if( null != this._glTFNode.Mesh )
            {
                this._Mesh = new VRMMesh( this._glTFNode.Mesh );
                this._Mesh.LateBinding();
            }

            for( int i = 0; i < this._Children.Length; i++ )
            {
                this._Children[ i ] = new VRMNode( this._glTFNode.Children[ i ] );
                this._Children[ i ].LateBinding();
            }
        }

        public virtual void Dispose()
        {
            foreach( var child in this._Children )
                child.Dispose();
            this._Children = null;

            this._Mesh?.Dispose();
            this._Mesh = null;

            this._glTFNode = null;  // disposeしない
        }



        // 進行と描画


        public void Update( double 現在時刻sec )
        {
            // glTFNode の Transform 関連のプロパティは それぞれ set 時にリアルタイムで更新されるため
            // ここでは何もすることがない。
        }

        /// <summary>
        ///     自分と子ノードを描画する。
        /// </summary>
        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters, VRMMaterialProperty[] vrmMaterials )
        {
            // ノードにメッシュがあれば描画する。
            this._Mesh?.Draw( d3ddc, ref shaderParameters, this._glTFNode.Skin, vrmMaterials );

            // 子ノードがあれば再帰的に描画する。
            foreach( var child in this._Children )
                child.Draw( d3ddc, ref shaderParameters, vrmMaterials );
        }



        // ローカル


        private glTFNode _glTFNode;

        private VRMMesh _Mesh;

        private VRMNode[] _Children;
    }
}
