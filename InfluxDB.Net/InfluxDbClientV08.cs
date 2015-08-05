using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    internal class InfluxDbClientV08 : IInfluxDbClient
    {
        private const string UserAgent = "InfluxDb.Net";

        private const string U = "u";
        private const string P = "p";

        private readonly InfluxDbClientConfiguration _configuration;
        private readonly ApiResponseErrorHandlingDelegate _defaultErrorHandlingDelegate = (statusCode, body) =>
        {
            if (statusCode < HttpStatusCode.OK || statusCode >= HttpStatusCode.BadRequest)
            {
                throw new InfluxDbApiException(statusCode, body);
            }
        };

        public InfluxDbClientV08(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "ping", null, null, false);
        }

        public async Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, Database database)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Post, "db", database);
        }

        public async Task<InfluxDbApiResponse> DropDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("db/{0}", name));
        }

        public async Task<InfluxDbApiResponse> ShowDatabases(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiWriteResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, WriteRequest request, string timePrecision)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name, string query)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DescribeClusterAdmins(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, User user)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DescribeDatabaseUsers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, User user, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> AuthenticateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string user, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DropSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> CreateShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, Shard shard)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> GetShards(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DropShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id, Shard.Member servers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> GetShardSpaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, ShardSpace shardSpace)
        {
            throw new NotImplementedException();
        }

        private async Task<InfluxDbApiResponse> RequestAsync(
    IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, HttpMethod method, string path,
    object data = null,
    Dictionary<string, string> extraParams = null, bool includeAuthToQuery = true, bool headerIsBody = false)
        {
            HttpResponseMessage response =
                await
                    RequestInnerAsync(null, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None, method,
                        path, data, extraParams, includeAuthToQuery);
            string content = string.Empty;

            if (!headerIsBody)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                IEnumerable<string> values;

                if (response.Headers.TryGetValues("X-Influxdb-Version", out values))
                {
                    content = values.First();
                }
            }

            HandleIfErrorResponse(response.StatusCode, content, errorHandlers);

            return new InfluxDbApiResponse(response.StatusCode, content);
        }

        private async Task<HttpResponseMessage> RequestInnerAsync(TimeSpan? requestTimeout,
            HttpCompletionOption completionOption, CancellationToken cancellationToken, HttpMethod method, string path,
            object data = null, Dictionary<string, string> extraParams = null, bool includeAuthToQuery = true)
        {
            HttpClient client = GetHttpClient();

            if (requestTimeout.HasValue)
            {
                client.Timeout = requestTimeout.Value;
            }

            StringBuilder uri = BuildUri(path, extraParams, includeAuthToQuery);
            HttpRequestMessage request = PrepareRequest(method, data, uri);

            return await client.SendAsync(request, completionOption, cancellationToken);
        }

        private HttpClient GetHttpClient()
        {
            return _configuration.BuildHttpClient();
        }

        private StringBuilder BuildUri(string path, Dictionary<string, string> extraParams, bool includeAuthToQuery)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", _configuration.EndpointBaseUri, path);

            if (includeAuthToQuery)
            {
                urlBuilder.AppendFormat("?{0}={1}&{2}={3}", U, HttpUtility.UrlEncode(_configuration.Username), P,
                    HttpUtility.UrlEncode(_configuration.Password));
            }

            if (extraParams != null && extraParams.Count > 0)
            {
                var keyValues = new List<string>(extraParams.Count);
                keyValues.AddRange(extraParams.Select(param => string.Format("{0}={1}", param.Key, param.Value)));
                urlBuilder.AppendFormat("{0}{1}", includeAuthToQuery ? "&" : "?", string.Join("&", keyValues));
            }

            return urlBuilder;
        }

        private static HttpRequestMessage PrepareRequest(HttpMethod method, object body, StringBuilder urlBuilder)
        {
            var request = new HttpRequestMessage(method, urlBuilder.ToString());

            request.Headers.Add("User-Agent", UserAgent);

            request.Headers.Add("Accept", "application/json");

            if (body != null)
            {
                var content = new JsonRequestContent(body, new JsonSerializer());
                HttpContent requestContent = content.GetContent();
                request.Content = requestContent;
            }

            return request;
        }

        private void HandleIfErrorResponse(HttpStatusCode statusCode, string responseBody,
    IEnumerable<ApiResponseErrorHandlingDelegate> handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            foreach (ApiResponseErrorHandlingDelegate handler in handlers)
            {
                handler(statusCode, responseBody);
            }
            _defaultErrorHandlingDelegate(statusCode, responseBody);
        }
    }
}