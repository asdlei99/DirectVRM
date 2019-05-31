using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;

namespace DirectVRM
{
    public static class Extensions
    {
        /// <summary>
        ///     Matrix の Rotation 成分を取り出す。
        /// </summary>
        /// <seealso cref="https://forum.unity.com/threads/how-to-assign-matrix4x4-to-transform.121966/"/>
        public static Quaternion ExtractRotation( this Matrix matrix )
        {
            Vector3 forward;
            forward.X = matrix.M13; // m02 on Unity
            forward.Y = matrix.M23; // m12
            forward.Z = matrix.M33; // m22

            Vector3 upwards;
            upwards.X = matrix.M12; // m01
            upwards.Y = matrix.M22; // m11
            upwards.Z = matrix.M32; // m21

            return Quaternion.RotationLookAtRH( forward, upwards ); // 右手系
        }

        /// <summary>
        ///     ベクトル from からベクトル to への回転を返す。
        /// </summary>
        /// <seealso cref="http://www.opengl-tutorial.org/jp/intermediate-tutorials/tutorial-17-quaternions/"/>
        public static Quaternion ToRotation( this Vector3 from, Vector3 to )
        {
            const float ゼロとみなす最大値 = 0.001f;

            from = Vector3.Normalize( from );
            to = Vector3.Normalize( to );

            // ２つのベクトル間の角度は内積で得られる。
            // from と to は正規化済みなので、内積は1×1×cosθ = cosθとなる。
            float cosTheta = Vector3.Dot( from, to );

            // ２つのベクトルが平行である場合（正反対の方向を向いているか同じ方向を向いている場合）は、内積が -1 になる。
            if( cosTheta < -1 + ゼロとみなす最大値 )
            {
                // その場合、それらのベクトルに垂直なベクトルを外積で求める。
                Vector3 rotationAxis = Vector3.Cross( Vector3.Right, from ); // X軸との外積を算出してみる。
                if( rotationAxis.Length() < ゼロとみなす最大値 )             // X軸と平行（外積ゼロ）なら、
                    rotationAxis = Vector3.Cross( Vector3.Up, from );        // Y軸との外積を算出してみる。X軸とY軸が両方とも平行になることは（原点以外）ないので、必ずどちらかが有効になる。
                rotationAxis.Normalize();

                // そのベクトルに対して180度回転するQuaternionを返す。
                return Quaternion.RotationAxis( rotationAxis, MathUtil.DegreesToRadians( 180.0f ) );
            }
            else if( cosTheta > 1 - ゼロとみなす最大値 )
            {
                return Quaternion.Identity;
            }
            else
            {
                // 平行ではない場合は、それらのベクトルに垂直なベクトルは外積で求まる。
                Vector3 rotationAxis = Vector3.Cross( from, to );

                // (A)
                //double s = Math.Sqrt( ( 1.0 + cosTheta ) * 2.0 );   // 半角の公式により cos(conTheta/2) を得る
                //double invs = 1.0 / s;
                //return new Quaternion(
                //    x: (float)( rotationAxis.X * invs ),
                //    y: (float)( rotationAxis.Y * invs ),
                //    z: (float)( rotationAxis.Z * invs ),
                //    w: (float)( s * 0.5 ) );    // 右手系

                // (B)
                //double w = Math.Sqrt( ( 1.0 + cosTheta ) / 2.0 );   // 半角の公式により w = cos(conTheta/2) を得る
                //double d = 0.5 / w;
                //return new Quaternion(
                //    x: (float)( rotationAxis.X * d ),
                //    y: (float)( rotationAxis.Y * d ),
                //    z: (float)( rotationAxis.Z * d ),
                //    w: (float)( w ) );    // 右手系

                // (C)
                var q = new Quaternion( rotationAxis, 1 + cosTheta );
                return Quaternion.Normalize( q );
            }
        }

        /// <summary>
        ///     左手系／右手系回転を逆の手系に変換する。
        /// </summary>
        public static Quaternion ChangeHand( this Quaternion me )
        {
            return Quaternion.RotationAxis( me.Axis, -me.Angle );
        }

        /// <summary>
        ///     自身と子ノードを順番に列挙する。
        /// </summary>
        public static IEnumerable<glTFNode> Traverse( this glTFNode t )
        {
            // まず自分を返す
            yield return t;

            // 次に子を順番に返す
            foreach( var x in t.Children )
            {
                foreach( var y in x.Traverse() )
                {
                    yield return y;
                }
            }
        }
    }
}
