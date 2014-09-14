using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InfluxDB.Net.Core;
using InfluxDB.Net.Models;
using RestSharp;

namespace InfluxDB.Net
{
    public class InfluxDb : IInfluxDb
    {
        private readonly IInfluxDbClient _influxDbClient;


        public InfluxDb(string url, string username, string password)
            : this(new InfluxDbClient(url, username, password))
        {
        }

        internal InfluxDb(IInfluxDbClient influxDbClient)
        {
            _influxDbClient = influxDbClient;
        }

        public Pong Ping()
        {
            var watch = new Stopwatch();
            watch.Start();

            Pong pong = _influxDbClient.Ping();
            pong.ResponseTime = watch.ElapsedMilliseconds;

            watch.Stop();

            return pong;
        }

        public string Version()
        {
            IRestResponse response = _influxDbClient.Version();
            String version = "unknown";
            IList<Parameter> headers = response.Headers;

            foreach (Parameter header in headers)
            {
                if (null != header.Name && header.Name.Equals("X-Influxdb-Version"))
                {
                    version = header.Value.ToString();
                }
            }

            return version;
        }

        public void Write(string database, TimeUnit.Unit precision, params Serie[] series)
        {
            _influxDbClient.Write(database, series, ToTimePrecision(precision));
        }

        public void WriteUdp(int port, TimeUnit precision, params Serie[] series)
        {
            throw new NotImplementedException("WriteUdp is not implemented yet, sorry.");
        }

        public List<Serie> Query(string database, string query, TimeUnit.Unit precision)
        {
            return _influxDbClient.Query(database, query, ToTimePrecision(precision));
        }

        public void CreateDatabase(string name)
        {
            var db = new Database { Name = name };
            _influxDbClient.CreateDatabase(db);
        }

        public void CreateDatabase(DatabaseConfiguration config)
        {
            _influxDbClient.CreateDatabase(config.Name, config);
        }

        public void DeleteDatabase(string name)
        {
            _influxDbClient.DeleteDatabase(name);
        }

        public List<Database> DescribeDatabases()
        {
            return _influxDbClient.DescribeDatabases();
        }

        public void CreateClusterAdmin(string username, string adminPassword)
        {
            var user = new User { Name = username, Password = adminPassword };
            _influxDbClient.CreateClusterAdmin(user);
        }

        public void DeleteClusterAdmin(string username)
        {
            _influxDbClient.DeleteClusterAdmin(username);
        }

        public List<User> DescribeClusterAdmins()
        {
            return _influxDbClient.DescribeClusterAdmins();
        }

        public void UpdateClusterAdmin(string username, string password)
        {
            var user = new User { Name = username, Password = password };

            _influxDbClient.UpdateClusterAdmin(user, username);
        }

        public void CreateDatabaseUser(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            _influxDbClient.CreateDatabaseUser(database, user);
        }

        public void DeleteDatabaseUser(string database, string name)
        {
            _influxDbClient.DeleteDatabaseUser(database, name);
        }

        public List<User> DescribeDatabaseUsers(string database)
        {
            return _influxDbClient.DescribeDatabaseUsers(database);
        }

        public void UpdateDatabaseUser(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            _influxDbClient.UpdateDatabaseUser(database, user, name);
        }

        public void AlterDatabasePrivilege(string database, string name, bool isAdmin, params string[] permissions)
        {
            var user = new User { Name = name, IsAdmin = isAdmin };
            user.SetPermissions(permissions);
            _influxDbClient.UpdateDatabaseUser(database, user, name);
        }

        public void AuthenticateDatabaseUser(string database, string user, string password)
        {
            _influxDbClient.AuthenticateDatabaseUser(database, user, password);
        }

        public List<ContinuousQuery> DescribeContinuousQueries(string database)
        {
            return _influxDbClient.GetContinuousQueries(database);
        }

        public void DeleteContinuousQuery(string database, int id)
        {
            _influxDbClient.DeleteContinuousQuery(database, id);
        }

        public void DeleteSeries(string database, string serieName)
        {
            _influxDbClient.DeleteSeries(database, serieName);
        }

        public void ForceRaftCompaction()
        {
            _influxDbClient.ForceRaftCompaction();
        }

        public List<string> Interfaces()
        {
            return _influxDbClient.Interfaces();
        }

        public bool Sync()
        {
            return _influxDbClient.Sync();
        }

        public List<Server> ListServers()
        {
            return _influxDbClient.ListServers();
        }

        public void RemoveServers(int id)
        {
            _influxDbClient.RemoveServers(id);
        }

        [Obsolete]
        public void CreateShard(Shard shard)
        {
            _influxDbClient.CreateShard(shard);
        }

        [Obsolete]
        public Shards GetShards()
        {
            return _influxDbClient.GetShards();
        }

        [Obsolete]
        public void DropShard(Shard shard)
        {
            _influxDbClient.DropShard(shard.Id, shard.Shards.First());
        }

        public List<ShardSpace> GetShardSpaces()
        {
            return _influxDbClient.GetShardSpaces();
        }

        public void DropShardSpace(string database, string name)
        {
            _influxDbClient.DropShardSpace(database, name);
        }

        public void CreateShardSpace(string database, ShardSpace shardSpace)
        {
            _influxDbClient.CreateShardSpace(database, shardSpace);
        }


        public IInfluxDb SetLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:

                    break;
                case LogLevel.Basic:

                    break;
                case LogLevel.Headers:

                    break;
                case LogLevel.Full:
                    break;
            }

            return this;
        }

        private static String ToTimePrecision(TimeUnit.Unit t)
        {
            switch (t)
            {
                case TimeUnit.Unit.Seconds:
                    return "s";
                case TimeUnit.Unit.Milliseconds:
                    return "ms";
                case TimeUnit.Unit.Microseconds:
                    return "u";
                default:
                    throw new ArgumentException("time precision must be " + TimeUnit.Unit.Seconds + ", " +
                                                TimeUnit.Unit.Milliseconds + " or " + TimeUnit.Unit.Microseconds);
            }
        }
    }
}