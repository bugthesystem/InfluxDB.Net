using System.Net.Http;

namespace InfluxDB.Net
{
    public abstract class Credentials
    {
        public abstract HttpClient BuildHttpClient();

        public abstract bool IsTlsCredentials();
    }
}