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
    ///     vrm.firstperson.schema.json
    /// </remarks>
    public class glTF_VRM_FirstPerson
    {
        /// <summary>
        ///     The bone whose rendering should be turned off in first-person view. Usually Head is specified.
        ///     When the value is -1, it means that no bone for first person is found.
        /// </summary>
        [JsonProperty( "firstPersonBone" )]
        public int? FirstPersonBone { get; set; }

        /// <summary>
        ///     The target position of the VR headset in first-person view. It is assumed that an offset from the head bone to the VR headset is added.
        /// </summary>
        [JsonProperty( "firstPersonBoneOffset" )]
        public Vector3? FirstPersonBoneOffset { get; set; }

        /// <summary>
        ///     Switch display / undisplay for each mesh in first-person view or the others.
        /// </summary>
        [JsonProperty( "meshAnnotations" )]
        public glTF_VRM_FirstPersonMeshAnnotation[] MeshAnnotations { get; set; }

        /// <summary>
        ///     Eye controller mode.
        /// </summary>
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "lookAtTypeName" )]
        [DefaultValue( LookAtType.Bone )]
        public LookAtType LookAtType { get; set; }

        /// <summary>
        ///     Eye controller setting.
        /// </summary>
        [JsonProperty( "lookAtHorizontalInner" )]
        public glTF_VRM_FirstPersonDegreeMap LookAtHorizontalInner { get; set; }

        /// <summary>
        ///     Eye controller setting.
        /// </summary>
        [JsonProperty( "lookAtHorizontalOuter" )]
        public glTF_VRM_FirstPersonDegreeMap LookAtHorizontalOuter { get; set; }

        /// <summary>
        ///     Eye controller setting.
        /// </summary>
        [JsonProperty( "lookAtVerticalDown" )]
        public glTF_VRM_FirstPersonDegreeMap LookAtVerticalDown { get; set; }

        /// <summary>
        ///     Eye controller setting.
        /// </summary>
        [JsonProperty( "lookAtVerticalUp" )]
        public glTF_VRM_FirstPersonDegreeMap LookAtVerticalUp { get; set; }
    }
}
