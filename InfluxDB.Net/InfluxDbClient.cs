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
    internal class InfluxDbClient : IInfluxDbClient
    {
        private const string UserAgent = "InfluxDb.Net";

        private const string U = "u";
        private const string P = "p";
        private const string Q = "q";
        private const string Id = "id";
        private const string Name = "name";
        private const string Database = "database";
        private const string TimePrecision = "time_precision";

        private readonly InfluxDbClientConfiguration _configuration;

        private readonly ApiResponseErrorHandlingDelegate _defaultErrorHandlingDelegate = (statusCode, body) =>
        {
            if (statusCode < HttpStatusCode.OK || statusCode >= HttpStatusCode.BadRequest)
            {
                throw new InfluxDbApiException(statusCode, body);
            }
        };

        public InfluxDbClient(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "ping", null, null, false);
        }

        public Task<InfluxDbApiResponse> Version(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return RequestAsync(errorHandlers, HttpMethod.Get, "interfaces", null, null, false, true);
        }

        public Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            Database database)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, "db", database);
        }

        public Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            DatabaseConfiguration config)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post,
                string.Format("cluster/database_configs/{0}", config.Name), config);
        }

        public Task<InfluxDbApiResponse> DeleteDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name)
        {
            return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("db/{0}", name));
        }

        public async Task<InfluxDbApiResponse> DescribeDatabases(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "db");
        }

        public Task<InfluxDbApiResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name,
            Serie[] series, string timePrecision)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("db/{0}/series", name), series,
                new Dictionary<string, string>
                {
                    {TimePrecision, timePrecision}
                });
        }

        public async Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name, string query, string timePrecision)
        {
            return
                await
                    RequestAsync(errorHandlers, HttpMethod.Get, string.Format("db/{0}/series", name), null,
                        new Dictionary<string, string>
                        {
                            {Q, query},
                            {TimePrecision, timePrecision}
                        });
        }

        public Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, "cluster_admins", user);
        }

        public Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name)
        {
            return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("cluster_admins/{0}", name));
        }

        public async Task<InfluxDbApiResponse> DescribeClusterAdmins(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster_admins");
        }

        public Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user, string name)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, "cluster_admins", user, new Dictionary<string, string>
            {
                {Name, name}
            });
        }

        public Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("db/{0}/users", database), user);
        }

        public Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("db/{0}/users/{1}", database, name));
        }

        public async Task<InfluxDbApiResponse> DescribeDatabaseUsers(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, string.Format("db/{0}/users", database));
        }

        public Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user, string name)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("db/{0}/users/{1}", database, name), user);
        }

        public Task<InfluxDbApiResponse> AuthenticateDatabaseUser(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string user, string password)
        {
            return RequestAsync(errorHandlers, HttpMethod.Get, string.Format("db/{0}/authenticate", database));
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            return
                await RequestAsync(errorHandlers, HttpMethod.Get, string.Format("db/{0}/continuous_queries", database));
        }

        public Task<InfluxDbApiResponse> DeleteContinuousQuery(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, int id)
        {
            return RequestAsync(errorHandlers, HttpMethod.Delete,
                string.Format("db/{0}/continuous_queries/{1}", database, id));
        }

        public Task<InfluxDbApiResponse> DeleteSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("db/{0}/series/{1}", database, name));
        }

        public Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, "raft/force_compaction");
        }

        public async Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "interfaces");
        }

        public async Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "sync");
        }

        public async Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster/servers");
        }

        public Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            int id)
        {
            return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("cluster/servers/{0}", id));
        }

        public Task<InfluxDbApiResponse> CreateShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            Shard shard)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, "cluster/shards", shard);
        }

        public async Task<InfluxDbApiResponse> GetShards(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster/shards");
        }

        public async Task<InfluxDbApiResponse> DropShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            int id, Shard.Member servers)
        {
            return
                await RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("cluster/shards/{0}", id), servers);
        }

        public async Task<InfluxDbApiResponse> GetShardSpaces(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster/shard_spaces");
        }

        public Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            return RequestAsync(errorHandlers, HttpMethod.Delete,
                string.Format("cluster/shard_spaces/{0}/{1}", database, name));
        }

        public Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, ShardSpace shardSpace)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("cluster/shard_spaces/{0}", database),
                shardSpace);
        }

        private HttpClient GetHttpClient()
        {
            return _configuration.BuildHttpClient();
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

    internal delegate void ApiResponseErrorHandlingDelegate(HttpStatusCode statusCode, string responseBody);
}