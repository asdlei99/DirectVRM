using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMMaterialProperty : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public string Shader => this._Native.Shader ?? "";

        public int RenderQueue => this._Native.RenderQueue;

        public Dictionary<string, float> FloatProperties => this._Native.FloatProperties ?? new Dictionary<string, float>();

        public Dictionary<string, float[]> VectorProperties => this._Native.VectorProperties ?? new Dictionary<string, float[]>();

        public Dictionary<string, glTFTexture> TextureProperties { get; protected set; }

        public Dictionary<string, bool> KeywordMap => this._Native.KeywordMap ?? new Dictionary<string, bool>();

        public Dictionary<string, string> TagMap => this._Native.TagMap ?? new Dictionary<string, string>();



        // 生成と終了


        public VRMMaterialProperty( glTF_VRM_Material native )
        {
            this._Native = native;

            this.TextureProperties = new Dictionary<string, glTFTexture>();
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            #region " TextureProperties "
            //----------------
            if( null != this._Native.TextureProperties )
            {
                foreach( var kvp in this._Native.TextureProperties )
                {
                    this.TextureProperties[ kvp.Key ] = gltf.Textures[ kvp.Value ];
                }
            }
            //----------------
            #endregion
        }

        public virtual void Dispose()
        {
            this.TextureProperties = null;  // disposeしない
        }



        // ローカル


        private glTF_VRM_Material _Native;
    }
}
