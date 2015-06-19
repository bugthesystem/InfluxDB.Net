using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    internal class InfluxDbClientV09 : IInfluxDbClient
    {
        private const string UserAgent = "InfluxDb.Net";

        private const string U = "u";
        private const string P = "p";
        private const string Q = "q";
        private const string Id = "id";
        private const string Name = "name";
        private const string Database = "db";
        private const string TimePrecision = "time_precision";

        private readonly InfluxDbClientConfiguration _configuration;

        private readonly ApiResponseErrorHandlingDelegate _defaultErrorHandlingDelegate = (statusCode, body) =>
        {
            if (statusCode < HttpStatusCode.OK || statusCode >= HttpStatusCode.BadRequest)
            {
                throw new InfluxDbApiException(statusCode, body);
            }
        };

        public InfluxDbClientV09(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "ping", null, null, 
                includeAuthToQuery:false, headerIsBody:true, contentType: ContentType.Deflate);
        }

        public Task<InfluxDbApiResponse> Version(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support db version");
            //return RequestAsync(errorHandlers, HttpMethod.Get, "interfaces", null, null, false, true);
        }

        public Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            Database database)
        {
            return Query(errorHandlers, "", string.Format("CREATE DATABASE {0}", database.Name));
        }

        public Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            DatabaseConfiguration config)
        {
            //return RequestAsync(errorHandlers, HttpMethod.Post,
            //    string.Format("cluster/database_configs/{0}", config.Name), config);
            throw new NotImplementedException("0.9 Currently doesn't support this");
        }

        public Task<InfluxDbApiResponse> DeleteDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name)
        {
            return Query(errorHandlers, "", string.Format("DROP DATABASE {0}", name));
        }

        public async Task<InfluxDbApiResponse> DescribeDatabases(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, "db");
        }

        public Task<InfluxDbApiResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name,
            Serie[] series, string timePrecision)
        {
            return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("write"), series,
                new Dictionary<string, string>
                {
                    {Database, name},
                    {TimePrecision, timePrecision}
                }, contentType:ContentType.Deflate);
        }

        public async Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name, string query, string timePrecision="")
        {
            var extraParams = new Dictionary<string, string>
            {
                {Q, query},
            };
            if(!string.IsNullOrEmpty(name))
                extraParams.Add(Database, name);
            if(!string.IsNullOrEmpty(timePrecision))
                extraParams.Add(TimePrecision, timePrecision);
            return
                await
                    RequestAsync(errorHandlers, HttpMethod.Get, string.Format("query"), null,
                        extraParams);
        }

        public Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Post, "cluster_admins", user);
        }

        public Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("cluster_admins/{0}", name));
        }

        public async Task<InfluxDbApiResponse> DescribeClusterAdmins(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster_admins");
        }

        public Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user, string name)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Post, "cluster_admins", user, new Dictionary<string, string>
            //{
            //    {Name, name}
            //});
        }

        public Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("db/{0}/users", database), user);
        }

        public Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("db/{0}/users/{1}", database, name));
        }

        public async Task<InfluxDbApiResponse> DescribeDatabaseUsers(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, string.Format("db/{0}/users", database));
        }

        public Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user, string name)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("db/{0}/users/{1}", database, name), user);
        }

        public Task<InfluxDbApiResponse> AuthenticateDatabaseUser(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string user, string password)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Get, string.Format("db/{0}/authenticate", database));
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return
            //    await RequestAsync(errorHandlers, HttpMethod.Get, string.Format("db/{0}/continuous_queries", database));
        }

        public Task<InfluxDbApiResponse> DeleteContinuousQuery(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, int id)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Delete,
            //    string.Format("db/{0}/continuous_queries/{1}", database, id));
        }

        public Task<InfluxDbApiResponse> DeleteSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("db/{0}/series/{1}", database, name));
        }

        public Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Post, "raft/force_compaction");
        }

        public async Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, "interfaces");
        }

        public async Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, "sync");
        }

        public async Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster/servers");
        }

        public Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            int id)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("cluster/servers/{0}", id));
        }

        public Task<InfluxDbApiResponse> CreateShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            Shard shard)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Post, "cluster/shards", shard);
        }

        public async Task<InfluxDbApiResponse> GetShards(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster/shards");
        }

        public async Task<InfluxDbApiResponse> DropShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            int id, Shard.Member servers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return
            //    await RequestAsync(errorHandlers, HttpMethod.Delete, string.Format("cluster/shards/{0}", id), servers);
        }

        public async Task<InfluxDbApiResponse> GetShardSpaces(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return await RequestAsync(errorHandlers, HttpMethod.Get, "cluster/shard_spaces");
        }

        public Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Delete,
            //    string.Format("cluster/shard_spaces/{0}/{1}", database, name));
        }

        public Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, ShardSpace shardSpace)
        {
            throw new NotImplementedException("0.9 Currently doesn't support this");
            //return RequestAsync(errorHandlers, HttpMethod.Post, string.Format("cluster/shard_spaces/{0}", database),
            //    shardSpace);
        }

        private HttpClient GetHttpClient()
        {
            return _configuration.BuildHttpClient();
        }

        private async Task<InfluxDbApiResponse> RequestAsync(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, HttpMethod method, string path,
            object data = null,
            Dictionary<string, string> extraParams = null, bool includeAuthToQuery = true, bool headerIsBody = false,
            ContentType contentType= ContentType.Json)
        {
            HttpResponseMessage response =
                await
                    RequestInnerAsync(null, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None, method,
                        path, data, extraParams, includeAuthToQuery, contentType);
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
            object data = null, Dictionary<string, string> extraParams = null, bool includeAuthToQuery = true, ContentType contentType= ContentType.Json)
        {
            HttpClient client = GetHttpClient();

            if (requestTimeout.HasValue)
            {
                client.Timeout = requestTimeout.Value;
            }

            StringBuilder uri = BuildUri(path, extraParams, includeAuthToQuery);
            HttpRequestMessage request = PrepareRequest(method, data, uri, contentType);

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

        private static HttpRequestMessage PrepareRequest(HttpMethod method, object body, StringBuilder urlBuilder, ContentType bodyType)
        {
            var request = new HttpRequestMessage(method, urlBuilder.ToString());
            request.Headers.Add("User-Agent", UserAgent);
            switch (bodyType)
            {
                    case ContentType.Json:
                        request.Headers.Add("Accept", "application/json");
                        if (body != null)
                        {
                            var content = new JsonRequestContent(body, new JsonSerializer());
                            HttpContent requestContent = content.GetContent();
                            request.Content = requestContent;
                        }
                    break;
                    case ContentType.Deflate:
                        request.Headers.Add("Accept-Encoding", "deflate");
                        if (body != null)
                        {
                            var bodyString = new StringBuilder();
                            if (body is IEnumerable)
                            {
                                var enumerator = ((IEnumerable) body).GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    bodyString.Append(enumerator.Current.ToString()+"\n");
                                }
                                bodyString = bodyString.Remove(bodyString.Length - 1, 1);
                            }
                            else
                            {
                                bodyString.Append(body.ToString());
                            }
                            HttpContent requestContent = new StringContent(bodyString.ToString());
                            request.Content = requestContent;
                        }
                    break;
                    case ContentType.Gzip:
                        request.Headers.Add("Accept-Encoding", "gzip");
                        if (body != null)
                        {
                            using (var stringStream = new MemoryStream(Encoding.UTF8.GetBytes(body.ToString())))
                            {
                                GZipStream zipStream = new GZipStream(stringStream, CompressionMode.Compress);
                                HttpContent requestContent = new StreamContent(zipStream);
                                request.Content = requestContent;
                            }
                        }
                    break;
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