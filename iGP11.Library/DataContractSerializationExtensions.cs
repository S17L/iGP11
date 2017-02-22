using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace iGP11.Library
{
    public static class DataContractSerializationExtensions
    {
        public static TObject Clone<TObject>(this TObject @object)
        {
            return @object.Serialize().Deserialize<TObject>();
        }

        public static object Deserialize(this string @object, Type type)
        {
            if (@object.IsNullOrEmpty())
            {
                return null;
            }

            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(@object)))
            {
                return new DataContractJsonSerializer(type).ReadObject(stream);
            }
        }

        public static TObject Deserialize<TObject>(this string @object)
        {
            return (TObject)@object.Deserialize(typeof(TObject));
        }

        public static string Serialize(this object @object)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            using (var stream = new MemoryStream())
            {
                new DataContractJsonSerializer(@object.GetType()).WriteObject(stream, @object);
                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}