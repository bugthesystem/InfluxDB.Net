using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Net.Models;

namespace InfluxDB.Net
{
    public class InfluxDb : IInfluxDb
    {
        internal readonly IEnumerable<ApiResponseErrorHandlingDelegate> NoErrorHandlers =
            Enumerable.Empty<ApiResponseErrorHandlingDelegate>();

        private readonly IInfluxDbClient _influxDbClient;

        public InfluxDb(string url, string username, string password)
            : this(new InfluxDbClient(new InfluxDbClientConfiguration(new Uri(url), username, password)))
        {
            Check.NotNullOrEmpty(url, "The URL may not be null or empty.");
            Check.NotNullOrEmpty(username, "The username may not be null or empty.");
        }

        internal InfluxDb(IInfluxDbClient influxDbClient)
        {
            _influxDbClient = influxDbClient;
        }

        /// <summary>
        ///     Ping this InfluxDB
        /// </summary>
        /// <returns>The response of the ping execution.</returns>
        public async Task<Pong> PingAsync()
        {
            Stopwatch watch = Stopwatch.StartNew();

            InfluxDbApiResponse response = await _influxDbClient.Ping(NoErrorHandlers);

            watch.Stop();
            var pong = response.ReadAs<Pong>();
            pong.ResponseTime = watch.ElapsedMilliseconds;

            return pong;
        }

        /// <summary>
        ///     Return the version of the connected influxDB Server.
        /// </summary>
        /// <returns>The version String, otherwise unknown</returns>
        public async Task<string> VersionAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.Version(NoErrorHandlers);
            const string version = "unknown";

            if (!string.IsNullOrEmpty(response.Body))
            {
                return response.Body;
            }

            return version;
        }

        /// <summary>
        ///     Write a Series to the given database.
        /// </summary>
        /// <param name="database">The name of the database to write to</param>
        /// <param name="precision">The precision used for the values.</param>
        /// <param name="series">An array of <see cref="Serie" /> to write</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> WriteAsync(string database, TimeUnit precision, params Serie[] series)
        {
            return await _influxDbClient.Write(NoErrorHandlers, database, series, ToTimePrecision(precision));
        }

        /// <summary>
        ///     Write a Series to the given database
        /// </summary>
        /// <param name="port">
        ///     The port where to reach the influxdb udp service. The database is configured per port in the
        ///     influxdb configuration.
        /// </param>
        /// <param name="precision">The precision used for the values</param>
        /// <param name="series">An array of <see cref="Serie" /> to write</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> WriteUdpAsync(int port, TimeUnit precision, params Serie[] series)
        {
            throw new NotImplementedException("WriteUdpAsync is not implemented yet, sorry.");
        }

        /// <summary>
        ///     Execute a query agains a database.
        /// </summary>
        /// <param name="database">the name of the database</param>
        /// <param name="query">
        ///     the query to execute, for language specification please see
        ///     <a href="http://influxdb.org/docs/query_language">http://influxdb.org/docs/query_language</a>
        /// </param>
        /// <param name="precision">the precision used for the values.</param>
        /// <returns>A list of Series which matched the query.</returns>
        public async Task<List<Serie>> QueryAsync(string database, string query, TimeUnit precision)
        {
            InfluxDbApiResponse response =
                await _influxDbClient.Query(NoErrorHandlers, database, query, ToTimePrecision(precision));

            return response.ReadAs<List<Serie>>();
        }

        /// <summary>
        ///     Create a new Database.
        /// </summary>
        /// <param name="name">The name of the new database</param>
        /// <returns></returns>
        public async Task<InfluxDbApiCreateResponse> CreateDatabaseAsync(string name)
        {
            var db = new Database {Name = name};

            InfluxDbApiResponse response = await _influxDbClient.CreateDatabase(NoErrorHandlers, db);

            return new InfluxDbApiCreateResponse(response.StatusCode, response.Body);
        }

        /// <summary>
        ///     Create a new Database from a <see cref="DatabaseConfiguration" />. This is the way to create a db with shards
        ///     specified.
        /// </summary>
        /// <param name="config">The configuration for the database to create..</param>
        /// <returns></returns>
        public async Task<InfluxDbApiCreateResponse> CreateDatabaseAsync(DatabaseConfiguration config)
        {
            InfluxDbApiResponse response = await _influxDbClient.CreateDatabase(NoErrorHandlers, config);
            return new InfluxDbApiCreateResponse(response.StatusCode, response.Body);
        }

        /// <summary>
        ///     Delete a database.
        /// </summary>
        /// <param name="name">The name of the database to delete.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiDeleteResponse> DeleteDatabaseAsync(string name)
        {
            InfluxDbApiResponse response = await _influxDbClient.DeleteDatabase(NoErrorHandlers, name);

            return new InfluxDbApiDeleteResponse(response.StatusCode, response.Body);
        }

