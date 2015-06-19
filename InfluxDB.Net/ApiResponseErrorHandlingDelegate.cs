using System.Net;

namespace InfluxDB.Net
{
    internal delegate void ApiResponseErrorHandlingDelegate(HttpStatusCode statusCode, string responseBody);
}