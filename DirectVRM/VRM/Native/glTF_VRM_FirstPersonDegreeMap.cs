using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DirectVRM
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///     vrm.firstperson.degreemap.schema.json
    /// </remarks>
    public class glTF_VRM_FirstPersonDegreeMap
    {
        /// <summary>
        ///     None linear mapping params. time, value, inTangent, outTangent
        /// </summary>
        [JsonConverter( typeof( JsonHelper.ArrayConverter ) )]
        [JsonProperty( "curve" )]
        public float[] Curve { get; set; }

        /// <summary>
        ///     Look at input clamp range degree.
        /// </summary>
        [JsonProperty( "xRange" )]
        [DefaultValue( 90.0f )]
        public float XRange { get; set; }

        /// <summary>
        ///     Look at map range degree from xRange.
        /// </summary>
        [JsonProperty( "yRange" )]
        [DefaultValue( 10.0f )]
        public float YRange { get; set; }
    }
}
