using Newtonsoft.Json;

namespace InfluxDB.Net.Models
{
    public class Database
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}