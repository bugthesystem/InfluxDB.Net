//using System;
//using System.Linq;
//using System.Threading;
//using NUnit.Framework;
//using FluentAssertions;
//using InfluxDB.Net.Core;
//using InfluxDB.Net.Models;
//using System.Collections.Generic;

//namespace InfluxDB.Net.Tests
//{
//    public class InfluxDbTests : TestBase
//    {
//        private IInfluxDb _client;

//        protected override void FinalizeSetUp()
//        {
//            _client = new InfluxDb("http://192.168.59.103:8086", "root", "root");

//            //TODO: Start docker container and kill it.
//            //https://registry.hub.docker.com/u/tutum/influxdb/
//            //https://github.com/ahmetalpbalkan/Docker.DotNet

//            EnsureInfluxDbStarted();
//        }

//        [Test]
//        public void Ping_Test()
//        {
//            Pong pong = _client.Ping();
//            pong.Should().NotBeNull();
//            pong.Status.Should().BeEquivalentTo("ok");
//        }

//        [Test]
//        public void Create_DB_Test()
//        {
//            string dbName = GetNewDbName();
//            CreateResponse response = _client.CreateDatabase(dbName);

//            DeleteResponse deleteResponse = _client.DeleteDatabase(dbName);

//            response.Success.Should().BeTrue();
//            deleteResponse.Success.Should().BeTrue();
//        }

//        [Test]
//        public void Create_DB_With_Config_Test()
//        {
//            string dbName = Guid.NewGuid().ToString("N").Substring(10);

//            CreateResponse response = _client.CreateDatabase(new DatabaseConfiguration
//            {
//                Name = dbName
//            });


//            DeleteResponse deleteResponse = _client.DeleteDatabase(dbName);

//            response.Success.Should().BeTrue();
//            deleteResponse.Success.Should().BeTrue();
//        }

//        [Test]
//        public void DescribeDatabases_Test()
//        {
//            string dbName = GetNewDbName();
//            CreateResponse createResponse = _client.CreateDatabase(dbName);
//            createResponse.Success.Should().BeTrue();

//            List<Database> databases = _client.DescribeDatabases();


//            DeleteResponse deleteResponse = _client.DeleteDatabase(dbName);

//            databases.Should().NotBeNullOrEmpty();
//            databases.Where(database => database.name.Equals(dbName)).Should().NotBeNull();
//            deleteResponse.Success.Should().BeTrue();
//        }

//        [Test]
//        public void Delete_Database_Test()
//        {
//            string dbName = GetNewDbName();
//            CreateResponse createResponse = _client.CreateDatabase(dbName);

//            createResponse.Success.Should().BeTrue();

//            DeleteResponse response = _client.DeleteDatabase(dbName);

//            response.Success.Should().BeTrue();
//        }

//        [Test]
//        public void Write_DB_Test()
//        {
//            var dbName = GetNewDbName();

//            CreateResponse createResponse = _client.CreateDatabase(dbName);

//            Serie serie = new Serie.Builder("testSeries")
//                .Columns("value1", "value2")
//                .Values(DateTime.Now.Millisecond, 5)
//                .Build();
//            InfluxDbResponse writeResponse = _client.Write(dbName, TimeUnit.Milliseconds, serie);

//            DeleteResponse deleteResponse = _client.DeleteDatabase(dbName);

//            createResponse.Success.Should().BeTrue();
//            writeResponse.Success.Should().BeTrue();
//            deleteResponse.Success.Should().BeTrue();
//        }

//        [Test]
//        public void Query_DB_Test()
//        {
//            var dbName = GetNewDbName();

//            CreateResponse createResponse = _client.CreateDatabase(dbName);

//            const string TMP_SERIE_NAME = "testSeries";
//            Serie serie = new Serie.Builder(TMP_SERIE_NAME)
//                .Columns("value1", "value2")
//                .Values(DateTime.Now.Millisecond, 5)
//                .Build();
//            InfluxDbResponse writeResponse = _client.Write(dbName, TimeUnit.Milliseconds, serie);

//            List<Serie> series = _client.Query(dbName, string.Format("select * from {0}", TMP_SERIE_NAME), TimeUnit.Milliseconds);

//            DeleteResponse deleteResponse = _client.DeleteDatabase(dbName);

//            series.Should().NotBeNull();
//            series.Count.Should().Be(1);

//            createResponse.Success.Should().BeTrue();
//            writeResponse.Success.Should().BeTrue();
//            deleteResponse.Success.Should().BeTrue();
//        }



//        protected override void FinalizeTearDown()
//        {
//            //TODO: KILL CONTAINER
//        }

//        private void EnsureInfluxDbStarted()
//        {
//            //TODO: Start influxdb docker container.
//            //
//            bool influxDBstarted = false;
//            do
//            {
//                try
//                {
//                    Pong response = _client.Ping();
//                    if (response.Status.Equals("ok"))
//                    {
//                        influxDBstarted = true;
//                    }
//                }
//                catch (Exception e)
//                {
//                    // NOOP intentional
//                }
//                Thread.Sleep(100);
//            } while (!influxDBstarted);

//            Console.WriteLine("##################################################################################");
//            Console.WriteLine("#  Connected to InfluxDB Version: " + _client.Version() + " #");
//            Console.WriteLine("##################################################################################");
//        }

//        private static string GetNewDbName()
//        {
//            return Guid.NewGuid().ToString("N").Substring(10);
//        }
//    }
//}