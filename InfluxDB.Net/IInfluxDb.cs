using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
	public interface IInfluxDb
	{
		Task<Pong> PingAsync();
		Task<InfluxDbApiWriteResponse> WriteAsync(string database, Point[] points, string retenionPolicy = "default");
		Task<List<Serie>> QueryAsync(string database, string query);
		Task<InfluxDbApiResponse> CreateDatabaseAsync(string name);
		Task<InfluxDbApiResponse> DropDatabaseAsync(string name);
		Task<List<Database>> ShowDatabasesAsync();
		Task<InfluxDbApiResponse> CreateClusterAdminAsync(string username, string adminPassword);
		Task<InfluxDbApiResponse> DeleteClusterAdminAsync(string username);
		Task<List<User>> DescribeClusterAdminsAsync();
		Task<InfluxDbApiResponse> UpdateClusterAdminAsync(string username, string password);

		Task<InfluxDbApiResponse> CreateDatabaseUserAsync(string database, string name, string password,
			 params string[] permissions);

		Task<InfluxDbApiResponse> DeleteDatabaseUserAsync(string database, string name);
		Task<List<User>> DescribeDatabaseUsersAsync(string database);

		Task<InfluxDbApiResponse> UpdateDatabaseUserAsync(string database, string name, string password,
			 params string[] permissions);

		Task<InfluxDbApiResponse> AlterDatabasePrivilegeAsync(string database, string name, bool isAdmin,
			 params string[] permissions);

		Task<InfluxDbApiResponse> AuthenticateDatabaseUserAsync(string database, string user, string password);
		Task<List<ContinuousQuery>> DescribeContinuousQueriesAsync(string database);
		Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(string database, int id);
		Task<InfluxDbApiResponse> DropSeriesAsync(string database, string serieName);
		Task<InfluxDbApiResponse> ForceRaftCompactionAsync();
		Task<List<string>> InterfacesAsync();
		Task<bool> SyncAsync();
		Task<List<Server>> ListServersAsync();
		Task<InfluxDbApiResponse> RemoveServersAsync(int id);

		Task<InfluxDbApiResponse> CreateShardAsync(Shard shard);
		Task<Shards> GetShardsAsync();
		Task<InfluxDbApiResponse> DropShardAsync(Shard shard);

		Task<List<ShardSpace>> GetShardSpacesAsync();
		Task<InfluxDbApiResponse> DropShardSpaceAsync(string database, string name);
		Task<InfluxDbApiResponse> CreateShardSpaceAsync(string database, ShardSpace shardSpace);
	    IFormatter GetFormatter();
        InfluxVersion GetClientVersion();
	}
}