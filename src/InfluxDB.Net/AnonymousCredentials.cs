using System.Net.Http;

namespace InfluxDB.Net
{
    public class AnonymousCredentials : Credentials
    {
        public AnonymousCredentials()
        {
        }

        public override HttpClient BuildHttpClient()
        {
            return new HttpClient();
        }

        public override bool IsTlsCredentials()
        {
            return false;
        }
    }
}