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
    /// 
    /// </summary>
    /// <remarks>
    ///     vrm.humanoid.bone.schema.json
    /// </remarks>
    public class glTF_VRM_HumanoidBone
    {
        /// <summary>
        ///     Human bone name.
        /// </summary>
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "bone" )]
        public BoneType Bone { get; set; }

        /// <summary>
        ///     Reference node index.
        ///     When the value is -1, it means that no node is found.
        /// </summary>
        [JsonProperty( "node" )]
        public int? Node { get; set; }

        /// <summary>
        ///     Unity's HumanLimit.useDefaultValues
        /// </summary>
        [JsonProperty( "useDefaultValues" )]
        [DefaultValue( true )]
        public bool UseDefaultValues { get; set; }

        /// <summary>
        ///     Unity's HumanLimit.min
        /// </summary>
        /// <remarks>
        ///     Disabled if <see cref="UseDefaultValues"/> is true.
        /// </remarks>
        [JsonProperty( "min" )]
        public Vector3? Min { get; set; }

        /// <summary>
        ///     Unity's HumanLimit.max
        /// </summary>
        /// <remarks>
        ///     Disabled if <see cref="UseDefaultValues"/> is true.
        /// </remarks>
        [JsonProperty( "max" )]
        public Vector3? Max { get; set; }

        /// <summary>
        ///     Unity's HumanLimit.center
        /// </summary>
        /// <remarks>
        ///     Disabled if <see cref="UseDefaultValues"/> is true.
        /// </remarks>
        [JsonProperty( "center" )]
        public Vector3? Center { get; set; }

        /// <summary>
        ///     Unity's HumanLimit.axisLength
        /// </summary>
        /// <remarks>
        ///     Disabled if <see cref="UseDefaultValues"/> is true.
        /// </remarks>
        [JsonProperty( "axisLength" )]
        public float? AxisLength { get; set; }
    }
}
