InfluxDB.Net
============
>[InfluxDB](http://influxdb.com/) An open-source distributed time series database
with no external dependencies. It is the new home for all of your metrics, events, and analytics.

A .NET library to access the REST API of a [InfluxDB](http://influxdb.com/)  database.

####List of supported methods (Detailed documentation available soon)
- Ping();
- Version();
- CreateDatabase(Database database);
- CreateDatabase(DatabaseConfiguration config);
- DeleteDatabase(string name);
- DescribeDatabases();
- Write(string name, Serie[] series, string timePrecision);
- Query(String name, String query, String timePrecision);
- CreateClusterAdmin(User user);
- DeleteClusterAdmin(string name);
- DescribeClusterAdmins();
- UpdateClusterAdmin(User user, string name);
- CreateDatabaseUser(string database, User user);
- DeleteDatabaseUser(string database, string name);
- DescribeDatabaseUsers(String database);
- UpdateDatabaseUser(string database, User user, string name);
- AuthenticateDatabaseUser(string database, string user, string password);
- GetContinuousQueries(String database);
- DeleteContinuousQuery(string database, int id);
- DeleteSeries(string database, string name);
- ForceRaftCompaction();
- Interfaces();
- Sync();
- ListServers();
- RemoveServers(int id);
- CreateShard(Shard shard);
- GetShards();
- DropShard(int id, Shard.Member servers);
- GetShardSpaces();
- DropShardSpace(string database, string name);
- CreateShardSpace(string database, ShardSpace shardSpace);
