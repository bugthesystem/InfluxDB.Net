using System;
using System.Net;

namespace InfluxDB.Net.Infrastructure.Influx
{
    public class InfluxDbException : Exception
    {
        public InfluxDbException(string message, Exception innerException)
             : base(message, innerException)
        {
        }

        public InfluxDbException(string message)
             : base(message)
        {
        }
    }

    public class InfluxDbApiException : InfluxDbException
    {
        public InfluxDbApiException(HttpStatusCode statusCode, string responseBody)
             : base(String.Format("InfluxDb API responded with status code={0}, response={1}", statusCode, responseBody))
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string ResponseBody { get; private set; }
    }
}