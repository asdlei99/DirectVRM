using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DirectVRM
{
    /// <summary>
    ///     The setting of automatic animation of string-like objects such as tails and hairs.
    /// </summary>
    /// <remarks>
    ///     vrm.secondaryanimation.schema.json
    /// </remarks>
    public class glTF_VRM_SecondaryAnimation
    {
        //[JsonSchema( ExplicitIgnorableItemLength = 0 )]
        [JsonProperty( "boneGroups" )]
        public glTF_VRM_SecondaryAnimationSpring[] BoneGroups { get; set; }

        //[JsonSchema( ExplicitIgnorableItemLength = 0 )]
        [JsonProperty( "colliderGroups" )]
        public glTF_VRM_SecondaryAnimationColliderGroup[] ColliderGroups { get; set; }
    }
}
