using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    public interface IInfluxDb
    {
        Task<Pong> PingAsync();
        Task<string> VersionAsync();
        Task<InfluxDbApiResponse> WriteAsync(string database, TimeUnit precision, params Serie[] series);
        Task<InfluxDbApiResponse> WriteUdpAsync(int port, TimeUnit precision, params Serie[] series);
        Task<List<Serie>> QueryAsync(string database, string query, TimeUnit precision);
        Task<InfluxDbApiCreateResponse> CreateDatabaseAsync(string name);
        Task<InfluxDbApiCreateResponse> CreateDatabaseAsync(DatabaseConfiguration config);
        Task<InfluxDbApiDeleteResponse> DeleteDatabaseAsync(String name);
        Task<List<Database>> DescribeDatabasesAsync();
        Task<InfluxDbApiResponse> CreateClusterAdminAsync(String username, String adminPassword);
        Task<InfluxDbApiResponse> DeleteClusterAdminAsync(String username);
        Task<List<User>> DescribeClusterAdminsAsync();
        Task<InfluxDbApiResponse> UpdateClusterAdminAsync(String username, String password);

        Task<InfluxDbApiResponse> CreateDatabaseUserAsync(String database, String name, String password,
            params String[] permissions);

        Task<InfluxDbApiResponse> DeleteDatabaseUserAsync(String database, String name);
        Task<List<User>> DescribeDatabaseUsersAsync(String database);

        Task<InfluxDbApiResponse> UpdateDatabaseUserAsync(String database, String name, String password,
            params String[] permissions);

        Task<InfluxDbApiResponse> AlterDatabasePrivilegeAsync(String database, String name, bool isAdmin,
            params String[] permissions);

        Task<InfluxDbApiResponse> AuthenticateDatabaseUserAsync(String database, String user, String password);
        Task<List<ContinuousQuery>> DescribeContinuousQueriesAsync(String database);
        Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(String database, int id);
        Task<InfluxDbApiResponse> DeleteSeriesAsync(String database, String serieName);
        Task<InfluxDbApiResponse> ForceRaftCompactionAsync();
        Task<List<string>> InterfacesAsync();
        Task<bool> SyncAsync();
        Task<List<Server>> ListServersAsync();
        Task<InfluxDbApiResponse> RemoveServersAsync(int id);

        Task<InfluxDbApiResponse> CreateShardAsync(Shard shard);
        Task<Shards> GetShardsAsync();
        Task<InfluxDbApiResponse> DropShardAsync(Shard shard);

        Task<List<ShardSpace>> GetShardSpacesAsync();
        Task<InfluxDbApiResponse> DropShardSpaceAsync(String database, String name);
        Task<InfluxDbApiResponse> CreateShardSpaceAsync(String database, ShardSpace shardSpace);
    }
}