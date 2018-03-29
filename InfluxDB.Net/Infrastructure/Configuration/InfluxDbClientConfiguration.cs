using System;
using System.Net.Http;
using InfluxDB.Net.Infrastructure.Validation;
using InfluxDB.Net.Enums;

namespace InfluxDB.Net.Infrastructure.Configuration
{
    public class InfluxDbClientConfiguration
    {
        public Uri EndpointBaseUri { get; internal set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public InfluxVersion InfluxVersion { get; private set; }

        public TimeSpan? RequestTimeout { get; private set; }

        private readonly HttpClient _httpClient;

        public InfluxDbClientConfiguration(Uri endpoint, string username, string password, InfluxVersion influxVersion, TimeSpan? requestTimeout)
        {
            Validate.NotNull(endpoint, "Endpoint may not be null or empty.");
            Validate.NotNullOrEmpty(password, "Password may not be null or empty.");
            Validate.NotNullOrEmpty(username, "Username may not be null or empty.");
            Username = username;
            Password = password;
            InfluxVersion = influxVersion;
            EndpointBaseUri = SanitizeEndpoint(endpoint, false);
            RequestTimeout = requestTimeout;
            _httpClient = new HttpClient();
        }

        private static Uri SanitizeEndpoint(Uri endpoint, bool isTls)
        {
            var builder = new UriBuilder(endpoint);

            if (isTls)
            {
                builder.Scheme = "https";
            }
            else if (builder.Scheme.Equals("tcp", StringComparison.CurrentCultureIgnoreCase))
            //InvariantCultureIgnoreCase, not supported in PCL
            {
                builder.Scheme = "http";
            }

            return builder.Uri;
        }

        public HttpClient BuildHttpClient()
        {
            return _httpClient;
        }
    }
}