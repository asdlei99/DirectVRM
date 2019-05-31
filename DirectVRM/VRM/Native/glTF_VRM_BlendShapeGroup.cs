using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DirectVRM
{
    /// <summary>
    ///     BlendShapeClip of UniVRM
    /// </summary>
    /// <remarks>
    ///     vrm.blendshape.group
    /// </remarks>
    public class glTF_VRM_BlendShapeGroup
    {
        /// <summary>
        ///     Expression name
        /// </summary>
        [JsonProperty( "name" )]
        public string Name { get; set; }

        /// <summary>
        ///     Predefined Expression name
        /// </summary>
        [JsonProperty( "presetName" )]
        public string PresetName { get; set; }

        /// <summary>
        ///     Low level blendshape references.
        /// </summary>
        [JsonProperty( "binds" )]
        public glTF_VRM_BlendShapeBind[] Binds { get; set; }

        /// <summary>
        ///     Material animation references.
        /// </summary>
        [JsonProperty( "materialValues" )]
        public glTF_VRM_BlendShapeMaterialValueBind[] MaterialValues { get; set; }

        /// <summary>
        ///     0 or 1. Do not allow an intermediate value. Value should rounded.
        /// </summary>
        [JsonProperty( "isBinary" )]
        public bool IsBinary { get; set; }
    }
}
