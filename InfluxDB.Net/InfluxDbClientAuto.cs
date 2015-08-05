using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    internal class InfluxDbClientAuto : IInfluxDbClient
    {
        private readonly IInfluxDbClient _influxDbClient;
        private readonly IEnumerable<ApiResponseErrorHandlingDelegate> _noErrorHandlers = Enumerable.Empty<ApiResponseErrorHandlingDelegate>();
        private string _version;

        public InfluxDbClientAuto(InfluxDbClientConfiguration configuration)
        {
            _influxDbClient = CheckClientVersion(new InfluxDbClient(configuration), "0.9") ??
                              CheckClientVersion(new InfluxDbClientV08(configuration), "0.8");

            if (_influxDbClient == null)
            {
                var ex = new InvalidOperationException("Cannot find a database client for the current influxDB version.");
                ex.Data.Add("Version", _version ?? "N/A");
                throw ex;
            }
        }

        private IInfluxDbClient CheckClientVersion(IInfluxDbClient client, string version)
        {
            InfluxDbApiResponse response;
            try
            {
                response = client.Ping(_noErrorHandlers).Result;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                return null;
            }
            if (!response.Success) return null;
            _version = response.Body;
            return response.Body.StartsWith(version) ? client : null;
        }

        public async Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.Ping(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, Database database)
        {
            return await _influxDbClient.CreateDatabase(errorHandlers, database);
        }

        public async Task<InfluxDbApiResponse> DropDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name)
        {
            return await _influxDbClient.DropDatabase(errorHandlers, name);
        }

        public async Task<InfluxDbApiResponse> ShowDatabases(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.ShowDatabases(errorHandlers);
        }

        public async Task<InfluxDbApiWriteResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, WriteRequest request, string timePrecision)
        {
            return await _influxDbClient.Write(errorHandlers,request, timePrecision);
        }

        public async Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name, string query)
        {
            return await _influxDbClient.Query(errorHandlers, name, query);
        }

        public async Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user)
        {
            return await _influxDbClient.CreateClusterAdmin(errorHandlers, user);
        }

        public async Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name)
        {
            return await _influxDbClient.DeleteClusterAdmin(errorHandlers, name);
        }

        public async Task<InfluxDbApiResponse> DescribeClusterAdmins(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.DescribeClusterAdmins(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user, string name)
        {
            return await _influxDbClient.UpdateClusterAdmin(errorHandlers,user, name);
        }

        public async Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, User user)
        {
            return await _influxDbClient.CreateDatabaseUser(errorHandlers, database, user);
        }

        public async Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            return await _influxDbClient.DeleteDatabaseUser(errorHandlers, database, name);
        }

        public async Task<InfluxDbApiResponse> DescribeDatabaseUsers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            return await _influxDbClient.DescribeDatabaseUsers(errorHandlers, database);
        }

        public async Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, User user, string name)
        {
            return await _influxDbClient.UpdateDatabaseUser(errorHandlers, database, user, name);
        }

        public async Task<InfluxDbApiResponse> AuthenticateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string user, string password)
        {
            return await _influxDbClient.AuthenticateDatabaseUser(errorHandlers, database, user, password);
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            return await _influxDbClient.GetContinuousQueries(errorHandlers, database);
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, int id)
        {
            return await _influxDbClient.DeleteContinuousQuery(errorHandlers, database, id);
        }

        public async Task<InfluxDbApiResponse> DropSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            return await _influxDbClient.DropSeries(errorHandlers, database, name);
        }

        public async Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.ForceRaftCompaction(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.Interfaces(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.Sync(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.ListServers(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id)
        {
            return await _influxDbClient.RemoveServers(errorHandlers, id);
        }

        public async Task<InfluxDbApiResponse> CreateShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, Shard shard)
        {
            return await _influxDbClient.CreateShard(errorHandlers, shard);
        }

        public async Task<InfluxDbApiResponse> GetShards(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.GetShards(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> DropShard(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id, Shard.Member servers)
        {
            return await _influxDbClient.DropShard(errorHandlers, id, servers);
        }

        public async Task<InfluxDbApiResponse> GetShardSpaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.GetShardSpaces(errorHandlers);
        }

        public async Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            return await _influxDbClient.DropShardSpace(errorHandlers, database, name);
        }

        public async Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, ShardSpace shardSpace)
        {
            return await _influxDbClient.CreateShardSpace(errorHandlers, database, shardSpace);
        }
    }
}