namespace InfluxDB.Net.Core
{
    public class InfluxDbFactory
    {
        public static IInfluxDb Connect(string url, string username, string password)
        {
            Check.NotNullOrEmpty(url, "The URL may not be null or empty.");
            Check.NotNullOrEmpty(username, "The username may not be null or empty.");
            return new InfluxDb(url, username, password);
        }
    }
}