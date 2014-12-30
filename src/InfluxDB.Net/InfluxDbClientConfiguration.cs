using System;

namespace InfluxDB.Net
{
    public class InfluxDbClientConfiguration
    {
        public Uri EndpointBaseUri { get; internal set; }

        public Credentials Credentials { get; internal set; }

        public InfluxDbClientConfiguration(Uri endpoint)
            : this(endpoint, new AnonymousCredentials())
        {
        }

        public InfluxDbClientConfiguration(Uri endpoint, Credentials credentials)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }

            if (credentials == null)
            {
                throw new ArgumentNullException("credentials");
            }

            EndpointBaseUri = SanitizeEndpoint(endpoint, credentials.IsTlsCredentials());
            Credentials = credentials;
        }

        internal IInfluxDbClient CreateClient()
        {
            return new InfluxDbClient(this);
        }

        private static Uri SanitizeEndpoint(Uri endpoint, bool isTls)
        {
            UriBuilder builder = new UriBuilder(endpoint);

            if (isTls)
            {
                builder.Scheme = "https";
            }
            else if (builder.Scheme.Equals("tcp", StringComparison.CurrentCultureIgnoreCase)) //InvariantCultureIgnoreCase, not supported in PCL
            {
                builder.Scheme = "http";
            }

            return builder.Uri;
        }
    }
}