using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Net.Models;
using System.Diagnostics;

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
        private const string Db = "db";
        private const string Precision = "precision";

        private const string AlterRetentionPolicyStmt = "alter retention policy {0} on {1} {2} {3} {4} {5}";
        private const string CreateContinuousQueryStmt = "create continuous query {0} on {1} begin {2} end;";
        private const string CreateDatabaseStmt = "create database \"{0}\"";
        private const string CreateRetentionPolicyStmt = "create retention policy \"{0}\" on {1} {2} {3} {4} {5}";
        private const string CreateUserStmt = "create user {0} with password {1} {2}";
        private const string DropContinuousQueryStmt = "drop continuous query {0}";
        private const string DropDatabaseStmt = "drop database \"{0}\"";
        private const string DropMeasurementStmt = "drop measurement \"{0}\"";
        private const string DropRetentionPolicyStmt = "drop retention policy \"{0}\" on {1}";
        private const string DropSeriesStmt = "drop series from \"{0}\"";
        private const string DropUserStmt = "drop user {0}";
        private const string GrantAllStmt = "grant all to {0}";
        private const string GrantStmt = "grant {0} on {1} to {2}";
        private const string ShowContinuousQueriesStmt = "show continuous queries";
        private const string ShowDatabasesStmt = "show databases";
        private const string ShowFieldKeysStmt = "show field keys {0} {1}";
        private const string ShowMeasurementsStmt = "show measurements";
        private const string ShowRetentionPolicies = "show retention policies {0}";
        private const string ShowSeriesStmt = "show series";
        private const string ShowTagKeysStmt = "show tag keys";
        private const string ShowTagValuesStmt = "show tag values";
        private const string ShowUsersStmt = "show users";
        private const string RevokeAllStmt = "revoke all privleges from {0}";
        private const string RevokeStmt = "revoke {0} on {1} from {2}";

        private readonly InfluxDbClientConfiguration _configuration;

        private readonly HttpClient _httpClient;

        private readonly ApiResponseErrorHandlingDelegate _defaultErrorHandlingDelegate = (statusCode, body) =>
        {
            if (statusCode < HttpStatusCode.OK || statusCode >= HttpStatusCode.BadRequest)
            {
                Debug.WriteLine(string.Format("[Error] {0} {1}", statusCode, body));
                throw new InfluxDbApiException(statusCode, body);
            }
        };

        public InfluxDbClient(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = _httpClient ?? (_httpClient = _configuration.BuildHttpClient());
        }

        /// <summary>Alters the retention policy.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="policyName">Name of the policy.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="replication">The replication factor.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
                    string policyName, string dbName, string duration, int replication)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
                new Dictionary<string, string>
                {
                    {Q, string.Format(AlterRetentionPolicyStmt, policyName, dbName, duration, replication) }
                });
        }

        /// <summary>Pings the server.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "ping", null, null, false, true);
        }

        /// <summary>Creates the database.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            Database database)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
                new Dictionary<string, string> { { Q, string.Format(CreateDatabaseStmt, database.Name) } });
        }

        /// <summary>Drops the database.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> DropDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
                new Dictionary<string, string> { { Q, string.Format(DropDatabaseStmt, name) } });
        }

        /// <summary>Queries the list of databases.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> ShowDatabases(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
                new Dictionary<string, string> { { Q, ShowDatabasesStmt } });
        }

        /// <summary>Writes the request to the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="request">The request.</param>
        /// <param name="timePrecision">The time precision.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiWriteResponse> Write(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            WriteRequest request, string timePrecision)
        {
            var content = new StringContent(request.GetLines(), Encoding.UTF8, "text/plain");
            var result = await RequestAsync(errorHandlers, HttpMethod.Post, "write", content,
                new Dictionary<string, string>
                {
                    { Db, request.Database},
                    { Precision, timePrecision }
                }, true, false);

            return new InfluxDbApiWriteResponse(result.StatusCode, result.Body);
        }

        /// <summary>Queries the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="name">The name.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
             string name, string query)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
                new Dictionary<string, string>
                {
                    {Db, name},
                    {Q, query}
                });
        }

        public Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DescribeClusterAdmins(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DescribeDatabaseUsers(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> AuthenticateDatabaseUser(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> GetContinuousQueries(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DeleteContinuousQuery(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<InfluxDbApiResponse> DropSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
                new Dictionary<string, string>
                {
                    { Db, database },
                    { Q, string.Format(DropSeriesStmt, name) }
                });
        }

        public Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            int id)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> CreateShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            Shard shard)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> GetShards(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DropShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            int id, Shard.Member servers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> GetShardSpaces(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, ShardSpace shardSpace)
        {
            throw new NotImplementedException();
        }

        private async Task<InfluxDbApiResponse> RequestAsync(
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, HttpMethod method, string path,
             HttpContent content = null,
             Dictionary<string, string> extraParams = null,
             bool includeAuthToQuery = true,
             bool headerIsBody = false)
        {
            var response = await RequestInnerAsync(null,
                HttpCompletionOption.ResponseHeadersRead,
                CancellationToken.None,
                method,
                path,
                content,
                extraParams,
                includeAuthToQuery);

            string responseContent = string.Empty;

            if (!headerIsBody)
            {
                responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                IEnumerable<string> values;

                if (response.Headers.TryGetValues("X-Influxdb-Version", out values))
                {
                    responseContent = values.First();
                }
            }

            HandleIfErrorResponse(response.StatusCode, responseContent, errorHandlers);

            Debug.WriteLine("[Response] {0}", response.ToJson());
            Debug.WriteLine("[ResponseData] {0}", responseContent);

            return new InfluxDbApiResponse(response.StatusCode, responseContent);
        }

        private async Task<HttpResponseMessage> RequestInnerAsync(TimeSpan? requestTimeout,
            HttpCompletionOption completionOption, CancellationToken cancellationToken, HttpMethod method, string path,
             HttpContent content = null, Dictionary<string, string> extraParams = null, bool includeAuthToQuery = true)
        {

            if (requestTimeout.HasValue)
            {
                _httpClient.Timeout = requestTimeout.Value;
            }

            StringBuilder uri = BuildUri(path, extraParams, includeAuthToQuery);
            HttpRequestMessage request = PrepareRequest(method, content, uri);

            Debug.WriteLine("[Request] {0}", request.ToJson());
            if (content != null)
            {
                Debug.WriteLine("[RequestData] {0}", content.ReadAsStringAsync().Result);
            }

            return await _httpClient.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
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

        private static HttpRequestMessage PrepareRequest(HttpMethod method, HttpContent content, StringBuilder urlBuilder)
        {
            var request = new HttpRequestMessage(method, urlBuilder.ToString());
            request.Headers.Add("User-Agent", UserAgent);
            request.Headers.Add("Accept", "application/json");

            request.Content = content;

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