using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectVRM
{
    public class VRMBlendShapeMaster : IDisposable
    {
        /// <summary>
        ///     現在適用されているブレンドシェイプグループ名。
        /// </summary>
        /// <remarks>
        ///     ブレンドシェイプグループのうち、任意の１つだけを選択することができる。
        ///     （ブレンドシェイプグループを重ね掛けすることはできない。）
        /// </remarks>
        public string CurrentGroupName { get; set; } = "neutral";

        /// <summary>
        ///     現在のブレンド割合（0.0～1.0）。
        /// </summary>
        public float CurrentValue
        {
            get => this._CurrentValue;
            set => this._CurrentValue = ( 0.0f <= value || value <= 1.0f ) ? value : throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        ///     すべてのブレンドシェイプグループのリスト。
        ///     key: プリセット名（すべて大文字）
        /// </summary>
        public Dictionary<string, VRMBlendShapeGroup> BlendShapeGroups { get; protected set; }



        // 生成と終了


        public VRMBlendShapeMaster( glTF_VRM_BlendShapeMaster native )
        {
            this._Native = native;

            #region " BlendShapeGroups "
            //----------------
            this.BlendShapeGroups = new Dictionary<string, VRMBlendShapeGroup>();

            if( null != this._Native.BlendShapeGroups )
            {
                foreach( var group in this._Native.BlendShapeGroups )
                {
                    this.BlendShapeGroups[ group.PresetName ] = new VRMBlendShapeGroup( group );
                }
            }
            //----------------
            #endregion
        }

        public void LateBinding( glTF gltf, glTF_VRM vrm )
        {
            // BlendShapeGroups
            foreach( var kvp in this.BlendShapeGroups )
                kvp.Value.LateBinding( gltf, vrm );
        }

        public virtual void Dispose()
        {
            // BlendShapeGroups
            foreach( var kvp in this.BlendShapeGroups )
                kvp.Value.Dispose();
        }



        // 進行と描画


        /// <summary>
        ///     現在のブレンドシェイプグループを適用する。
        /// </summary>
        public void Apply()
        {
            if( this.BlendShapeGroups.TryGetValue( this.CurrentGroupName, out var presetGroup ) )
            {
                float blendValue = this._CurrentValue;

                // IsBinary が true の場合は、0.0 または 1.0 のいずれかしか値をとらない。
                if( presetGroup.IsBinary )
                    blendValue = (float) Math.Round( blendValue );

                // (1) プリセットのすべてのモーフターゲットを設定する。
                foreach( var bind in presetGroup.Binds )
                {
                    if( bind.Index.HasValue )
                        bind.Mesh.Weights[ bind.Index.Value ] = bind.Weight * blendValue / 100.0f;   // VRMのweightは0～100なので、glTFのweightの0～1に直す
                }

                // (2) プリセットのすべてのマテリアルを設定する。
                foreach( var value in presetGroup.MaterialValues )
                {
                    // todo: プリセットのすべてのマテリアルを設定する。


                }
            }
        }



        // ローカル


        private glTF_VRM_BlendShapeMaster _Native;

        private float _CurrentValue = 1.0f;
    }
}
