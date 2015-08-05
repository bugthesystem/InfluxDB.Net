using System;
using System.Net.Http;

namespace InfluxDB.Net
{
	public class InfluxDbClientConfiguration
	{
        public InfluxDbClientConfiguration(Uri endpoint)
            : this(endpoint, null, null, InfluxDbVersion.Auto)
		{
		}

        public InfluxDbClientConfiguration(Uri endpoint, string username, string password, InfluxDbVersion influxDbVersion)
		{
			Check.NotNull(endpoint, "Endpoint may not be null or empty.");
			Check.NotNullOrEmpty(password, "Password may not be null or empty.");
			Check.NotNullOrEmpty(username, "Username may not be null or empty.");
			Username = username;
			Password = password;
            InfluxDbVersion = influxDbVersion;
			EndpointBaseUri = SanitizeEndpoint(endpoint, false);
		}

		public Uri EndpointBaseUri { get; internal set; }

		public string Username { get; private set; }
		public string Password { get; private set; }
        public InfluxDbVersion InfluxDbVersion { get; private set; }

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
			return new HttpClient();
		}
	}
}