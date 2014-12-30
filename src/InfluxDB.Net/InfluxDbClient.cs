using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    internal class InfluxDbClient : IInfluxDbClient
    {
        private const string USER_AGENT = "InfluxDb.Net";

        public const String U = "u";
        public const String P = "p";
        public const String Q = "q";
        public const String ID = "id";
        public const String NAME = "name";
        public const String DATABASE = "database";
        public const String TIME_PRECISION = "time_precision";

        private readonly string _url;
        private readonly InfluxDbClientConfiguration _configuration;
        private readonly string _username;
        private readonly string _password;

        public InfluxDbClient(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }
        public InfluxDbClient(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        private HttpClient GetHttpClient()
        {
            return this._configuration.Credentials.BuildHttpClient();
        }

        public Pong Ping()
        {
            Task<HttpResponseMessage> response = Request(HttpMethod.Get, "/ping", null, null, false);
            return response.Result.ReadAs<Pong>();
        }

        public Task<HttpResponseMessage> Version()
        {
            return Request(HttpMethod.Get, "/interfaces", null, null, false);
        }

        public Task<HttpResponseMessage> CreateDatabase(Database database)
        {
            return Request(HttpMethod.Post, "/db", database);
        }

        public Task<HttpResponseMessage> CreateDatabase(DatabaseConfiguration config)
        {
            return Request(HttpMethod.Post, string.Format("/cluster/database_configs/{0}", config.Name), config);
        }

        public Task<HttpResponseMessage> DeleteDatabase(string name)
        {
            return Request(HttpMethod.Delete, string.Format("/db/{0}", name));
        }

        public async Task<List<Database>> DescribeDatabases()
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, "/db");
            return response.ReadAs<List<Database>>();
        }

        public Task<HttpResponseMessage> Write(string name, Serie[] series, string timePrecision)
        {
            return Request(HttpMethod.Post, string.Format("/db/{0}/series", name), series, new Dictionary<string, string>
            {
                {TIME_PRECISION,timePrecision}
            });
        }

        public async Task<List<Serie>> Query(string name, string query, string timePrecision)
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, string.Format("/db/{0}/series", name), null, new Dictionary<string, string>
            {
                {Q, query},
                {TIME_PRECISION, timePrecision}
            });

            return response.ReadAs<List<Serie>>();
        }

        public Task<HttpResponseMessage> CreateClusterAdmin(User user)
        {
            return Request(HttpMethod.Post, "/cluster_admins", user);
        }

        public Task<HttpResponseMessage> DeleteClusterAdmin(string name)
        {
            return Request(HttpMethod.Delete, string.Format("/cluster_admins/{0}", name));
        }

        public async Task<List<User>> DescribeClusterAdmins()
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, "/cluster_admins");

            return response.ReadAs<List<User>>();
        }

        public Task<HttpResponseMessage> UpdateClusterAdmin(User user, string name)
        {
            return Request(HttpMethod.Post, "/cluster_admins", user, new Dictionary<string, string>
            {
                { NAME, name }
            });
        }

        public Task<HttpResponseMessage> CreateDatabaseUser(string database, User user)
        {
            return Request(HttpMethod.Post, string.Format("/db/{0}/users", database), user);
        }

        public Task<HttpResponseMessage> DeleteDatabaseUser(string database, string name)
        {
            return Request(HttpMethod.Delete, string.Format("/db/{0}/users/{1}", database, name));
        }

        public async Task<List<User>> DescribeDatabaseUsers(string database)
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, string.Format("/db/{0}/users", database));

            return response.ReadAs<List<User>>();
        }

        public Task<HttpResponseMessage> UpdateDatabaseUser(string database, User user, string name)
        {
            return Request(HttpMethod.Post, string.Format("/db/{0}/users/{1}", database, name), user);
        }

        public Task<HttpResponseMessage> AuthenticateDatabaseUser(string database, string user, string password)
        {
            return Request(HttpMethod.Get, string.Format("/db/{0}/authenticate", database));
        }

        public async Task<List<ContinuousQuery>> GetContinuousQueries(string database)
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, string.Format("/db/{0}/continuous_queries", database));

            return response.ReadAs<List<ContinuousQuery>>();
        }

        public Task<HttpResponseMessage> DeleteContinuousQuery(string database, int id)
        {
            return Request(HttpMethod.Delete, string.Format("/db/{0}/continuous_queries/{1}", database, id));
        }

        public Task<HttpResponseMessage> DeleteSeries(string database, string name)
        {
            return Request(HttpMethod.Delete, string.Format("/db/{0}/series/{1}", database, name));
        }

        public Task<HttpResponseMessage> ForceRaftCompaction()
        {
            return Request(HttpMethod.Post, "/raft/force_compaction");
        }

        public async Task<List<string>> Interfaces()
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, "/interfaces");

            return response.ReadAs<List<string>>();
        }

        public async Task<bool> Sync()
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, "/sync");

            return response.ReadAs<bool>();
        }

        public async Task<List<Server>> ListServers()
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, "/cluster/servers");

            return response.ReadAs<List<Server>>();
        }

        public Task<HttpResponseMessage> RemoveServers(int id)
        {
            return Request(HttpMethod.Delete, string.Format("/cluster/servers/{0}", id));
        }

        public Task<HttpResponseMessage> CreateShard(Shard shard)
        {
            return Request(HttpMethod.Post, "/cluster/shards", shard);
        }

        public async Task<Shards> GetShards()
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, "/cluster/shards");

            return response.ReadAs<Shards>();
        }

        public Task<HttpResponseMessage> DropShard(int id, Shard.Member servers)
        {
            return Request(HttpMethod.Delete, string.Format("/cluster/shards/{0}", id), servers);
        }

        public async Task<List<ShardSpace>> GetShardSpaces()
        {
            HttpResponseMessage response = await Request(HttpMethod.Get, "/cluster/shard_spaces");

            return response.ReadAs<List<ShardSpace>>();
        }

        public Task<HttpResponseMessage> DropShardSpace(string database, string name)
        {
            return Request(HttpMethod.Delete, string.Format("/cluster/shard_spaces/{0}/{1}", database, name));
        }

        public Task<HttpResponseMessage> CreateShardSpace(string database, ShardSpace shardSpace)
        {
            return Request(HttpMethod.Post, string.Format("/cluster/shard_spaces/{0}", database), shardSpace);
        }

        private async Task<HttpResponseMessage> Request(HttpMethod method, string path, object body = null, Dictionary<string, string> extraParams = null, bool includeAuthToQuery = true)
        {
            try
            {
                var uri = BuildUri(path, extraParams, includeAuthToQuery);

                HttpClient client = GetHttpClient();

                var request = PrepareRequest(method, body, uri);

                return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw new InfluxDbException(string.Format("An error occured while execute request. Path : {0} , Method : {1}", path, method), ex);
            }
        }

        private StringBuilder BuildUri(string path, Dictionary<string, string> extraParams, bool includeAuthToQuery)
        {
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", _url, path);

            if (includeAuthToQuery)
            {
                urlBuilder.AppendFormat("?{0}={1}&{2}={3}", U, HttpUtility.UrlEncode(_username), P, HttpUtility.UrlEncode(_password));
            }

            if (extraParams != null && extraParams.Count > 0)
            {
                List<string> keyValues = new List<string>(extraParams.Count);
                keyValues.AddRange(extraParams.Select(param => string.Format("{0}={1}", param.Key, param.Value)));
                urlBuilder.AppendFormat("{0}{1}", includeAuthToQuery ? "&" : "?", string.Join("&", keyValues));
            }

            return urlBuilder;
        }

        private static HttpRequestMessage PrepareRequest(HttpMethod method, object body, StringBuilder urlBuilder)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, urlBuilder.ToString());

            request.Headers.Add("User-Agent", USER_AGENT);

            request.Headers.Add("Accept", "application/json");

            if (body != null)
            {
                //TODO: Pass parameter
                var content = new JsonRequestContent(body, new JsonSerializer());
                HttpContent requestContent = content.GetContent();
                request.Content = requestContent;
            }

            return request;
        }
    }
}