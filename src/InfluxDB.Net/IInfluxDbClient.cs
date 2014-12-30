using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    internal interface IInfluxDbClient
    {
        Pong Ping();

        Task<HttpResponseMessage> Version();

        Task<HttpResponseMessage> CreateDatabase(Database database);

        Task<HttpResponseMessage> CreateDatabase(DatabaseConfiguration config);

        Task<HttpResponseMessage> DeleteDatabase(string name);

        Task<List<Database>> DescribeDatabases();

        Task<HttpResponseMessage> Write(string name, Serie[] series, string timePrecision);

        Task<List<Serie>> Query(String name, String query, String timePrecision);

        Task<HttpResponseMessage> CreateClusterAdmin(User user);

        Task<HttpResponseMessage> DeleteClusterAdmin(string name);

        Task<List<User>> DescribeClusterAdmins();

        Task<HttpResponseMessage> UpdateClusterAdmin(User user, string name);

        Task<HttpResponseMessage> CreateDatabaseUser(string database, User user);

        Task<HttpResponseMessage> DeleteDatabaseUser(string database, string name);

        Task<List<User>> DescribeDatabaseUsers(String database);

        Task<HttpResponseMessage> UpdateDatabaseUser(string database, User user, string name);

        Task<HttpResponseMessage> AuthenticateDatabaseUser(string database, string user, string password);

        Task<List<ContinuousQuery>> GetContinuousQueries(String database);

        Task<HttpResponseMessage> DeleteContinuousQuery(string database, int id);

        Task<HttpResponseMessage> DeleteSeries(string database, string name);

        Task<HttpResponseMessage> ForceRaftCompaction();

        Task<List<string>> Interfaces();

        Task<bool> Sync();

        Task<List<Server>> ListServers();

        Task<HttpResponseMessage> RemoveServers(int id);

        Task<HttpResponseMessage> CreateShard(Shard shard);

        Task<Shards> GetShards();

        Task<HttpResponseMessage> DropShard(int id, Shard.Member servers);

        Task<List<ShardSpace>> GetShardSpaces();

        Task<HttpResponseMessage> DropShardSpace(string database, string name);

        Task<HttpResponseMessage> CreateShardSpace(string database, ShardSpace shardSpace);

    }
}