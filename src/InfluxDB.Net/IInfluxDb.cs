using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    public interface IInfluxDb
    {
        Pong PingAsync();
        Task<string> VersionAsync();
        Task<InfluxDbResponse> WriteAsync(string database, TimeUnit precision, params Serie[] series);
        Task<InfluxDbResponse> WriteUdpAsync(int port, TimeUnit precision, params Serie[] series);
        Task<List<Serie>> QueryAsync(string database, string query, TimeUnit precision);
        Task<CreateResponse> CreateDatabaseAsync(string name);
        Task<CreateResponse> CreateDatabaseAsync(DatabaseConfiguration config);
        Task<DeleteResponse> DeleteDatabaseAsync(String name);
        Task<List<Database>> DescribeDatabasesAsync();
        Task<InfluxDbResponse> CreateClusterAdminAsync(String username, String adminPassword);
        Task<InfluxDbResponse> DeleteClusterAdminAsync(String username);
        Task<List<User>> DescribeClusterAdminsAsync();
        Task<InfluxDbResponse> UpdateClusterAdminAsync(String username, String password);
        Task<InfluxDbResponse> CreateDatabaseUserAsync(String database, String name, String password, params String[] permissions);
        Task<InfluxDbResponse> DeleteDatabaseUserAsync(String database, String name);
        Task<List<User>> DescribeDatabaseUsersAsync(String database);
        Task<InfluxDbResponse> UpdateDatabaseUserAsync(String database, String name, String password, params String[] permissions);
        Task<InfluxDbResponse> AlterDatabasePrivilegeAsync(String database, String name, bool isAdmin, params String[] permissions);
        Task<InfluxDbResponse> AuthenticateDatabaseUserAsync(String database, String user, String password);
        Task<List<ContinuousQuery>> DescribeContinuousQueriesAsync(String database);
        Task<InfluxDbResponse> DeleteContinuousQueryAsync(String database, int id);
        Task<InfluxDbResponse> DeleteSeriesAsync(String database, String serieName);
        Task<InfluxDbResponse> ForceRaftCompactionAsync();
        Task<List<string>> InterfacesAsync();
        Task<bool> SyncAsync();
        Task<List<Server>> ListServersAsync();
        Task<InfluxDbResponse> RemoveServersAsync(int id);

        Task<InfluxDbResponse> CreateShardAsync(Shard shard);
        Task<Shards> GetShardsAsync();
        Task<InfluxDbResponse> DropShardAsync(Shard shard);

        Task<List<ShardSpace>> GetShardSpacesAsync();
        Task<InfluxDbResponse> DropShardSpaceAsync(String database, String name);
        Task<InfluxDbResponse> CreateShardSpaceAsync(String database, ShardSpace shardSpace);
    }
}