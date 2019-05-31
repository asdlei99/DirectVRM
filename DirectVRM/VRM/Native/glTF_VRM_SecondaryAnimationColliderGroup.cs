using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DirectVRM
{
    /// <summary>
    ///     Set sphere balls for colliders used for collision detections with swaying objects.
    /// </summary>
    /// <remarks>
    ///     vrm.secondaryanimation.collidergroup.schema.json
    /// </remarks>
    public class glTF_VRM_SecondaryAnimationColliderGroup
    {
        /// <summary>
        ///     The node of the collider group for setting up collision detections.
        /// </summary>
        [JsonProperty( "node" )]
        public int? Node;

        [JsonProperty( "colliders" )]
        public glTF_VRM_SecondaryAnimationCollider[] Colliders { get; set; }
    }
}
