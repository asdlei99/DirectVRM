using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DirectVRM
{
    /// <summary>
    ///     VRM extension is for 3d humanoid avatars (and models) in VR applications.
    /// </summary>
    public class glTF_VRM
    {
        public static string ExtensionName => "VRM";


        /// <summary>
        ///     Version of exporter that vrm created. UniVRM-0.46
        /// </summary>
        [JsonProperty( "exporterVersion" )]
        public string ExporterVersion { get; set; }

        /// <summary>
        ///     The VRM specification version that this extension uses. 0.0
        /// </summary>
        [JsonProperty( "specVersion" )]
        public string SpecVersion { get; set; }

        [JsonProperty( "meta" )]
        public glTF_VRM_Meta Meta { get; set; }

        [JsonProperty( "humanoid" )]
        public glTF_VRM_Humanoid Humanoid { get; set; }

        [JsonProperty( "firstPerson" )]
        public glTF_VRM_FirstPerson FirstPerson { get; set; }

        [JsonProperty( "blendShapeMaster" )]
        public glTF_VRM_BlendShapeMaster BlendShapeMaster { get; set; }

        [JsonProperty( "secondaryAnimation" )]
        public glTF_VRM_SecondaryAnimation SecondaryAnimation { get; set; }

        [JsonProperty( "materialProperties" )]
        public glTF_VRM_Material[] MaterialProperties { get; set; }
    }
}
