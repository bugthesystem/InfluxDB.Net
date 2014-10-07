using Newtonsoft.Json;
using RestSharp;
using JsonSerializer = RestSharp.Serializers.JsonSerializer;

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

            T deserialize = JsonConvert.DeserializeObject<T>(response.Content);
            return deserialize;
            //TODO: Fix RestSharp json deserializer
            //JsonDeserializer serializer = new JsonDeserializer();
            //return serializer.Deserialize<T>(response);
        }
    }
}