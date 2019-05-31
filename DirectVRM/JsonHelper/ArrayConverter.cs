using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DirectVRM.JsonHelper
{
    /// <summary>
    ///     glTFでサポートできる型の配列をデシリアライズする。
    /// </summary>
    /// <seealso cref="https://github.com/KhronosGroup/glTF-CSharp-Loader/blob/master/glTFLoader/ArrayConverter.cs"/>
    class ArrayConverter : JsonConverter
    {
        private static bool IsEnum( Type t )
        {
            return t.GetType().IsEnum;
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            if( objectType == typeof( bool[] ) ) return ReadImpl<bool>( reader );
            if( objectType == typeof( int[] ) ) return ReadImpl<long>( reader ).Select( ( v ) => (int)v ).ToArray();
            if( objectType == typeof( string[] ) ) return ReadImpl<string>( reader );
            if( objectType == typeof( float[] ) ) return ReadFloats( reader );
            if( objectType == typeof( object[] ) ) return ReadImpl<object>( reader );

            // enum の配列である場合、JSONから取得した数値を enum に変換して返す。
            if( objectType.IsArray && IsEnum( objectType.GetElementType() ) )
            {
                var elementType = objectType.GetElementType();
                var rawValues = ReadImpl<long>( reader ).Select( ( v ) => (int)v ).ToArray();   // enum の値は 整数 であるとする

                var resultArray = Array.CreateInstance( elementType, rawValues.Length );

                for( int i = 0; i < rawValues.Length; ++i )
                {
                    var enumerator = Enum.GetValues( elementType ).GetEnumerator();
                    do
                    {
                        enumerator.MoveNext();
                    } while( (int)enumerator.Current != rawValues[ i ] );
                    resultArray.SetValue( enumerator.Current, i );
                }

                return resultArray;
            }

            throw new NotImplementedException();
        }

        private t_array[] ReadImpl<t_array>( JsonReader reader )
        {
            // StartArray じゃないなら Value をそのまま配列として返す。
            if( reader.TokenType != JsonToken.StartArray )
            {
                return new[] { (t_array)reader.Value };
            }

            // StartArray だったなら、EndArray が来るまで values リストに追加する。
            reader.Read();
            var values = new List<t_array>();
            while( reader.TokenType != JsonToken.EndArray )
            {
                values.Add( (t_array)reader.Value );
                reader.Read();
            }

            // values リストを配列化して返す。
            return values.ToArray();
        }

        private object ReadFloats( JsonReader reader )
        {
            if( reader.TokenType != JsonToken.StartArray )
            {
                return new[] { SingleValueToFloat( reader.TokenType, reader.Value ) };
            }

            reader.Read();
            var values = new List<float>();
            while( reader.TokenType != JsonToken.EndArray )
            {
                values.Add( SingleValueToFloat( reader.TokenType, reader.Value ) );
                reader.Read();
            }

            return values.ToArray();
        }

        private float SingleValueToFloat( JsonToken tokenType, object value )
        {
            switch( tokenType )
            {
                case JsonToken.Integer:
                    return (long)value;
                case JsonToken.Float:
                    return (float)(double)value;
                default:
                    throw new NotImplementedException( tokenType.ToString() );
            }
        }

        public override bool CanConvert( Type type )
        {
            return true;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson( JsonWriter w, object o, JsonSerializer s )
        {
            throw new NotImplementedException();
        }
    }
}