        /// <summary>
        ///     Describe all available databases.
        /// </summary>
        /// <returns>A list of all Databases</returns>
        public async Task<List<Database>> DescribeDatabasesAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.DescribeDatabases(NoErrorHandlers);

            return response.ReadAs<List<Database>>();
        }

        /// <summary>
        ///     Create a new cluster admin.
        /// </summary>
        /// <param name="username">The name of the new admin.</param>
        /// <param name="adminPassword">The password for the new admin.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> CreateClusterAdminAsync(string username, string adminPassword)
        {
            var user = new User {Name = username, Password = adminPassword};
            return await _influxDbClient.CreateClusterAdmin(NoErrorHandlers, user);
        }

        /// <summary>
        ///     Delete a cluster admin.
        /// </summary>
        /// <param name="username">The name of the admin to delete.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> DeleteClusterAdminAsync(string username)
        {
            return await _influxDbClient.DeleteClusterAdmin(NoErrorHandlers, username);
        }

        /// <summary>
        ///     Describe all cluster admins.
        /// </summary>
        /// <returns>A list of all admins.</returns>
        public async Task<List<User>> DescribeClusterAdminsAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.DescribeClusterAdmins(NoErrorHandlers);

            return response.ReadAs<List<User>>();
        }

        /// <summary>
        ///     Update the password of the given admin.
        /// </summary>
        /// <param name="username">The name of the admin for which the password should be updated.</param>
        /// <param name="password">The new password for the given admin.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> UpdateClusterAdminAsync(string username, string password)
        {
            var user = new User {Name = username, Password = password};

            return await _influxDbClient.UpdateClusterAdmin(NoErrorHandlers, user, username);
        }

        /// <summary>
        ///     Create a new regular database user. Without any given permissions the new user is allowed to
        ///     read and write to the database. The permission must be specified in regex which will match
        ///     for the series. You have to specify either no permissions or both (readFrom and writeTo) permissions.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the new database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> CreateDatabaseUserAsync(string database, string name, string password,
            params string[] permissions)
        {
            var user = new User {Name = name, Password = password};
            user.SetPermissions(permissions);
            return await _influxDbClient.CreateDatabaseUser(NoErrorHandlers, database, user);
        }

        /// <summary>
        ///     Delete a database user.
        /// </summary>
        /// <param name="database">The name of the database the given user should be removed from.</param>
        /// <param name="name">The name of the user to remove.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> DeleteDatabaseUserAsync(string database, string name)
        {
            return await _influxDbClient.DeleteDatabaseUser(NoErrorHandlers, database, name);
        }

        /// <summary>
        ///     Describe all database users allowed to acces the given database.
        /// </summary>
        /// <param name="database">The name of the database for which all users should be described.</param>
        /// <returns>A list of all users.</returns>
        public async Task<List<User>> DescribeDatabaseUsersAsync(string database)
        {
            InfluxDbApiResponse response = await _influxDbClient.DescribeDatabaseUsers(NoErrorHandlers, database);

            return response.ReadAs<List<User>>();
        }

        /// <summary>
        ///     Update the password and/or the permissions of a database user.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the existing database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> UpdateDatabaseUserAsync(string database, string name, string password,
            params string[] permissions)
        {
            var user = new User {Name = name, Password = password};
            user.SetPermissions(permissions);
            return await _influxDbClient.UpdateDatabaseUser(NoErrorHandlers, database, user, name);
        }

        /// <summary>
        ///     Alter the admin privilege of a given database user.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the existing database user.</param>
        /// <param name="isAdmin">If set to true this user is a database admin, otherwise it isnt.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> AlterDatabasePrivilegeAsync(string database, string name, bool isAdmin,
            params string[] permissions)
        {
            var user = new User {Name = name, IsAdmin = isAdmin};
            user.SetPermissions(permissions);
            return await _influxDbClient.UpdateDatabaseUser(NoErrorHandlers, database, user, name);
        }

        /// <summary>
        ///     Authenticate with the given credentials against the database.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="user">The name of the existing database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> AuthenticateDatabaseUserAsync(string database, string user,
            string password)
        {
            return await _influxDbClient.AuthenticateDatabaseUser(NoErrorHandlers, database, user, password);
        }

        /// <summary>
        ///     Describe all contious queries in a database.
        /// </summary>
        /// <param name="database">The name of the database for which all continuous queries should be described.</param>
        /// <returns>A list of all contious queries.</returns>
        public async Task<List<ContinuousQuery>> DescribeContinuousQueriesAsync(string database)
        {
            InfluxDbApiResponse response = await _influxDbClient.GetContinuousQueries(NoErrorHandlers, database);

            return response.ReadAs<List<ContinuousQuery>>();
        }

        /// <summary>
        ///     Delete a continous query.
        /// </summary>
        /// <param name="database">The name of the database for which this query should be deleted.</param>
        /// <param name="id">The id of the query.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(string database, int id)
        {
            return await _influxDbClient.DeleteContinuousQuery(NoErrorHandlers, database, id);
        }

        /// <summary>
        ///     Delete a serie.
        /// </summary>
        /// <param name="database">The database in which the given serie should be deleted.</param>
        /// <param name="serieName">The name of the serie.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> DeleteSeriesAsync(string database, string serieName)
        {
            return await _influxDbClient.DeleteSeries(NoErrorHandlers, database, serieName);
        }

        /// <summary>
        ///     Force Database compaction.
        /// </summary>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> ForceRaftCompactionAsync()
        {
            return await _influxDbClient.ForceRaftCompaction(NoErrorHandlers);
        }

        /// <summary>
        ///     List all interfaces influxDB is listening.
        /// </summary>
        /// <returns>A list of interface names.</returns>
        public async Task<List<string>> InterfacesAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.Interfaces(NoErrorHandlers);

            return response.ReadAs<List<string>>();
        }

        /// <summary>
        ///     Sync the database to the filesystem.
        /// </summary>
        /// <returns>true|false if successful.</returns>
        public async Task<bool> SyncAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.Sync(NoErrorHandlers);

            return response.ReadAs<bool>();
        }

        /// <summary>
        ///     List all servers which are member of the cluster.
        /// </summary>
        /// <returns>A list of all influxdb servers.</returns>
        public async Task<List<Server>> ListServersAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.ListServers(NoErrorHandlers);

            return response.ReadAs<List<Server>>();
        }

        /// <summary>
        ///     Remove the given Server from the cluster.
        /// </summary>
        /// <param name="id">The id of the server to remove.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> RemoveServersAsync(int id)
        {
            return await _influxDbClient.RemoveServers(NoErrorHandlers, id);
        }

        /// <summary>
        ///     Create a new Shard.
        /// </summary>
        /// <param name="shard">the new shard to create.</param>
        /// <returns></returns>
        [Obsolete("This functionality is gone with 0.8.0, will be removed in the next version.")]
        public async Task<InfluxDbApiResponse> CreateShardAsync(Shard shard)
        {
            return await _influxDbClient.CreateShard(NoErrorHandlers, shard);
        }

        /// <summary>
        ///     Describe all existing shards.
        /// </summary>
        /// <returns>A list of all Shards.</returns>
        [Obsolete("This functionality is gone with 0.8.0, will be removed in the next version.")]
        public async Task<Shards> GetShardsAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.GetShards(NoErrorHandlers);

            return response.ReadAs<Shards>();
        }

        /// <summary>
        ///     Drop the given shard.
        /// </summary>
        /// <param name="shard">The shard (<see cref="Shard" />) to delete.</param>
        /// <returns></returns>
        [Obsolete("This functionality is gone with 0.8.0, will be removed in the next version.")]
        public async Task<InfluxDbApiResponse> DropShardAsync(Shard shard)
        {
            return await _influxDbClient.DropShard(NoErrorHandlers, shard.Id, shard.Shards.First());
        }

        /// <summary>
        ///     Describe all existing shardspaces.
        /// </summary>
        /// <returns>A list of all <see cref="ShardSpace"></see>'s.</returns>
        public async Task<List<ShardSpace>> GetShardSpacesAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.GetShardSpaces(NoErrorHandlers);

            return response.ReadAs<List<ShardSpace>>();
        }

        /// <summary>
        ///     Drop a existing ShardSpace from a Database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <param name="name">The name of the ShardSpace to delete.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> DropShardSpaceAsync(string database, string name)
        {
            return await _influxDbClient.DropShardSpace(NoErrorHandlers, database, name);
        }

        /// <summary>
        ///     Create a ShardSpace in a Database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <param name="shardSpace">The shardSpace to create in this database</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> CreateShardSpaceAsync(string database, ShardSpace shardSpace)
        {
            return await _influxDbClient.CreateShardSpace(NoErrorHandlers, database, shardSpace);
        }


        public IInfluxDb SetLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:

                    break;
                case LogLevel.Basic:

                    break;
                case LogLevel.Headers:

                    break;
                case LogLevel.Full:
                    break;
            }

            return this;
        }

        private static String ToTimePrecision(TimeUnit t)
        {
            switch (t)
            {
                case TimeUnit.Seconds:
                    return "s";
                case TimeUnit.Milliseconds:
                    return "ms";
                case TimeUnit.Microseconds:
                    return "u";
                default:
                    throw new ArgumentException("time precision must be " + TimeUnit.Seconds + ", " +
                                                TimeUnit.Milliseconds + " or " + TimeUnit.Microseconds);
            }
        }
    }
}