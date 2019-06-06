using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class glTF : IDisposable
    {
        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public string[] ExtensionsUsed => ( null != this._Native.ExtensionsUsed ) ? this._Native.ExtensionsUsed : new string[ 0 ];

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public string[] ExtensionsRequired => ( null != this._Native.ExtensionsRequired ) ? this._Native.ExtensionsRequired : new string[ 0 ];

        /// <summary>
        ///     未定義時は既定値（not null）
        /// </summary>
        public glTFAsset Asset { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFBuffer[] Buffers { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFBufferView[] BufferViews { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFAccessor[] Accessors { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFCamera[] Cameras { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFSampler[] Samplers { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFImage[] Images { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFTexture[] Textures { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFMaterial[] Materials { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFNode[] Nodes { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFMesh[] Meshes { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFScene[] Scenes { get; }

        public glTFScene Scene { get; set; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFAnimation[] Animations { get; }

        /// <summary>
        ///     未定義時は空配列（not null）。
        /// </summary>
        public glTFSkin[] Skins { get; }

        /// <summary>
        ///     未定義時は空辞書（not null）。
        /// </summary>
        public Dictionary<string, object> Extensions { get; }

        public glTFLoader.Schema.Extras Extras => this._Native.Extras;



        // 生成と終了


        public glTF( glTFLoader.Schema.Gltf native )
        {
            this._Native = native;
        
            #region " Asset "
            //----------------
            this.Asset = ( null != this._Native.Asset ) ?
                new glTFAsset( this._Native.Asset ) :
                new glTFAsset( null );
            //----------------
            #endregion

            #region " Buffers "
            //----------------
            if( null != this._Native.Buffers )
            {
                this.Buffers = new glTFBuffer[ 1 ];  // 常に１つだけ有効
                this.Buffers[ 0 ] = new glTFBuffer( 0, this._Native.Buffers[ 0 ] );
            }
            else
            {
                this.Buffers = new glTFBuffer[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " BufferViews "
            //----------------
            if( null != this._Native.BufferViews )
            {
                this.BufferViews = new glTFBufferView[ this._Native.BufferViews.Length ];

                for( int i = 0; i < this._Native.BufferViews.Length; i++ )
                {
                    this.BufferViews[ i ] = new glTFBufferView( i, this._Native.BufferViews[ i ] );
                }
            }
            else
            {
                this.BufferViews = new glTFBufferView[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Accessors "
            //----------------
            if( null != this._Native.Accessors )
            {
                this.Accessors = new glTFAccessor[ this._Native.Accessors.Length ];

                for( int i = 0; i < this._Native.Accessors.Length; i++ )
                {
                    this.Accessors[ i ] = new glTFAccessor( i, this._Native.Accessors[ i ] );
                }
            }
            else
            {
                this.Accessors = new glTFAccessor[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Cameras "
            //----------------
            if( null != this._Native.Cameras )
            {
                this.Cameras = new glTFCamera[ this._Native.Cameras.Length ];

                for( int i = 0; i < this._Native.Cameras.Length; i++ )
                {
                    this.Cameras[ i ] = new glTFCamera( i, this._Native.Cameras[ i ] );
                }
            }
            else
            {
                this.Cameras = new glTFCamera[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Samplers "
            //----------------
            if( null != this._Native.Samplers )
            {
                this.Samplers = new glTFSampler[ this._Native.Samplers.Length ];

                for( int i = 0; i < this._Native.Samplers.Length; i++ )
                {
                    this.Samplers[ i ] = new glTFSampler( i, this._Native.Samplers[ i ] );
                }
            }
            else
            {
                this.Samplers = new glTFSampler[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Images "
            //----------------
            if( null != this._Native.Images )
            {
                this.Images = new glTFImage[ this._Native.Images.Length ];

                for( int i = 0; i < this._Native.Images.Length; i++ )
                {
                    this.Images[ i ] = new glTFImage( i, this._Native.Images[ i ] );
                }
            }
            else
            {
                this.Images = new glTFImage[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Textures "
            //----------------
            if( null != this._Native.Textures )
            {
                this.Textures = new glTFTexture[ this._Native.Textures.Length ];

                for( int i = 0; i < this._Native.Textures.Length; i++ )
                {
                    this.Textures[ i ] = new glTFTexture( i, this._Native.Textures[ i ] );
                }
            }
            else
            {
                this.Textures = new glTFTexture[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Materials "
            //----------------
            if( null != this._Native.Materials )
            {
                this.Materials = new glTFMaterial[ this._Native.Materials.Length ];

                for( int i = 0; i < this._Native.Materials.Length; i++ )
                {
                    this.Materials[ i ] = new glTFMaterial( i, this._Native.Materials[ i ] );
                }
            }
            else
            {
                this.Materials = new glTFMaterial[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Nodes "
            //----------------
            if( null != this._Native.Nodes )
            {
                this.Nodes = new glTFNode[ this._Native.Nodes.Length ];

                for( int i = 0; i < this._Native.Nodes.Length; i++ )
                {
                    this.Nodes[ i ] = new glTFNode( i, this._Native.Nodes[ i ] );
                }
            }
            else
            {
                this.Nodes = new glTFNode[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Meshes "
            //----------------
            if( null != this._Native.Meshes )
            {
                this.Meshes = new glTFMesh[ this._Native.Meshes.Length ];

                for( int i = 0; i < this._Native.Meshes.Length; i++ )
                {
                    this.Meshes[ i ] = new glTFMesh( this._Native.Meshes[ i ] );
                }
            }
            else
            {
                this.Meshes = new glTFMesh[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Scenes "
            //----------------
            if( null != this._Native.Scenes )
            {
                this.Scenes = new glTFScene[ this._Native.Scenes.Length ];

                for( int i = 0; i < this._Native.Scenes.Length; i++ )
                {
                    this.Scenes[ i ] = new glTFScene( i, this._Native.Scenes[ i ] );
                }
            }
            else
            {
                this.Scenes = new glTFScene[ 0 ]; // 未定義時は空配列（not null）
            }

            this.Scene = null;
            //----------------
            #endregion

            #region " Animation "
            //----------------
            if( null != this._Native.Animations )
            {
                this.Animations = new glTFAnimation[ this._Native.Animations.Length ];

                for( int i = 0; i < this._Native.Animations.Length; i++ )
                {
                    this.Animations[ i ] = new glTFAnimation( i, this._Native.Animations[ i ] );
                }
            }
            else
            {
                this.Animations = new glTFAnimation[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Skins "
            //----------------
            if( null != this._Native.Skins )
            {
                this.Skins = new glTFSkin[ this._Native.Skins.Length ];

                for( int i = 0; i < this._Native.Skins.Length; i++ )
                {
                    this.Skins[ i ] = new glTFSkin( i, this._Native.Skins[ i ] );
                }
            }
            else
            {
                this.Skins = new glTFSkin[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " Extensions "
            //----------------
            this.Extensions = this._Native.Extensions ?? new Dictionary<string, object>();
            //----------------
            #endregion
        }

        internal void LateBinding( byte[] binaryBuffer, SharpDX.Direct3D11.Device d3dDevice )
        {
            // Asset
            this.Asset?.LateBinding( this );

            // Buffers
            if( 0 < this.Buffers.Length )
                this.Buffers[ 0 ].LateBinding( binaryBuffer );   // 最初の Buffer だけ対応

            // BufferViews - Buffersより後であること
            foreach( var bufferView in this.BufferViews )
                bufferView.LateBinding( this );

            // Accessors
            foreach( var accessor in this.Accessors )
                accessor.LateBinding( this );

            // Cameras
            foreach( var camera in this.Cameras )
                camera.LateBinding( this );

            // Samplers
            foreach( var sampler in this.Samplers )
                sampler.LateBinding( this, d3dDevice );

            // Images
            foreach( var image in this.Images )
                image.LateBinding( this );

            // Textures
            foreach( var texture in this.Textures )
                texture.LateBinding( this, d3dDevice );

            // Materials
            foreach( var material in this.Materials )
                material.LateBinding( this );

            // Nodes
            foreach( var node in this.Nodes )
                node.LateBinding( this );

            // Meshes
            foreach( var mesh in this.Meshes )
                mesh.LateBinding( this, d3dDevice );

            // Scenes, Scene
            foreach( var scene in this.Scenes )
                scene.LateBinding( this );

            this.Scene = ( this._Native.Scene.HasValue ) ?
                this.Scenes[ this._Native.Scene.Value ] :
                null;

            // Animations
            foreach( var animation in this.Animations )
                animation.LateBinding( this );

            // Skins
            foreach( var skin in this.Skins )
                skin.LateBinding( this, d3dDevice );
        }

        public virtual void Dispose()
        {
            // Skins
            foreach( var skin in this.Skins )
                skin.Dispose();

            // Animations 
            foreach( var animation in this.Animations )
                animation.Dispose();

            // Scenes, Scene
            this.Scene = null;
            foreach( var scene in this.Scenes )
                scene.Dispose();

            // Meshes
            foreach( var mesh in this.Meshes )
                mesh.Dispose();

            // Nodes
            foreach( var node in this.Nodes )
                node.Dispose();

            // Materials
            foreach( var material in this.Materials )
                material.Dispose();

            // Textures
            foreach( var texture in this.Textures )
                texture.Dispose();

            // Images
            foreach( var image in this.Images )
                image.Dispose();

            // Samplers
            foreach( var sampler in this.Samplers )
                sampler.Dispose();

            // Cameras
            foreach( var camera in this.Cameras )
                camera.Dispose();
            
            // Accessors
            foreach( var accessor in this.Accessors )
                accessor.Dispose();
            
            // BufferViews
            foreach( var bufferView in this.BufferViews )
                bufferView.Dispose();

            // Buffers
            foreach( var buffer in this.Buffers )
                buffer.Dispose();

            // Asset
            this.Asset?.Dispose();
        }



        // 進行と描画


        public void Update( double 現在時刻sec )
        {
            var scene = this.Scene ?? this.Scenes[ 0 ];

            scene.Update( 現在時刻sec );
        }

        public void Draw( SharpDX.Direct3D11.DeviceContext d3ddc, ref ShaderParameters shaderParameters )
        {
            this.Scene?.Draw( d3ddc, ref shaderParameters );
        }


        // ローカル


        public glTFLoader.Schema.Gltf _Native;
    }
}
