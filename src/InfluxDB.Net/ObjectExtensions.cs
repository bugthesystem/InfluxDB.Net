using Newtonsoft.Json;

namespace InfluxDB.Net
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        public static T ReadAs<T>(this InfluxDbApiResponse response)
        {
            T o = JsonConvert.DeserializeObject<T>(response.Body);
            return o;
        }
    } 
}