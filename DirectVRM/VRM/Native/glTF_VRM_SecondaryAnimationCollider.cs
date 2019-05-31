using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using SharpDX;

namespace DirectVRM
{
    public class glTF_VRM_SecondaryAnimationCollider
    {
        /// <summary>
        ///     The local coordinate from the node of the collider group.
        /// </summary>
        [JsonProperty( "offset" )]
        public Vector3 Offset { get; set; }

        /// <summary>
        ///     The radius of the collider.
        ///     0.0～1.0。
        /// </summary>
        [JsonProperty( "radius" )]
        public float Radius { get; set; }
    }
}
