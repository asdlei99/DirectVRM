using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DirectVRM
{
    /// <summary>
    ///     BlendShapeAvatar of UniVRM
    /// </summary>
    /// <remarks>
    ///     vrm.blendshape
    /// </remarks>
    public class glTF_VRM_BlendShapeMaster
    {
        [JsonProperty( "blendShapeGroups" )]
        public glTF_VRM_BlendShapeGroup[] BlendShapeGroups { get; set; }
    }
}
