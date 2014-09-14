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
            Check.NotNullOrEmpty(url, "The URL may not be null or empty.");
            Check.NotNullOrEmpty(username, "The username may not be null or empty.");
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

        public InfluxDbResponse Write(string database, TimeUnit.Unit precision, params Serie[] series)
        {
            return new InfluxDbResponse(_influxDbClient.Write(database, series, ToTimePrecision(precision)));
        }

        public InfluxDbResponse WriteUdp(int port, TimeUnit precision, params Serie[] series)
        {
            throw new NotImplementedException("WriteUdp is not implemented yet, sorry.");
        }

        public List<Serie> Query(string database, string query, TimeUnit.Unit precision)
        {
            return _influxDbClient.Query(database, query, ToTimePrecision(precision));
        }

        public InfluxDbResponse CreateDatabase(string name)
        {
            var db = new Database { name = name };

            return new InfluxDbResponse(_influxDbClient.CreateDatabase(db));
        }

        public InfluxDbResponse CreateDatabase(DatabaseConfiguration config)
        {
            return new InfluxDbResponse(_influxDbClient.CreateDatabase(config.Name, config));
        }

        public InfluxDbResponse DeleteDatabase(string name)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteDatabase(name));
        }

        public List<Database> DescribeDatabases()
        {
            return _influxDbClient.DescribeDatabases();
        }

        public InfluxDbResponse CreateClusterAdmin(string username, string adminPassword)
        {
            var user = new User { Name = username, Password = adminPassword };
            return new InfluxDbResponse(_influxDbClient.CreateClusterAdmin(user));
        }

        public InfluxDbResponse DeleteClusterAdmin(string username)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteClusterAdmin(username));
        }

        public List<User> DescribeClusterAdmins()
        {
            return _influxDbClient.DescribeClusterAdmins();
        }

        public InfluxDbResponse UpdateClusterAdmin(string username, string password)
        {
            var user = new User { Name = username, Password = password };

            return new InfluxDbResponse(_influxDbClient.UpdateClusterAdmin(user, username));
        }

        public InfluxDbResponse CreateDatabaseUser(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            return new InfluxDbResponse(_influxDbClient.CreateDatabaseUser(database, user));
        }

        public InfluxDbResponse DeleteDatabaseUser(string database, string name)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteDatabaseUser(database, name));
        }

        public List<User> DescribeDatabaseUsers(string database)
        {
            return _influxDbClient.DescribeDatabaseUsers(database);
        }

        public InfluxDbResponse UpdateDatabaseUser(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            return new InfluxDbResponse(_influxDbClient.UpdateDatabaseUser(database, user, name));
        }

        public InfluxDbResponse AlterDatabasePrivilege(string database, string name, bool isAdmin, params string[] permissions)
        {
            var user = new User { Name = name, IsAdmin = isAdmin };
            user.SetPermissions(permissions);
            return new InfluxDbResponse(_influxDbClient.UpdateDatabaseUser(database, user, name));
        }

        public InfluxDbResponse AuthenticateDatabaseUser(string database, string user, string password)
        {
            return new InfluxDbResponse(_influxDbClient.AuthenticateDatabaseUser(database, user, password));
        }

        public List<ContinuousQuery> DescribeContinuousQueries(string database)
        {
            return _influxDbClient.GetContinuousQueries(database);
        }

        public InfluxDbResponse DeleteContinuousQuery(string database, int id)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteContinuousQuery(database, id));
        }

        public InfluxDbResponse DeleteSeries(string database, string serieName)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteSeries(database, serieName));
        }

        public InfluxDbResponse ForceRaftCompaction()
        {
            return new InfluxDbResponse(_influxDbClient.ForceRaftCompaction());
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

        public InfluxDbResponse RemoveServers(int id)
        {
            return new InfluxDbResponse(_influxDbClient.RemoveServers(id));
        }

        [Obsolete]
        public InfluxDbResponse CreateShard(Shard shard)
        {
            return new InfluxDbResponse(_influxDbClient.CreateShard(shard));
        }

        [Obsolete]
        public Shards GetShards()
        {
            return _influxDbClient.GetShards();
        }

        [Obsolete]
        public InfluxDbResponse DropShard(Shard shard)
        {
            return new InfluxDbResponse(_influxDbClient.DropShard(shard.Id, shard.Shards.First()));
        }

        public List<ShardSpace> GetShardSpaces()
        {
            return _influxDbClient.GetShardSpaces();
        }

        public InfluxDbResponse DropShardSpace(string database, string name)
        {
            return new InfluxDbResponse(_influxDbClient.DropShardSpace(database, name));
        }

        public InfluxDbResponse CreateShardSpace(string database, ShardSpace shardSpace)
        {
            return new InfluxDbResponse(_influxDbClient.CreateShardSpace(database, shardSpace));
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
                    throw new ArgumentException("time precision must be " + TimeUnit.Unit.Seconds + ", " + TimeUnit.Unit.Milliseconds + " or " + TimeUnit.Unit.Microseconds);
            }
        }
    }
}