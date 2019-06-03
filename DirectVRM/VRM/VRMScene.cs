using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMScene : IDisposable
    {

        public VRMNode[] RootNodes { get; protected set; }



        // 生成と終了


        public VRMScene( glTFScene gltfScene )
        {
            this._glTFScene = gltfScene;

            // RootNodes
            this.RootNodes = new VRMNode[ this._glTFScene.Nodes.Length ];
        }

        internal void LateBinding()
        {
            for( int i = 0; i < this.RootNodes.Length; i++ )
            {
                this.RootNodes[ i ] = new VRMNode( this._glTFScene.Nodes[ i ] );
                this.RootNodes[ i ].LateBinding();
            }
        }

        public virtual void Dispose()
        {
            foreach( var node in this.RootNodes )
                node.Dispose();
            this.RootNodes = null;

            this._glTFScene = null; // disposeしない
        }



        // 進行と描画


        public void Update( double 現在時刻sec )
        {
            // 各ルートノードから順番に進行する。
            foreach( var rootNode in this.RootNodes )
                rootNode.Update( 現在時刻sec );
        }

        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters, VRMMaterialProperty[] materialProperties  )
        {
            // 各ルートノードから順番に描画する。
            foreach( var rootNode in this.RootNodes )
                rootNode.Draw( d3ddc, ref shaderParameters, materialProperties );
        }



        // ローカル


        private glTFScene _glTFScene;
    }
}
