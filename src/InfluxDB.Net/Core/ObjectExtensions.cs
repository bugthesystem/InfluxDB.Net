using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace InfluxDB.Net.Core
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object @object)
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Serialize(@object);
        }

        public static T ReadAs<T>(this IRestResponse response)
        {
            JsonDeserializer serializer = new JsonDeserializer();
            return serializer.Deserialize<T>(response);
        }
    }
}