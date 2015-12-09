using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.Net.Infrastructure.Influx;
using InfluxDB.Net.Models;

namespace InfluxDB.Net.Contracts
{
    public interface IInfluxDb
    {
        #region Database

        Task<InfluxDbApiResponse> CreateDatabaseAsync(string name);

        Task<InfluxDbApiResponse> DropDatabaseAsync(string name);

        Task<List<Database>> ShowDatabasesAsync();

        #endregion Database

        #region Basic Querying

        Task<InfluxDbApiWriteResponse> WriteAsync(string database, Point point, string retenionPolicy = "default");

        Task<InfluxDbApiWriteResponse> WriteAsync(string database, Point[] points, string retenionPolicy = "default");

        Task<List<Serie>> QueryAsync(string database, string query);

        #endregion Basic Querying

        #region Continuous Queries

        Task<List<ContinuousQuery>> DescribeContinuousQueriesAsync(string database);

        Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(string database, int id);

        #endregion Continuous Queries

        #region Series

        Task<InfluxDbApiResponse> DropSeriesAsync(string database, string serieName);

        #endregion Series

        #region Clustering

        Task<InfluxDbApiResponse> CreateClusterAdminAsync(string username, string adminPassword);

        Task<InfluxDbApiResponse> DeleteClusterAdminAsync(string username);

        Task<List<User>> DescribeClusterAdminsAsync();

        Task<InfluxDbApiResponse> UpdateClusterAdminAsync(string username, string password);

        #endregion Clustering

        #region Sharding

        Task<List<ShardSpace>> GetShardSpacesAsync();

        Task<InfluxDbApiResponse> DropShardSpaceAsync(string database, string name);

        Task<InfluxDbApiResponse> CreateShardSpaceAsync(string database, ShardSpace shardSpace);

        #endregion Sharding

        #region Users

        Task<InfluxDbApiResponse> CreateDatabaseUserAsync(string database, string name, string password, params string[] permissions);

        Task<InfluxDbApiResponse> DeleteDatabaseUserAsync(string database, string name);

        Task<List<User>> DescribeDatabaseUsersAsync(string database);

        Task<InfluxDbApiResponse> UpdateDatabaseUserAsync(string database, string name, string password, params string[] permissions);

        Task<InfluxDbApiResponse> AlterDatabasePrivilegeAsync(string database, string name, bool isAdmin, params string[] permissions);

        Task<InfluxDbApiResponse> AuthenticateDatabaseUserAsync(string database, string user, string password);

        #endregion Users

        #region Other

        Task<Pong> PingAsync();

        Task<InfluxDbApiResponse> ForceRaftCompactionAsync();

        Task<List<string>> InterfacesAsync();

        Task<bool> SyncAsync();

        Task<List<Server>> ListServersAsync();

        Task<InfluxDbApiResponse> RemoveServersAsync(int id);

        #endregion Other
    }
}