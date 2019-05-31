using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMMeta : IDisposable
    {
        public string Title => this._Native.Title ?? "";

        public string Version => this._Native.Version ?? "";

        public string Author => this._Native.Author ?? "";

        public string ContactInformation => this._Native.ContactInformation ?? "";

        public string Reference => this._Native.Reference ?? "";

        public glTFTexture Texture { get; protected set; }

        public AllowedUser AllowedUser => this._Native.AllowedUser;

        public UsageLicense ViolentUssage => this._Native.ViolentUssage;

        public UsageLicense SexualUssage => this._Native.SexualUssage;

        public UsageLicense CommercialUssage => this._Native.CommercialUssage;

        public string OtherPermissionUrl => this._Native.OtherPermissionUrl ?? "";

        public LicenseType LicenseType => this._Native.LicenseType;

        public string OtherLicenseUrl => this._Native.OtherLicenseUrl ?? "";



        // 生成と終了


        public VRMMeta( glTF_VRM_Meta native )
        {
            this._Native = native;
            this.Texture = null;
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // Texture
            this.Texture = ( this._Native.Texture.HasValue && 0 <= this._Native.Texture.Value ) ?
                gltf.Textures[ this._Native.Texture.Value ] :
                null;
        }

        public virtual void Dispose()
        {
            this.Texture = null;    // disposeしない
        }



        // ローカル


        private glTF_VRM_Meta _Native;
    }
}
