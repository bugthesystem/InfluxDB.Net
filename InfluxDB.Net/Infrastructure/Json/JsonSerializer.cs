using Newtonsoft.Json;

namespace InfluxDB.Net.Infrastructure.Jsons
{
    /// <summary>
    ///     Facade for <see cref="Newtonsoft.Json.JsonConvert" />.
    /// </summary>
    internal class JsonSerializer
    {
        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string SerializeObject<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}