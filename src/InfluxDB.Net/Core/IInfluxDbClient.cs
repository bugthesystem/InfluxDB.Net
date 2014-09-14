using System;
using System.Collections.Generic;
using InfluxDB.Net.Models;
using RestSharp;

namespace InfluxDB.Net.Core
{
    public interface IInfluxDbClient
    {
        Pong Ping();

        IRestResponse Version();

        IRestResponse CreateDatabase(Database database);

        IRestResponse CreateDatabase(string name, DatabaseConfiguration config);

        IRestResponse DeleteDatabase(string name);

        List<Database> DescribeDatabases();

        IRestResponse Write(string name, Serie[] series, string timePrecision);

        List<Serie> Query(String name, String query, String timePrecision);

        IRestResponse CreateClusterAdmin(User user);

        IRestResponse DeleteClusterAdmin(string name);

        List<User> DescribeClusterAdmins();

        IRestResponse UpdateClusterAdmin(User user, string name);

        IRestResponse CreateDatabaseUser(string database, User user);

        IRestResponse DeleteDatabaseUser(string database, string name);

        List<User> DescribeDatabaseUsers(String database);

        IRestResponse UpdateDatabaseUser(string database, User user, string name);

        IRestResponse AuthenticateDatabaseUser(string database, string user, string password);

        List<ContinuousQuery> GetContinuousQueries(String database);

        IRestResponse DeleteContinuousQuery(string database, int id);

        IRestResponse DeleteSeries(string database, string name);

        IRestResponse ForceRaftCompaction();

        List<String> Interfaces();

        Boolean Sync();

        List<Server> ListServers();

        IRestResponse RemoveServers(int id);

        IRestResponse CreateShard(Shard shard);

        Shards GetShards();

        IRestResponse DropShard(int id, Shard.Member servers);

        List<ShardSpace> GetShardSpaces();

        IRestResponse DropShardSpace(string database, string name);

        IRestResponse CreateShardSpace(string database, ShardSpace shardSpace);

    }
}