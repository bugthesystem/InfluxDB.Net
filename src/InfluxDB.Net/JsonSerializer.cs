using Newtonsoft.Json;

namespace InfluxDB.Net
{
    /// <summary>
    /// Facade for <see cref="Newtonsoft.Json.JsonConvert"/>.
    /// </summary>
    internal class JsonSerializer
    {
        private JsonConverter[] Converters { get; set; }

        public JsonSerializer()
        {
            Converters = new JsonConverter[]
            {
                new JsonIso8601AndUnixEpochDateConverter(),
                new JsonVersionConverter()
            };
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, this.Converters);
        }

        public string SerializeObject<T>(T value)
        {
            return JsonConvert.SerializeObject(value, this.Converters);
        }
    }
}