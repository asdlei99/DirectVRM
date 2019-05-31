using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DirectVRM
{
    /// <summary>
    ///     vrm.firstperson.meshannotation
    /// </summary>
    public class glTF_VRM_FirstPersonMeshAnnotation
    {
        [JsonProperty( "mesh" )]
        public int? Mesh { get; set; }

        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "firstPersonFlag" )]
        public FirstPersonFlag FirstPersonFlag { get; set; }
    }
}
