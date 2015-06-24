using System.Collections.Generic;

namespace InfluxDB.Net.Models
{
	public class Shards
	{
		public List<Shard> LongTerm { get; set; }
		public List<Shard> ShortTerm { get; set; }
	}
}