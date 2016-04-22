using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.Net.Models;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Infrastructure.Influx;
using InfluxDB.Net.Infrastructure.Configuration;

namespace InfluxDB.Net.Client
{
    internal class InfluxDbClientAutoVersion : IInfluxDbClient
    {
        private readonly IInfluxDbClient _influxDbClient;

        public InfluxDbClientAutoVersion(InfluxDbClientConfiguration influxDbClientConfiguration)
        {
            _influxDbClient = new InfluxDbClientBase(influxDbClientConfiguration);
            var errorHandlers = new List<ApiResponseErrorHandlingDelegate>();
            // TODO: needs testing - potentially bad if it's going to ping for every request
            var result = _influxDbClient.Ping(errorHandlers).Result;
            var databaseVersion = result.Body;

            if (databaseVersion.StartsWith("0.12."))
            {
                _influxDbClient = new InfluxDbClientV012x(influxDbClientConfiguration);
            }
            else if (databaseVersion.StartsWith("0.11."))
            {
                _influxDbClient = new InfluxDbClientV011x(influxDbClientConfiguration);
            }
            else if (databaseVersion.StartsWith("0.10."))
            {
                _influxDbClient = new InfluxDbClientV010x(influxDbClientConfiguration);
            }
            else if (databaseVersion.StartsWith("0.9."))
            {
                switch (databaseVersion)
                {
                    case "0.9.2":
                        _influxDbClient = new InfluxDbClientV092(influxDbClientConfiguration);
                        break;
                    case "0.9.5":
                        _influxDbClient = new InfluxDbClientV092(influxDbClientConfiguration);
                        break;
                    case "0.9.6":
                        _influxDbClient = new InfluxDbClientV092(influxDbClientConfiguration);
                        break;
                }
            }
            else
            {
                _influxDbClient = new InfluxDbClientV0x(influxDbClientConfiguration);
            }
        }

        #region Database

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

        #endregion Database

        #region Basic Querying

        public async Task<InfluxDbApiWriteResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, WriteRequest request, string timePrecision)
        {
            return await _influxDbClient.Write(errorHandlers, request, timePrecision);
        }

        public async Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name, string query)
        {
            return await _influxDbClient.Query(errorHandlers, name, query);
        }

        #endregion Basic Querying

        #region Continuous Queries

        public async Task<InfluxDbApiResponse> GetContinuousQueries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            return await _influxDbClient.GetContinuousQueries(errorHandlers, database);
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, int id)
        {
            return await _influxDbClient.DeleteContinuousQuery(errorHandlers, database, id);
        }

        #endregion Continuous Queries

        #region Series

        public async Task<InfluxDbApiResponse> DropSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            return await _influxDbClient.DropSeries(errorHandlers, database, name);
        }

        #endregion Series

        #region Clustering

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
            return await _influxDbClient.UpdateClusterAdmin(errorHandlers, user, name);
        }

        #endregion Clustering

        #region Sharding

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

        #endregion Sharding

        #region Users

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

        #endregion Users

        #region Other

        public async Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await _influxDbClient.Ping(errorHandlers);
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

        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string policyName, string dbName, string duration, int replication)
        {
            return await _influxDbClient.AlterRetentionPolicy(errorHandlers, policyName, dbName, duration, replication);
        }

        public IFormatter GetFormatter()
        {
            return _influxDbClient.GetFormatter();
        }

        public InfluxVersion GetVersion()
        {
            return _influxDbClient.GetVersion();
        }

        #endregion Other
    }
}