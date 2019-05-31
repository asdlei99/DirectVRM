using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///     vrm.material
    /// </summary>
    public class glTF_VRM_Material
    {
        [JsonProperty( "name" )]
        public string Name { get; set; }

        [JsonProperty( "shader" )]
        public string Shader { get; set; }

        [JsonProperty( "renderQueue" )]
        [DefaultValue( -1 )]
        public int RenderQueue { get; set; }

        [JsonProperty( "floatProperties" )]
        public Dictionary<string, float> FloatProperties { get; set; }

        [JsonProperty( "vectorProperties" )]
        public Dictionary<string, float[]> VectorProperties { get; set; }

        [JsonProperty( "textureProperties" )]
        public Dictionary<string, int> TextureProperties { get; set; }

        [JsonProperty( "keywordMap" )]
        public Dictionary<string, bool> KeywordMap { get; set; }

        [JsonProperty( "tagMap" )]
        public Dictionary<string, string> TagMap { get; set; }
    }
}
