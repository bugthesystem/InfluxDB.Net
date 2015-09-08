using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    internal interface IInfluxDbClient
    {
        Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            Database database);

        Task<InfluxDbApiResponse> DropDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name);

        Task<InfluxDbApiResponse> ShowDatabases(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiWriteResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, WriteRequest request, string timePrecision);

        Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name,
            string query);

        Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user);

        Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string name);

        Task<InfluxDbApiResponse> DescribeClusterAdmins(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            User user, string name);

        Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user);

        Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name);

        Task<InfluxDbApiResponse> DescribeDatabaseUsers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database);

        Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, User user, string name);

        Task<InfluxDbApiResponse> AuthenticateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string user, string password);

        Task<InfluxDbApiResponse> GetContinuousQueries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database);

        Task<InfluxDbApiResponse> DeleteContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, int id);

        Task<InfluxDbApiResponse> DropSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name);

        Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id);

        Task<InfluxDbApiResponse> CreateShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, Shard shard);

        Task<InfluxDbApiResponse> GetShards(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> DropShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id,
            Shard.Member servers);

        Task<InfluxDbApiResponse> GetShardSpaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, string name);

        Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
            string database, ShardSpace shardSpace);

        IFormatter GetFormatter();
        InfluxVersion GetVersion();
    }
}