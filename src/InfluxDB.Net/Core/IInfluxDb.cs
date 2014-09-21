using System;
using System.Collections.Generic;
using InfluxDB.Net.Models;

namespace InfluxDB.Net.Core
{
    public interface IInfluxDb
    {
        Pong Ping();
        String Version();
        InfluxDbResponse Write(string database, TimeUnit.Unit precision, params Serie[] series);
        InfluxDbResponse WriteUdp(int port, TimeUnit precision, params Serie[] series);
        List<Serie> Query(string database, string query, TimeUnit.Unit precision);
        CreateResponse CreateDatabase(string name);
        CreateResponse CreateDatabase(DatabaseConfiguration config);
        DeleteResponse DeleteDatabase(String name);
        List<Database> DescribeDatabases();
        InfluxDbResponse CreateClusterAdmin(String username, String adminPassword);
        InfluxDbResponse DeleteClusterAdmin(String username);
        List<User> DescribeClusterAdmins();
        InfluxDbResponse UpdateClusterAdmin(String username, String password);
        InfluxDbResponse CreateDatabaseUser(String database, String name, String password, params String[] permissions);
        InfluxDbResponse DeleteDatabaseUser(String database, String name);
        List<User> DescribeDatabaseUsers(String database);
        InfluxDbResponse UpdateDatabaseUser(String database, String name, String password, params String[] permissions);
        InfluxDbResponse AlterDatabasePrivilege(String database, String name, bool isAdmin, params String[] permissions);
        InfluxDbResponse AuthenticateDatabaseUser(String database, String user, String password);
        List<ContinuousQuery> DescribeContinuousQueries(String database);
        InfluxDbResponse DeleteContinuousQuery(String database, int id);
        InfluxDbResponse DeleteSeries(String database, String serieName);
        InfluxDbResponse ForceRaftCompaction();
        List<String> Interfaces();
        Boolean Sync();
        List<Server> ListServers();
        InfluxDbResponse RemoveServers(int id);

        InfluxDbResponse CreateShard(Shard shard);
        Shards GetShards();
        InfluxDbResponse DropShard(Shard shard);

        List<ShardSpace> GetShardSpaces();
        InfluxDbResponse DropShardSpace(String database, String name);
        InfluxDbResponse CreateShardSpace(String database, ShardSpace shardSpace);
    }
}