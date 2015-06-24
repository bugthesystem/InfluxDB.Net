namespace InfluxDB.Net.Models
{
	public class ShardSpace
	{
		public string Name { get; set; }
		public string Database { get; set; }
		public string RetentionPolicy { get; set; }
		public string ShardDuration { get; set; }
		public string Regex { get; set; }
		public int ReplicationFactor { get; set; }
		public int Split { get; set; }
	}
}