InfluxDB.Net
============
>[InfluxDB](http://influxdb.com/) An open-source distributed time series database
with no external dependencies. It is the new home for all of your metrics, events, and analytics.

A .NET library to access the REST API of a [InfluxDB](http://influxdb.com/)  database.

####List of supported methods (Detailed documentation available soon)
- [Ping](#ping)
- [Version](#version)
- [CreateDatabase](#create-database)
- [DeleteDatabase](#delete-database)
- [DescribeDatabases](#describe-databases)
- [Write](#write)
- [Query](#query)
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

## Ping
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
  Pong pong = _client.Ping();
```
## Version
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
  Pong pong = _client.Version();
```
## Create Database
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
 CreateResponse response = _client.CreateDatabase("MyDb");
 //Or
 CreateResponse response = _client.CreateDatabase(new DatabaseConfiguration
            {
                Name = "MyDb"
            });
```
## Delete Database
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
DeleteResponse deleteResponse = _client.DeleteDatabase("MyDb");
```
## Describe Databases
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
List<Database> databases = _client.DescribeDatabases();
```
## Write
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
Serie serie = new Serie.Builder("testSeries")
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
InfluxDbResponse writeResponse = _client.Write("MyDb", TimeUnit.Milliseconds, serie);
```

## Query
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
 List<Serie> series = _client.Query("MyDb", "select * from testSeries"), TimeUnit.Milliseconds);
```

##Bugs
If you encounter a bug, performance issue, or malfunction, please add an [Issue](https://github.com/ziyasal/InfluxDB.Net/issues) with steps on how to reproduce the problem.

##TODO
- Add more tests
- Add more documentation

##Open Source  Projects in Use
- [RestSharp](https://github.com/restsharp/RestSharp) by RestSharp Contributors

##License

Code and documentation are available according to the *MIT* License (see [LICENSE](https://github.com/ziyasal/InfluxDB.Net/blob/master/LICENSE)).
