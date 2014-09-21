using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InfluxDB.Net.Core;
using InfluxDB.Net.Models;
using RestSharp;

namespace InfluxDB.Net
{
    public class InfluxDb : IInfluxDb
    {
        private readonly IInfluxDbClient _influxDbClient;


        public InfluxDb(string url, string username, string password)
            : this(new InfluxDbClient(url, username, password))
        {
            Check.NotNullOrEmpty(url, "The URL may not be null or empty.");
            Check.NotNullOrEmpty(username, "The username may not be null or empty.");
        }

        internal InfluxDb(IInfluxDbClient influxDbClient)
        {
            _influxDbClient = influxDbClient;
        }

        /// <summary>
        /// Ping this InfluxDB
        /// </summary>
        /// <returns>The response of the ping execution.</returns>
        public Pong Ping()
        {
            var watch = new Stopwatch();
            watch.Start();

            Pong pong = _influxDbClient.Ping();
            pong.ResponseTime = watch.ElapsedMilliseconds;

            watch.Stop();

            return pong;
        }

        /// <summary>
        /// Return the version of the connected influxDB Server.
        /// </summary>
        /// <returns>The version String, otherwise unknown</returns>
        public string Version()
        {
            IRestResponse response = _influxDbClient.Version();
            String version = "unknown";
            IList<Parameter> headers = response.Headers;

            foreach (Parameter header in headers)
            {
                if (null != header.Name && header.Name.Equals("X-Influxdb-Version"))
                {
                    version = header.Value.ToString();
                }
            }

            return version;
        }

        /// <summary>
        /// Write a Series to the given database.
        /// </summary>
        /// <param name="database">The name of the database to write to</param>
        /// <param name="precision">The precision used for the values.</param>
        /// <param name="series">An array of <see cref="Serie"/> to write</param>
        /// <returns></returns>
        public InfluxDbResponse Write(string database, TimeUnit.Unit precision, params Serie[] series)
        {
            return new InfluxDbResponse(_influxDbClient.Write(database, series, ToTimePrecision(precision)));
        }

        /// <summary>
        /// Write a Series to the given database
        /// </summary>
        /// <param name="port">The port where to reach the influxdb udp service. The database is configured per port in the influxdb configuration.</param>
        /// <param name="precision">The precision used for the values</param>
        /// <param name="series">An array of <see cref="Serie"/> to write</param>
        /// <returns></returns>
        public InfluxDbResponse WriteUdp(int port, TimeUnit precision, params Serie[] series)
        {
            throw new NotImplementedException("WriteUdp is not implemented yet, sorry.");
        }

        /// <summary>
        /// Execute a query agains a database.
        /// </summary>
        /// <param name="database">the name of the database</param>
        /// <param name="query">the query to execute, for language specification please see <a href="http://influxdb.org/docs/query_language">http://influxdb.org/docs/query_language</a></param>
        /// <param name="precision">the precision used for the values.</param>
        /// <returns>A list of Series which matched the query.</returns>
        public List<Serie> Query(string database, string query, TimeUnit.Unit precision)
        {
            return _influxDbClient.Query(database, query, ToTimePrecision(precision));
        }

        /// <summary>
        /// Create a new Database.
        /// </summary>
        /// <param name="name">The name of the new database</param>
        /// <returns></returns>
        public CreateDbResponse CreateDatabase(string name)
        {
            var db = new Database { name = name };

            return new CreateDbResponse(_influxDbClient.CreateDatabase(db));
        }

        /// <summary>
        /// Create a new Database from a <see cref="DatabaseConfiguration"/>. This is the way to create a db with shards specified.
        /// </summary>
        /// <param name="config">The configuration for the database to create..</param>
        /// <returns></returns>
        public CreateDbResponse CreateDatabase(DatabaseConfiguration config)
        {
            return new CreateDbResponse(_influxDbClient.CreateDatabase(config));
        }

