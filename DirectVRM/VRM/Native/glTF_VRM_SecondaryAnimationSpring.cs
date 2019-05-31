using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///     vrm.secondaryanimation.spring
    /// </summary>
    public class glTF_VRM_SecondaryAnimationSpring
    {
        /// <summary>
        ///     Annotation comment
        /// </summary>
        [JsonProperty( "comment" )]
        public string Comment { get; set; }

        /// <summary>
        ///     The resilience of the swaying object (the power of returning to the initial pose).
        /// </summary>
        [JsonProperty( "stiffiness" )]
        [DefaultValue( 1.0f )]
        public float Stiffiness { get; set; }

        /// <summary>
        ///     The strength of gravity.
        /// </summary>
        [JsonProperty( "gravityPower" )]
        [DefaultValue( 0f )]
        public float GravityPower { get; set; }

        /// <summary>
        ///     The direction of gravity. Set (0, -1, 0) for simulating the gravity. Set (1, 0, 0) for simulating the wind.
        /// </summary>
        [JsonProperty( "gravityDir" )]
        public Vector3 GravityDir { get; set; }

        /// <summary>
        ///     The resistance (deceleration) of automatic animation.
        /// </summary>
        [JsonProperty( "dragForce" )]
        [DefaultValue( 0.4f )]
        public float DragForce { get; set; }

        /// <summary>
        ///     The reference point of a swaying object can be set at any location except the origin. 
        ///     When implementing UI moving with warp, the parent node to move with warp can be specified if you don't want to make the object swaying with warp movement.
        /// </summary>
        /// <remarks>
        ///     NOTE: This value denotes index but may contain -1 as a value.
        ///     When the value is -1, it means that center node is not specified.
        ///     This is a historical issue and a compromise for forward compatibility.
        /// </remarks>
        [JsonProperty( "center" )]
        public int? Center { get; set; }

        /// <summary>
        ///     The radius of the sphere used for the collision detection with colliders.
        /// </summary>
        [JsonProperty( "hitRadius" )]
        [DefaultValue( 0.02f )]
        public float HitRadius { get; set; }

        /// <summary>
        ///     Specify the node index of the root bone of the swaying object.
        /// </summary>
        //[ItemJsonSchema( Minimum = 0 )]
        [JsonConverter( typeof( JsonHelper.ArrayConverter ) )]
        [JsonProperty( "bones" )]
        public int[] Bones { get; set; }

        /// <summary>
        ///     Specify the index of the collider group for collisions with swaying objects.
        /// </summary>
        //[ItemJsonSchema( Minimum = 0 )]
        [JsonConverter( typeof( JsonHelper.ArrayConverter ) )]
        [JsonProperty( "colliderGroups" )]
        public int[] ColliderGroups { get; set; }
    }

}
