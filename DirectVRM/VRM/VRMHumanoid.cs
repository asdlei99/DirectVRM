using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMHumanoid : IDisposable
    {
        public VRMHumanoidBone[] HumanBones { get; protected set; }

        public float ArmStretch => this._Native.ArmStretch;

        public float LegStretch => this._Native.LegStretch;

        public float UpperArmTwist => this._Native.UpperArmTwist;

        public float LowerArmTwist => this._Native.LowerArmTwist;

        public float UpperLegTwist => this._Native.UpperLegTwist;

        public float LowerLegTwist => this._Native.LowerLegTwist;

        public float FeetSpacing => this._Native.FeetSpacing;

        public bool HasTranslationDoF => this._Native.HasTranslationDoF;



        // 生成と終了


        public VRMHumanoid( glTF_VRM_Humanoid native )
        {
            this._Native = native;

            #region " HumanBones "
            //----------------
            if( null != this._Native.HumanBones )
            {
                this.HumanBones = new VRMHumanoidBone[ this._Native.HumanBones.Length ];

                for( int i = 0; i < this._Native.HumanBones.Length; i++ )
                {
                    this.HumanBones[ i ] = new VRMHumanoidBone( this._Native.HumanBones[ i ] );
                }
            }
            else
            {
                this.HumanBones = new VRMHumanoidBone[ 0 ]; // 未定義時は空配列（not null）
            }
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // HumanBones
            foreach( var bone in this.HumanBones )
                bone.LateBinding( gltf, vrm );
        }

        public virtual void Dispose()
        {
            // HumanBones
            foreach( var bone in this.HumanBones )
                bone.Dispose();
        }



        // ローカル


        private glTF_VRM_Humanoid _Native;
    }
}