        /// <summary>
        /// Delete a database.
        /// </summary>
        /// <param name="name">The name of the database to delete.</param>
        /// <returns></returns>
        public InfluxDbResponse DeleteDatabase(string name)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteDatabase(name));
        }

        /// <summary>
        /// Describe all available databases.
        /// </summary>
        /// <returns>A list of all Databases</returns>
        public List<Database> DescribeDatabases()
        {
            return _influxDbClient.DescribeDatabases();
        }

        /// <summary>
        /// Create a new cluster admin.
        /// </summary>
        /// <param name="username">The name of the new admin.</param>
        /// <param name="adminPassword">The password for the new admin.</param>
        /// <returns></returns>
        public InfluxDbResponse CreateClusterAdmin(string username, string adminPassword)
        {
            var user = new User { Name = username, Password = adminPassword };
            return new InfluxDbResponse(_influxDbClient.CreateClusterAdmin(user));
        }

        /// <summary>
        /// Delete a cluster admin.
        /// </summary>
        /// <param name="username">The name of the admin to delete.</param>
        /// <returns></returns>
        public InfluxDbResponse DeleteClusterAdmin(string username)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteClusterAdmin(username));
        }

        /// <summary>
        /// Describe all cluster admins.
        /// </summary>
        /// <returns>A list of all admins.</returns>
        public List<User> DescribeClusterAdmins()
        {
            return _influxDbClient.DescribeClusterAdmins();
        }

        /// <summary>
        /// Update the password of the given admin.
        /// </summary>
        /// <param name="username">The name of the admin for which the password should be updated.</param>
        /// <param name="password">The new password for the given admin.</param>
        /// <returns></returns>
        public InfluxDbResponse UpdateClusterAdmin(string username, string password)
        {
            var user = new User { Name = username, Password = password };

            return new InfluxDbResponse(_influxDbClient.UpdateClusterAdmin(user, username));
        }

        /// <summary>
        /// Create a new regular database user. Without any given permissions the new user is allowed to
        ///  read and write to the database. The permission must be specified in regex which will match
        ///  for the series. You have to specify either no permissions or both (readFrom and writeTo) permissions.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the new database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        public InfluxDbResponse CreateDatabaseUser(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            return new InfluxDbResponse(_influxDbClient.CreateDatabaseUser(database, user));
        }

        /// <summary>
        /// Delete a database user.
        /// </summary>
        /// <param name="database">The name of the database the given user should be removed from.</param>
        /// <param name="name">The name of the user to remove.</param>
        /// <returns></returns>
        public InfluxDbResponse DeleteDatabaseUser(string database, string name)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteDatabaseUser(database, name));
        }

        /// <summary>
        /// Describe all database users allowed to acces the given database.
        /// </summary>
        /// <param name="database">The name of the database for which all users should be described.</param>
        /// <returns>A list of all users.</returns>
        public List<User> DescribeDatabaseUsers(string database)
        {
            return _influxDbClient.DescribeDatabaseUsers(database);
        }

        /// <summary>
        /// Update the password and/or the permissions of a database user.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the existing database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        public InfluxDbResponse UpdateDatabaseUser(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            return new InfluxDbResponse(_influxDbClient.UpdateDatabaseUser(database, user, name));
        }

        /// <summary>
        /// Alter the admin privilege of a given database user.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the existing database user.</param>
        /// <param name="isAdmin">If set to true this user is a database admin, otherwise it isnt.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        public InfluxDbResponse AlterDatabasePrivilege(string database, string name, bool isAdmin, params string[] permissions)
        {
            var user = new User { Name = name, IsAdmin = isAdmin };
            user.SetPermissions(permissions);
            return new InfluxDbResponse(_influxDbClient.UpdateDatabaseUser(database, user, name));
        }

        /// <summary>
        /// Authenticate with the given credentials against the database.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="user">The name of the existing database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <returns></returns>
        public InfluxDbResponse AuthenticateDatabaseUser(string database, string user, string password)
        {
            return new InfluxDbResponse(_influxDbClient.AuthenticateDatabaseUser(database, user, password));
        }

        /// <summary>
        /// Describe all contious queries in a database.
        /// </summary>
        /// <param name="database">The name of the database for which all continuous queries should be described.</param>
        /// <returns>A list of all contious queries.</returns>
        public List<ContinuousQuery> DescribeContinuousQueries(string database)
        {
            return _influxDbClient.GetContinuousQueries(database);
        }

        /// <summary>
        /// Delete a continous query.
        /// </summary>
        /// <param name="database">The name of the database for which this query should be deleted.</param>
        /// <param name="id">The id of the query.</param>
        /// <returns></returns>
        public InfluxDbResponse DeleteContinuousQuery(string database, int id)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteContinuousQuery(database, id));
        }

        /// <summary>
        /// Delete a serie.
        /// </summary>
        /// <param name="database">The database in which the given serie should be deleted.</param>
        /// <param name="serieName">The name of the serie.</param>
        /// <returns></returns>
        public InfluxDbResponse DeleteSeries(string database, string serieName)
        {
            return new InfluxDbResponse(_influxDbClient.DeleteSeries(database, serieName));
        }

        /// <summary>
        /// Force Database compaction.
        /// </summary>
        /// <returns></returns>
        public InfluxDbResponse ForceRaftCompaction()
        {
            return new InfluxDbResponse(_influxDbClient.ForceRaftCompaction());
        }

        /// <summary>
        /// List all interfaces influxDB is listening.
        /// </summary>
        /// <returns>A list of interface names.</returns>
        public List<string> Interfaces()
        {
            return _influxDbClient.Interfaces();
        }

        /// <summary>
        /// Sync the database to the filesystem.
        /// </summary>
        /// <returns>true|false if successful.</returns>
        public bool Sync()
        {
            return _influxDbClient.Sync();
        }

        /// <summary>
        /// List all servers which are member of the cluster.
        /// </summary>
        /// <returns>A list of all influxdb servers.</returns>
        public List<Server> ListServers()
        {
            return _influxDbClient.ListServers();
        }

        /// <summary>
        /// Remove the given Server from the cluster.
        /// </summary>
        /// <param name="id">The id of the server to remove.</param>
        /// <returns></returns>
        public InfluxDbResponse RemoveServers(int id)
        {
            return new InfluxDbResponse(_influxDbClient.RemoveServers(id));
        }

        /// <summary>
        /// Create a new Shard.
        /// </summary>
        /// <param name="shard">the new shard to create.</param>
        /// <returns></returns>
        [Obsolete("This functionality is gone with 0.8.0, will be removed in the next version.")]
        public InfluxDbResponse CreateShard(Shard shard)
        {
            return new InfluxDbResponse(_influxDbClient.CreateShard(shard));
        }

        /// <summary>
        /// Describe all existing shards.
        /// </summary>
        /// <returns>A list of all Shards.</returns>
        [Obsolete("This functionality is gone with 0.8.0, will be removed in the next version.")]
        public Shards GetShards()
        {
            return _influxDbClient.GetShards();
        }

        /// <summary>
        /// Drop the given shard.
        /// </summary>
        /// <param name="shard">The shard (<see cref="Shard"/>) to delete.</param>
        /// <returns></returns>
        [Obsolete("This functionality is gone with 0.8.0, will be removed in the next version.")]
        public InfluxDbResponse DropShard(Shard shard)
        {
            return new InfluxDbResponse(_influxDbClient.DropShard(shard.Id, shard.Shards.First()));
        }

        /// <summary>
        /// Describe all existing shardspaces.
        /// </summary>
        /// <returns>A list of all <see cref="ShardSpace"></see>'s.</returns>
        public List<ShardSpace> GetShardSpaces()
        {
            return _influxDbClient.GetShardSpaces();
        }

        /// <summary>
        /// Drop a existing ShardSpace from a Database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <param name="name">The name of the ShardSpace to delete.</param>
        /// <returns></returns>
        public InfluxDbResponse DropShardSpace(string database, string name)
        {
            return new InfluxDbResponse(_influxDbClient.DropShardSpace(database, name));
        }

        /// <summary>
        /// Create a ShardSpace in a Database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <param name="shardSpace">The shardSpace to create in this database</param>
        /// <returns></returns>
        public InfluxDbResponse CreateShardSpace(string database, ShardSpace shardSpace)
        {
            return new InfluxDbResponse(_influxDbClient.CreateShardSpace(database, shardSpace));
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

        private static String ToTimePrecision(TimeUnit.Unit t)
        {
            switch (t)
            {
                case TimeUnit.Unit.Seconds:
                    return "s";
                case TimeUnit.Unit.Milliseconds:
                    return "ms";
                case TimeUnit.Unit.Microseconds:
                    return "u";
                default:
                    throw new ArgumentException("time precision must be " + TimeUnit.Unit.Seconds + ", " + TimeUnit.Unit.Milliseconds + " or " + TimeUnit.Unit.Microseconds);
            }
        }
    }
}