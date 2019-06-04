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
    ///     vrm.meta
    /// </summary>
    public class glTF_VRM_Meta
    {
        /// <summary>
        ///     Title of VRM model
        /// </summary>
        [JsonProperty( "title" )]
        public string Title { get; set; }

        /// <summary>
        ///     Version of VRM model
        /// </summary>
        [JsonProperty( "version" )]
        public string Version { get; set; }

        /// <summary>
        ///     Author of VRM model
        /// </summary>
        [JsonProperty( "author" )]
        public string Author { get; set; }

        /// <summary>
        ///     Contact Information of VRM model author
        /// </summary>
        [JsonProperty( "contactInformation" )]
        public string ContactInformation { get; set; }

        /// <summary>
        ///     Reference of VRM model
        /// </summary>
        [JsonProperty( "reference" )]
        public string Reference { get; set; }

        /// <summary>
        ///     Thumbnail of VRM model
        ///     When the value is -1, it means that texture is not specified.
        /// </summary>
        [JsonProperty( "texture" )]
        public int? Texture { get; set; }

        /// <summary>
        ///     A person who can perform with this avatar
        /// </summary>
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "allowedUserName" )]
        [DefaultValue( VRMAllowedUser.OnlyAuthor )]
        public VRMAllowedUser AllowedUser { get; set; }

        /// <summary>
        ///     Permission to perform violent acts with this avatar
        /// </summary>
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "violentUssageName" )]
        [DefaultValue( VRMUsageLicense.Disallow )]
        public VRMUsageLicense ViolentUssage { get; set; }

        /// <summary>
        ///     Permission to perform sexual acts with this avatar
        /// </summary>
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "sexualUssageName" )]
        [DefaultValue( VRMUsageLicense.Disallow )]
        public VRMUsageLicense SexualUssage { get; set; }

        /// <summary>
        ///     For commercial use
        /// </summary>
        [JsonRequired()]
        [JsonProperty( "commercialUssageName" )]
        [DefaultValue( VRMUsageLicense.Disallow )]
        public VRMUsageLicense CommercialUssage { get; set; }

        /// <summary>
        ///     If there are any conditions not mentioned above, put the URL link of the license document here.
        /// </summary>
        [JsonProperty( "otherPermissionUrl" )]
        public string OtherPermissionUrl { get; set; }

        /// <summary>
        ///     License type
        /// </summary>
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonRequired()]
        [JsonProperty( "licenseName" )]
        [DefaultValue( VRMLicenseType.Redistribution_Prohibited )]
        public VRMLicenseType LicenseType { get; set; }

        /// <summary>
        ///     If “Other” is selected, put the URL link of the license document here.
        /// </summary>
        [JsonProperty( "otherLicenseUrl" )]
        public string OtherLicenseUrl { get; set; }
    }
}
