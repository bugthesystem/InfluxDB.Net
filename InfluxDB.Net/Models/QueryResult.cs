namespace InfluxDB.Net.Models
{
	public class QueryResult
	{
		public Result[] Results { get; set; }
	}

	public class Result
	{
		public Serie[] Series { get; set; }
	}
}