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
    ///     vrm.humanoid
    /// </summary>
    public class glTF_VRM_Humanoid
    {
        [JsonProperty( "humanBones" )]
        public glTF_VRM_HumanoidBone[] HumanBones { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.armStretch
        /// </summary>
        [JsonProperty( "armStretch" )]
        [DefaultValue( 0.05f )]
        public float ArmStretch { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.legStretch
        /// </summary>
        [JsonProperty( "legStretch" )]
        [DefaultValue( 0.05f )]
        public float LegStretch { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.upperArmTwist
        /// </summary>
        [JsonProperty( "upperArmTwist" )]
        [DefaultValue( 0.5f )]
        public float UpperArmTwist { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.lowerArmTwist
        /// </summary>
        [JsonProperty( "lowerArmTwist" )]
        [DefaultValue( 0.5f )]
        public float LowerArmTwist { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.upperLegTwist
        /// </summary>
        [JsonProperty( "upperLegTwist" )]
        [DefaultValue( 0.5f )]
        public float UpperLegTwist { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.lowerLegTwist
        /// </summary>
        [JsonProperty( "lowerLegTwist" )]
        [DefaultValue( 0.5f )]
        public float LowerLegTwist { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.feetSpacing
        /// </summary>
        [JsonProperty( "feetSpacing" )]
        [DefaultValue( 0.0f )]
        public float FeetSpacing { get; set; }

        /// <summary>
        ///     Unity's HumanDescription.hasTranslationDoF
        /// </summary>
        [JsonProperty( "hasTranslationDoF" )]
        [DefaultValue( false )]
        public bool HasTranslationDoF { get; set; }
    }
}
