using System.Net;
using RestSharp;

namespace InfluxDB.Net.Core
{
    public class InfluxDbResponse
    {
        public IRestResponse Raw { get; private set; }

        public bool Success
        {
            get { return Raw.StatusCode == HttpStatusCode.OK; }
        }

        public InfluxDbResponse(IRestResponse response)
        {
            Raw = response;
        }
    }
}