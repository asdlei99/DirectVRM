using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public class BVHMotion
    {
        public BVHMotion( string bvhFilePath, VRMScene scene )
        {
            var text = File.ReadAllText( bvhFilePath );
            this._bvhFormat = new BVHForat( text );

            this._DataNumPerFrame = 0;
            foreach( var node in this._bvhFormat.Root.Traverse() )
                this._DataNumPerFrame += node.Channels.Length;

            var hips = scene.glTF.Nodes.Where( ( n ) => n.Name == "Hips" ).FirstOrDefault();
            if( null != hips )
                this._InitialLocalPositionRH = hips.LocalPositionRH;
        }

        private Vector3 _InitialLocalPositionRH;

        public void GetFrameAndApply( float time, VRMScene scene )
        {
            this._GetFrame( time, ( bnode, pos, rot ) => {

                foreach( var gnode in scene.glTF.Nodes )
                {
                    if( gnode.Name.ToLower() == bnode.Name.ToLower() )
                    {
                        if( gnode.Name == "Hips" )
                            gnode.LocalPositionRH = this._InitialLocalPositionRH - pos * 0.0115f;
                        gnode.LocalRotationRH = rot;
                        break;
                    }
                }

            } );
        }

        private void _GetFrame( float time, Action<BVHForat.BvhNode, Vector3, Quaternion> applyTo )
        {
            if( 0 >= this._bvhFormat.MotionFrames || applyTo == null )
                return;

            int offset = (int)Math.Truncate( time / this._bvhFormat.MotionFramaTime );

            while( offset >= this._bvhFormat.MotionFrames )
                offset -= this._bvhFormat.MotionFrames;    // ループ

            offset *= this._DataNumPerFrame;

            foreach( var node in this._bvhFormat.Root.Traverse() )
            {
                if( node.Channels.Length == 0 )
                    continue;

                Vector3 pos = Vector3.Zero;
                Vector3 rotv = Vector3.Zero;
                Quaternion rot = Quaternion.Identity;

                for( int i = 0; i < node.Channels.Length; i++ )
                {
                    float v = this._bvhFormat.MotionFrameDatas[ offset++ ];

                    switch( node.Channels[ i ] )
                    {
                        case BVHForat.ChannelType.Xposition:
                            pos.X += v;
                            break;

                        case BVHForat.ChannelType.Yposition:
                            pos.Y += v;
                            break;

                        case BVHForat.ChannelType.Zposition:
                            pos.Z += v;
                            break;

                        case BVHForat.ChannelType.Xrotation:
                            v = MathUtil.DegreesToRadians( v );
                            rot = Quaternion.RotationAxis( Vector3.Right, v ) * rot;
                            break;

                        case BVHForat.ChannelType.Yrotation:
                            v = MathUtil.DegreesToRadians( v );
                            rot = Quaternion.RotationAxis( Vector3.Up, -v ) * rot;
                            break;

                        case BVHForat.ChannelType.Zrotation:
                            v = MathUtil.DegreesToRadians( v );
                            rot = Quaternion.RotationAxis( Vector3.ForwardRH, -v ) * rot;
                            break;

                    }
                }

                rot = rot.ChangeHand();

                applyTo( node, pos, rot );
            }
        }



        protected BVHForat _bvhFormat;

        protected int _DataNumPerFrame;
    }
}
