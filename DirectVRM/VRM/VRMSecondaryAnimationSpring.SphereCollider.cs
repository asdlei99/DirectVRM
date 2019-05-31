using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///		衝突判定用の球
    /// </summary>
    public struct SphereCollider
    {
        public Vector3 Position;    // 位置
        public float Radius;        // 半径
    }
}
