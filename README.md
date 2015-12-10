InfluxDB.Net
============
####Update for 0.9.5 API changes

>[InfluxDB](http://influxdb.com/) An open-source distributed time series database
with no external dependencies. It is the new home for all of your metrics, events, and analytics.

A Portable .NET library to access the REST API of a [InfluxDB](http://influxdb.com/)  database.

**Original NuGet**  
This is a fork of [InfluxDB.Net](https://github.com/ziyasal/InfluxDB.Net/) NuGet library which currently seems to be in hibernation. I took whatever was out there (including improvements from other forks) did some refactoring on the codebase and plan on implementing the rest of the InfluxDB API.

**Installation**  
I did not create a separate NuGet for this fork so you will have to download the .sln, build it in release and add it to your project. Unit tests work, and the Influx API calls that were implemented seem to work fine.

**InfluxDB v0.9.6 and the TICK stack**  
Since Influx released InfluxDB v0.9.6 and [announced](https://influxdata.com/blog/influxdb-the-platform-for-time-series-data/) their TICK stack, I plan on creating an umbrella NuGet which would support all components of the TICK stack.

####List of supported methods (More documentation available soon)
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
  Pong pong =await _client.PingAsync();
```
## Version
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
  string version =await  _client.VersionAsync();
```
## Create Database
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
 InfluxDbApiCreateResponse response =await  _client.CreateDatabaseAsync("MyDb");
 //Or
 InfluxDbApiCreateResponse response = await _client.CreateDatabaseAsync(new DatabaseConfiguration
            {
                Name = "MyDb"
            });
```
## Delete Database
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
InfluxDbApiDeleteResponse deleteResponse = await _client.DeleteDatabaseAsync("MyDb");
```
## Describe Databases
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
List<Database> databases = await _client.DescribeDatabasesAsync();
```
## Write
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
Serie serie = new Serie.Builder("testSeries")
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
InfluxDbApiResponse writeResponse =await _client.WriteAsync("MyDb", TimeUnit.Milliseconds, serie);
```

## Query
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
 List<Serie> series = await _client.QueryAsync("MyDb", "select * from testSeries"), TimeUnit.Milliseconds);
```

##Bugs
If you encounter a bug, performance issue, or malfunction, please add an [Issue](https://github.com/ziyasal/InfluxDB.Net/issues) with steps on how to reproduce the problem.

##TODO
- Add more tests
- Add more documentation

##License

Code and documentation are available according to the *MIT* License (see [LICENSE](https://github.com/ziyasal/InfluxDB.Net/blob/master/LICENSE)).
