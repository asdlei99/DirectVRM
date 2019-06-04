using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class VRMFirstPerson : IDisposable
    {
        public glTFNode FirstPersonBone { get; protected set; }

        public Vector3? FirstPersonBoneOffset => this._Native.FirstPersonBoneOffset;

        public VRMFirstPersonMeshAnnotation[] MeshAnnotations { get; protected set; }

        public VRMLookAtType LookAtType => this._Native.LookAtType;

        public VRMFirstPersonDegreeMap LookAtHorizontalInner { get; protected set; }

        public VRMFirstPersonDegreeMap LookAtHorizontalOuter { get; protected set; }

        public VRMFirstPersonDegreeMap LookAtVerticalDown { get; protected set; }

        public VRMFirstPersonDegreeMap LookAtVerticalUp { get; protected set; }



        // 生成と終了


        public VRMFirstPerson( glTF_VRM_FirstPerson native )
        {
            this._Native = native;

            this.FirstPersonBone = null;

            #region " MeshAnnotations "
            //----------------
            if( null != this._Native.MeshAnnotations )
            {
                this.MeshAnnotations = new VRMFirstPersonMeshAnnotation[ this._Native.MeshAnnotations.Length ];

                for( int i = 0; i < this._Native.MeshAnnotations.Length; i++ )
                {
                    this.MeshAnnotations[ i ] = new VRMFirstPersonMeshAnnotation( this._Native.MeshAnnotations[ i ] );
                }
            }
            else
            {
                this.MeshAnnotations = new VRMFirstPersonMeshAnnotation[ 0 ];   // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " LookAtHorizontalInner "
            //----------------
            this.LookAtHorizontalInner = ( null != this._Native.LookAtHorizontalInner ) ?
                new VRMFirstPersonDegreeMap( this._Native.LookAtHorizontalInner ) :
                null;
            //----------------
            #endregion

            #region " LookAtHorizontalOuter "
            //----------------
            this.LookAtHorizontalOuter = ( null != this._Native.LookAtHorizontalOuter ) ?
                new VRMFirstPersonDegreeMap( this._Native.LookAtHorizontalOuter ) :
                null;
            //----------------
            #endregion

            #region " LookAtVerticalDown "
            //----------------
            this.LookAtVerticalDown = ( null != this._Native.LookAtVerticalDown ) ?
                new VRMFirstPersonDegreeMap( this._Native.LookAtVerticalDown ) :
                null;
            //----------------
            #endregion

            #region " LookAtVerticalUp "
            //----------------
            this.LookAtVerticalUp = ( null != this._Native.LookAtVerticalUp ) ?
                new VRMFirstPersonDegreeMap( this._Native.LookAtVerticalUp ) :
                null;
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // FirstPersonBone
            this.FirstPersonBone = ( this._Native.FirstPersonBone.HasValue && 0 <= this._Native.FirstPersonBone.Value ) ?
                gltf.Nodes[ this._Native.FirstPersonBone.Value ] :
                null;

            // MeshAnnotations
            foreach( var anno in this.MeshAnnotations )
                anno.LateBinding( gltf, vrm );

            this.LookAtHorizontalInner?.LateBinding( gltf, vrm );
            this.LookAtHorizontalOuter?.LateBinding( gltf, vrm );
            this.LookAtVerticalDown?.LateBinding( gltf, vrm );
            this.LookAtVerticalUp?.LateBinding( gltf, vrm );
        }

        public virtual void Dispose()
        {
            this.FirstPersonBone = null;    // disposeしない

            foreach( var anno in this.MeshAnnotations )
                anno.Dispose();

            this.LookAtHorizontalInner?.Dispose();
            this.LookAtHorizontalOuter?.Dispose();
            this.LookAtVerticalDown?.Dispose();
            this.LookAtVerticalUp?.Dispose();
        }



        // ローカル


        private glTF_VRM_FirstPerson _Native;
    }
}
