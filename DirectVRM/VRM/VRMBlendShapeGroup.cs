using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMBlendShapeGroup : IDisposable
    {
        public string Name => this._Native.Name ?? "";

        public string PresetName => this._Native.PresetName ?? "";

        public VRMBlendShapeBind[] Binds { get; protected set; }

        public VRMBlendShapeMaterialValueBind[] MaterialValues { get; protected set; }

        public bool IsBinary => this._Native.IsBinary;



        // 生成と終了


        public VRMBlendShapeGroup( glTF_VRM_BlendShapeGroup native )
        {
            this._Native = native;

            #region " Binds "
            //----------------
            if( null != this._Native.Binds )
            {
                this.Binds = new VRMBlendShapeBind[ this._Native.Binds.Length ];

                for( int i = 0; i < this._Native.Binds.Length; i++ )
                {
                    this.Binds[ i ] = new VRMBlendShapeBind( this._Native.Binds[ i ] );
                }
            }
            else
            {
                this.Binds = new VRMBlendShapeBind[ 0 ];    // 未定義時は空配列（not null）
            }
            //----------------
            #endregion

            #region " MaterialValues "
            //----------------
            if( null != this._Native.MaterialValues )
            {
                this.MaterialValues = new VRMBlendShapeMaterialValueBind[ this._Native.MaterialValues.Length ];

                for( int i = 0; i < this._Native.MaterialValues.Length; i++ )
                {
                    this.MaterialValues[ i ] = new VRMBlendShapeMaterialValueBind( this._Native.MaterialValues[ i ] );
                }
            }
            else
            {
                this.MaterialValues = new VRMBlendShapeMaterialValueBind[ 0 ];  // 未定義時は空配列（not null）
            }
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // Binds
            foreach( var bind in this.Binds )
                bind.LateBinding( gltf, vrm );

            // MaterialValues
            foreach( var value in this.MaterialValues )
                value.LateBinding( gltf, vrm );
        }

        public virtual void Dispose()
        {
            // Binds
            foreach( var bind in this.Binds )
                bind.Dispose();

            // MaterialValues
            foreach( var value in this.MaterialValues )
                value.Dispose();
        }



        // ローカル


        private glTF_VRM_BlendShapeGroup _Native;
    }
}
