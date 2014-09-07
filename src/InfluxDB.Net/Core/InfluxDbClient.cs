using System;
using RestSharp;
using InfluxDB.Net.Models;
using RestSharp.Serializers;
using System.Collections.Generic;

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
            IRestResponse response = ProcessRequest(Method.GET, "/ping", null, false);
            return response.ReadAs<Pong>();
        }

        //TODO: Wrap
        public IRestResponse Version()
        {
            return ProcessRequest(Method.GET, "/interfaces", null, false);
        }

        public string CreateDatabase(Database database)
        {
            string url = string.Format("{0}{1}", _url, "/db");

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new JsonSerializer()
            };

            request.AddParameter(U, _username);
            request.AddParameter(P, _password);


            request.AddBody(database);


            IRestResponse restResponse = client.Execute(request);
            return "";
        }

        public string CreateDatabase(string name, DatabaseConfiguration config)
        {
            string url = string.Format("{0}{1}", _url, "/cluster/database_configs/{name}");

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new JsonSerializer()
            };

            request.AddParameter(U, _username);
            request.AddParameter(P, _password);
            request.AddUrlSegment(NAME, name);

            request.AddBody(config);

            IRestResponse restResponse = client.Execute(request);

            return "";
        }

        public string DeleteDatabase(string name)
        {
            throw new NotImplementedException();
        }

        public List<Database> DescribeDatabases()
        {
            throw new NotImplementedException();
        }

        public string Write(string name, Serie[] series, string timePrecision)
        {
            throw new NotImplementedException();
        }

        public List<Serie> Query(string name, string query, string timePrecision)
        {
            throw new NotImplementedException();
        }

        public string CreateClusterAdmin(User user)
        {
            throw new NotImplementedException();
        }

        public string DeleteClusterAdmin(string name)
        {
            throw new NotImplementedException();
        }

        public List<User> DescribeClusterAdmins()
        {
            throw new NotImplementedException();
        }

        public string UpdateClusterAdmin(User user, string name)
        {
            throw new NotImplementedException();
        }

        public string CreateDatabaseUser(string database, User user)
        {
            throw new NotImplementedException();
        }

        public string DeleteDatabaseUser(string database, string name)
        {
            throw new NotImplementedException();
        }

        public List<User> DescribeDatabaseUsers(string database)
        {
            throw new NotImplementedException();
        }

        public string UpdateDatabaseUser(string database, User user, string name)
        {
            throw new NotImplementedException();
        }

        public string AuthenticateDatabaseUser(string database, string user, string password)
        {
            throw new NotImplementedException();
        }

        public List<ContinuousQuery> GetContinuousQueries(string database)
        {
            throw new NotImplementedException();
        }

        public string DeleteContinuousQuery(string database, int id)
        {
            throw new NotImplementedException();
        }

        public string DeleteSeries(string database, string name)
        {
            throw new NotImplementedException();
        }

        public string ForceRaftCompaction()
        {
            throw new NotImplementedException();
        }

        public List<string> Interfaces()
        {
            throw new NotImplementedException();
        }

        public bool Sync()
        {
            throw new NotImplementedException();
        }

        public List<Server> ListServers()
        {
            throw new NotImplementedException();
        }

        public string RemoveServers(int id)
        {
            throw new NotImplementedException();
        }

        public string CreateShard(Shard shard)
        {
            throw new NotImplementedException();
        }

        public Shards GetShards()
        {
            throw new NotImplementedException();
        }

        public string DropShard(int id, Shard.Member servers)
        {
            throw new NotImplementedException();
        }

        public List<ShardSpace> GetShardSpaces()
        {
            throw new NotImplementedException();
        }

        public string DropShardSpace(string database, string name)
        {
            throw new NotImplementedException();
        }

        public string CreateShardSpace(string database, ShardSpace shardSpace)
        {
            throw new NotImplementedException();
        }

        private IRestResponse ProcessRequest(Method requestMethod, string path, object data, bool includeAuthToQuery = true)
        {
            try
            {
                RestRequest request;
                IRestClient client = PrepareClient(requestMethod, path, data, includeAuthToQuery, out request);
                return client.Execute(request);
            }
            catch (Exception ex)
            {
                throw new InfluxDbException(string.Format("An error occured while execute request. Path : {0} , Method : {1}", path, requestMethod), ex);
            }
        }

        private IRestClient PrepareClient(Method requestMethod, string path, object data, bool includeAuthToQuery, out RestRequest request)
        {
            string url = string.Format("{0}{1}", _url, path);

            var client = new RestClient(url);
            request = new RestRequest(requestMethod)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new JsonSerializer()
            };

            if (includeAuthToQuery)
            {
                request.AddParameter(U, _username);
                request.AddParameter(P, _password);
            }

            if (data != null)
            {
                request.AddBody(data);
            }

            return client;
        }
    }
}