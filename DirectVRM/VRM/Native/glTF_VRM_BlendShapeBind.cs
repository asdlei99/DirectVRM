using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DirectVRM
{
    /// <summary>
    ///     vrm.blendshape.bind
    /// </summary>
    public class glTF_VRM_BlendShapeBind
    {
        [JsonRequired()]
        [JsonProperty( "mesh" )]
        public int? Mesh { get; set; }

        [JsonRequired()]
        [JsonProperty( "index" )]
        public int? Index { get; set; }

        /// <summary>
        ///     SkinnedMeshRenderer.SetBlendShapeWeight
        /// </summary>
        [JsonRequired()]
        [JsonProperty( "weight" )]
        public float Weight { get; set; }
    }
}
