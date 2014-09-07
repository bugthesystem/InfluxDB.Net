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

        String CreateDatabase(Database database);

        String CreateDatabase(String name, DatabaseConfiguration config);

        String DeleteDatabase(String name);

        List<Database> DescribeDatabases();

        String Write(String name, Serie[] series, String timePrecision);

        List<Serie> Query(String name, String query, String timePrecision);

        String CreateClusterAdmin(User user);

        String DeleteClusterAdmin(String name);

        List<User> DescribeClusterAdmins();

        String UpdateClusterAdmin(User user, String name);

        String CreateDatabaseUser(String database, User user);

        String DeleteDatabaseUser(String database, String name);

        List<User> DescribeDatabaseUsers(String database);

        String UpdateDatabaseUser(String database, User user, String name);

        String AuthenticateDatabaseUser(String database, string user, string password);

        List<ContinuousQuery> GetContinuousQueries(String database);

        String DeleteContinuousQuery(String database, int id);

        String DeleteSeries(String database, String name);

        String ForceRaftCompaction();

        List<String> Interfaces();

        Boolean Sync();

        List<Server> ListServers();

        String RemoveServers(int id);

        String CreateShard(Shard shard);

        Shards GetShards();

        String DropShard(int id, Shard.Member servers);

        List<ShardSpace> GetShardSpaces();

        String DropShardSpace(String database, String name);

        String CreateShardSpace(String database, ShardSpace shardSpace);

    }
}