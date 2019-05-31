using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DirectVRM
{
    /// <summary>
    ///     vrm.blendshape.materialbind
    /// </summary>
    public class glTF_VRM_BlendShapeMaterialValueBind
    {
        [JsonProperty( "materialName" )]
        public string MaterialName { get; set; }

        [JsonProperty( "propertyName" )]
        public string PropertyName { get; set; }

        [JsonConverter( typeof( JsonHelper.ArrayConverter ) )]
        [JsonProperty( "targetValue" )]
        public float[] TargetValue { get; set; }
    }
}
