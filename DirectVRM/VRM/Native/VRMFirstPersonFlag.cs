using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public enum VRMFirstPersonFlag
    {
        Auto, // Create headlessModel
        Both, // Default layer
        ThirdPersonOnly,
        FirstPersonOnly,
    }
}
