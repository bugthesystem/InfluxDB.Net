using System;
using System.Linq;
using System.Text;
using RestSharp;
using InfluxDB.Net.Models;
using System.Collections.Generic;
using RestSharp.Extensions;
using RestSharp.Serializers;

namespace InfluxDB.Net.Core
{
    internal class InfluxDbClient : IInfluxDbClient
    {
        public const String U = "u";
        public const String P = "p";
        public const String Q = "q";
        public const String ID = "id";
        public const String NAME = "name";
        public const String DATABASE = "database";
        public const String TIME_PRECISION = "time_precision";

        private readonly string _url;
        private readonly string _username;
        private readonly string _password;

        public InfluxDbClient(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        public Pong Ping()
        {
            IRestResponse response = Request(Method.GET, "/ping", null, null, false);
            return response.ReadAs<Pong>();
        }

        public IRestResponse Version()
        {
            return Request(Method.GET, "/interfaces", null, null, false);
        }

        public IRestResponse CreateDatabase(Database database)
        {
            return Request(Method.POST, "/db", database);
        }

        public IRestResponse CreateDatabase(DatabaseConfiguration config)
        {
            return Request(Method.POST, string.Format("/cluster/database_configs/{0}", config.Name), config);
        }

        public IRestResponse DeleteDatabase(string name)
        {
            return Request(Method.DELETE, string.Format("/db/{0}", name));
        }

        public List<Database> DescribeDatabases()
        {
            IRestResponse response = Request(Method.GET, "/db");
            return response.ReadAs<List<Database>>();
        }

        public IRestResponse Write(string name, Serie[] series, string timePrecision)
        {
            return Request(Method.POST, string.Format("/db/{0}/series", name), series, new Dictionary<string, string>
            {
                {TIME_PRECISION,timePrecision}
            });
        }

        public List<Serie> Query(string name, string query, string timePrecision)
        {
            IRestResponse response = Request(Method.GET, string.Format("/db/{0}/series", name), null, new Dictionary<string, string>
            {
                {Q, query},
                {TIME_PRECISION, timePrecision}
            });

            return response.ReadAs<List<Serie>>();
        }

        public IRestResponse CreateClusterAdmin(User user)
        {
            return Request(Method.POST, "/cluster_admins", user);
        }

        public IRestResponse DeleteClusterAdmin(string name)
        {
            return Request(Method.DELETE, string.Format("/cluster_admins/{0}", name));
        }

        public List<User> DescribeClusterAdmins()
        {
            IRestResponse response = Request(Method.GET, "/cluster_admins");

            return response.ReadAs<List<User>>();
        }

        public IRestResponse UpdateClusterAdmin(User user, string name)
        {
            return Request(Method.POST, "/cluster_admins", user, new Dictionary<string, string>
            {
                { NAME, name }
            });
        }

        public IRestResponse CreateDatabaseUser(string database, User user)
        {
            return Request(Method.POST, string.Format("/db/{0}/users", database), user);
        }

        public IRestResponse DeleteDatabaseUser(string database, string name)
        {
            return Request(Method.DELETE, string.Format("/db/{0}/users/{1}", database, name));
        }

        public List<User> DescribeDatabaseUsers(string database)
        {
            IRestResponse response = Request(Method.GET, string.Format("/db/{0}/users", database));

            return response.ReadAs<List<User>>();
        }

        public IRestResponse UpdateDatabaseUser(string database, User user, string name)
        {
            return Request(Method.POST, string.Format("/db/{0}/users/{1}", database, name), user);
        }

        public IRestResponse AuthenticateDatabaseUser(string database, string user, string password)
        {
            return Request(Method.GET, string.Format("/db/{0}/authenticate", database));
        }

        public List<ContinuousQuery> GetContinuousQueries(string database)
        {
            IRestResponse response = Request(Method.GET, string.Format("/db/{0}/continuous_queries", database));

            return response.ReadAs<List<ContinuousQuery>>();
        }

        public IRestResponse DeleteContinuousQuery(string database, int id)
        {
            return Request(Method.DELETE, string.Format("/db/{0}/continuous_queries/{1}", database, id));
        }

        public IRestResponse DeleteSeries(string database, string name)
        {
            return Request(Method.DELETE, string.Format("/db/{0}/series/{1}", database, name));
        }

        public IRestResponse ForceRaftCompaction()
        {
            return Request(Method.POST, "/raft/force_compaction");
        }

        public List<string> Interfaces()
        {
            IRestResponse response = Request(Method.GET, "/interfaces");

            return response.ReadAs<List<string>>();
        }

        public bool Sync()
        {
            IRestResponse response = Request(Method.GET, "/sync");

            return response.ReadAs<bool>();
        }

        public List<Server> ListServers()
        {
            IRestResponse response = Request(Method.GET, "/cluster/servers");

            return response.ReadAs<List<Server>>();
        }

        public IRestResponse RemoveServers(int id)
        {
            return Request(Method.DELETE, string.Format("/cluster/servers/{0}", id));
        }

        public IRestResponse CreateShard(Shard shard)
        {
            return Request(Method.POST, "/cluster/shards", shard);
        }

        public Shards GetShards()
        {
            IRestResponse response = Request(Method.GET, "/cluster/shards");

            return response.ReadAs<Shards>();
        }

        public IRestResponse DropShard(int id, Shard.Member servers)
        {
            return Request(Method.DELETE, string.Format("/cluster/shards/{0}", id), servers);
        }

        public List<ShardSpace> GetShardSpaces()
        {
            IRestResponse response = Request(Method.GET, "/cluster/shard_spaces");

            return response.ReadAs<List<ShardSpace>>();
        }

        public IRestResponse DropShardSpace(string database, string name)
        {
            return Request(Method.DELETE, string.Format("/cluster/shard_spaces/{0}/{1}", database, name));
        }

        public IRestResponse CreateShardSpace(string database, ShardSpace shardSpace)
        {
            return Request(Method.POST, string.Format("/cluster/shard_spaces/{0}", database), shardSpace);
        }

        private IRestResponse Request(Method requestMethod, string path, object body = null, Dictionary<string, string> extraParams = null, bool includeAuthToQuery = true)
        {
            try
            {
                RestRequest request;
                IRestClient client = PrepareClient(requestMethod, path, body, extraParams, includeAuthToQuery, out request);
                return client.Execute(request);
            }
            catch (Exception ex)
            {
                throw new InfluxDbException(string.Format("An error occured while execute request. Path : {0} , Method : {1}", path, requestMethod), ex);
            }
        }

        private IRestClient PrepareClient(Method requestMethod, string path, object body, Dictionary<string, string> extraParams, bool includeAuthToQuery, out RestRequest request)
        {
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", _url, path);

            if (includeAuthToQuery)
            {
                urlBuilder.AppendFormat("?{0}={1}&{2}={3}", U, _username.UrlEncode(), P, _password.UrlEncode());
            }

            if (extraParams != null && extraParams.Count > 0)
            {
                List<string> keyValues = new List<string>(extraParams.Count);
                keyValues.AddRange(extraParams.Select(param => string.Format("{0}={1}", param.Key, param.Value)));
                urlBuilder.AppendFormat("{0}{1}", includeAuthToQuery ? "&" : "?", string.Join("&", keyValues));
            }

            var client = new RestClient(urlBuilder.ToString());

            request = new RestRequest
            {
                Method = requestMethod
            };
            request.AddHeader("Accept", "application/json");

            request.Parameters.Clear();

            if (body != null)
            {
                string jsonBody = new JsonSerializer().Serialize(body);
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            }

            return client;
        }
    }
}