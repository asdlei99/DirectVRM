using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sprache;
using SharpDX;

namespace DirectVRM
{
    /// <summary>
    ///     bvhファイルの読み込み
    /// </summary>
    /// <seealso cref="https://qiita.com/ousttrue/items/0a970d1a1f13d98b686a"/>
    public class BVHForat
    {
        public enum ChannelType
        {
            Xposition,
            Yposition,
            Zposition,
            Zrotation,
            Yrotation,
            Xrotation,
        }

        public class BvhNode
        {
            public string Name { get; set; }

            public Vector3 Offset { get; set; }

            public ChannelType[] Channels { get; protected set; }

            public List<BvhNode> Children { get; protected set; }


            public BvhNode( string name, Vector3 offset, IEnumerable<ChannelType> channels = null, IEnumerable<BvhNode> children = null )
            {
                this.Name = name;
                this.Offset = offset;
                this.Channels = ( null != channels ) ? channels.ToArray() : new ChannelType[] { };
                this.Children = ( null != children ) ? children.ToList() : new List<BvhNode>();
            }

            public IEnumerable<BvhNode> Traverse()
            {
                yield return this;

                foreach( var child in this.Children )
                {
                    foreach( var n in child.Traverse() )
                    {
                        yield return n;
                    }
                }
            }
        }


        public BvhNode Root { get; protected set; }

        public int MotionFrames { get; protected set; }

        public float MotionFramaTime { get; protected set; }

        public float[] MotionFrameDatas { get; protected set; }


        public BVHForat()
        {
        }

        public BVHForat( string bvhText )
            : this()
        {
            this.ParseFrom( bvhText );
        }

        public void ParseFrom( string bvhText )
        {
            var all = from hierarchy in Hierarchy()
                      from motion in Motion()
                      select new { hierarchy, motion };

            var result = all.Parse( bvhText );

            this.Root = result.hierarchy;
            this.MotionFrames = result.motion.frames;
            this.MotionFramaTime = result.motion.frameTime;
            this.MotionFrameDatas = result.motion.values.ToArray();
        }


        protected static Parser<BvhNode> Hierarchy()
        {
            return from hierarchy in Parse.String( "HIERARCHY" ).Token()
                   from root in Node( "ROOT" )
                   select root;
        }

        protected static Parser<(int frames, float frameTime, IEnumerable<float> values)> Motion()
        {
            return from mortion in Parse.String( "MOTION" ).Token()
                   from framesLabel in Parse.String( "Frames" ).Token()
                   from _1 in Parse.Char( ':' ).Token()
                   from frames in Parse.Number.Select( x => Convert.ToInt32( x ) )
                   from frameTimeLavel in Parse.String( "Frame Time" ).Token()
                   from _2 in Parse.Char( ':' ).Token()
                   from frameTime in FloatEx().Token()
                   from values in FloatEx().Token().AtLeastOnce()
                   select (frames, frameTime, values);
        }

        protected static Parser<string> Exponent()
        {
            return from _ in Parse.Char( 'E' )
                   from sign in Parse.Chars( "+-" )
                   from num in Parse.Number
                   select string.Format( "E{0}{1}", sign, num );
        }

        protected static Parser<float> FloatEx()
        {
            return from negative in Parse.Char( '-' ).Optional().Select( x => x.IsDefined ? x.Get().ToString() : "" )
                   from num in Parse.Decimal
                   from exponent in Exponent().Optional().Select( x => x.IsDefined ? x.Get() : "" )
                   select Convert.ToSingle( negative + num + exponent );
        }

        protected static Parser<Vector3> Offset()
        {
            return from _ in Parse.String( "OFFSET" ).Token()
                   from x in FloatEx().Token().Select( x => Convert.ToSingle( x ) )
                   from y in FloatEx().Token().Select( x => Convert.ToSingle( x ) )
                   from z in FloatEx().Token().Select( x => Convert.ToSingle( x ) )
                   select new Vector3( x, y, z );
        }

        protected static Parser<IEnumerable<ChannelType>> Channels()
        {
            return from _ in Parse.String( "CHANNELS" ).Token()
                   from n in Parse.Number.Select( x => Convert.ToInt32( x ) )
                   from channels in Parse.String( "Xposition" ).Token().Return( ChannelType.Xposition )
                       .Or( Parse.String( "Yposition" ).Token().Return( ChannelType.Yposition ) )
                       .Or( Parse.String( "Zposition" ).Token().Return( ChannelType.Zposition ) )
                       .Or( Parse.String( "Xrotation" ).Token().Return( ChannelType.Xrotation ) )
                       .Or( Parse.String( "Yrotation" ).Token().Return( ChannelType.Yrotation ) )
                       .Or( Parse.String( "Zrotation" ).Token().Return( ChannelType.Zrotation ) )
                       .Repeat( n )
                   select channels;
        }

        protected static Parser<BvhNode> EndSite()
        {
            return from _ in Parse.String( "End" ).Token()
                   from name in Parse.LetterOrDigit.Many().Token().Text()
                   from open in Parse.Char( '{' ).Token()
                   from offset in Offset()
                   from close in Parse.Char( '}' ).Token()
                   select new BvhNode( name, offset );
        }

        protected static Parser<BvhNode> Node( string prefix )
        {
            return from type in Parse.String( prefix ).Token()
                   from name in Parse.LetterOrDigit.Many().Token().Text()
                   from begin in Parse.Char( '{' ).Token()
                   from offset in Offset()
                   from channels in Channels()
                   from children in Node( "JOINT" ).AtLeastOnce().Or( EndSite().Select( x => new BvhNode[] { x } ) )
                   from end in Parse.Char( '}' ).Token()
                   select new BvhNode( name, offset, channels, children );
        }
    }
}
