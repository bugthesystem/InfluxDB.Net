using System;
using System.Collections.Generic;
using InfluxDB.Net.Models;

namespace InfluxDB.Net.Core
{
    public interface IInfluxDb
    {
        Pong Ping();
        String Version();
        void Write(string database, TimeUnit.Unit precision, params Serie[] series);
        void WriteUdp(int port, TimeUnit precision, params Serie[] series);
        List<Serie> Query(string database, string query, TimeUnit.Unit precision);
        void CreateDatabase(String name);
        void CreateDatabase(DatabaseConfiguration config);
        void DeleteDatabase(String name);
        List<Database> DescribeDatabases();
        void CreateClusterAdmin(String username, String adminPassword);
        void DeleteClusterAdmin(String username);
        List<User> DescribeClusterAdmins();
        void UpdateClusterAdmin(String username, String password);
        void CreateDatabaseUser(String database, String name, String password, params String[] permissions);
        void DeleteDatabaseUser(String database, String name);
        List<User> DescribeDatabaseUsers(String database);
        void UpdateDatabaseUser(String database, String name, String password, params String[] permissions);
        void AlterDatabasePrivilege(String database, String name, bool isAdmin, params String[] permissions);
        void AuthenticateDatabaseUser(String database, String user, String password);
        List<ContinuousQuery> DescribeContinuousQueries(String database);
        void DeleteContinuousQuery(String database, int id);
        void DeleteSeries(String database, String serieName);
        void ForceRaftCompaction();
        List<String> Interfaces();
        Boolean Sync();
        List<Server> ListServers();
        void RemoveServers(int id);

        void CreateShard(Shard shard);
        Shards GetShards();
        void DropShard(Shard shard);

        List<ShardSpace> GetShardSpaces();
        void DropShardSpace(String database, String name);
        void CreateShardSpace(String database, ShardSpace shardSpace);
    }
}