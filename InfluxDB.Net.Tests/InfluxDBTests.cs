using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using InfluxDB.Net.Models;
using NUnit.Framework;

namespace InfluxDB.Net.Tests
{
    public class InfluxDbTests : TestBase
    {
        private IInfluxDb _db;

        protected override void FinalizeSetUp()
        {
            _db = new InfluxDb("http://localhost:8086", "root", "root");
            EnsureInfluxDbStarted();
        }

        protected override void FinalizeTearDown()
        {
            //TODO: KILL CONTAINER
        }

        private async void EnsureInfluxDbStarted()
        {
            //TODO: Start influxdb docker container.
            //
            bool influxDBstarted = false;
            do
            {
                try
                {
                    Pong response = await _db.PingAsync();
                    if (response.Status.Equals("ok"))
                    {
                        influxDBstarted = true;
                    }
                }
                catch (Exception e)
                {
                    // NOOP intentional
                }
                Thread.Sleep(100);
            } while (!influxDBstarted);

            Console.WriteLine("##################################################################################");
            Console.WriteLine("#  Connected to InfluxDB Version: " + await _db.VersionAsync() + " #");
            Console.WriteLine("##################################################################################");
        }

        private static string GetNewDbName()
        {
            return Guid.NewGuid().ToString("N").Substring(10);
        }

        [Test]
        public async void Create_DB_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse response = await _db.CreateDatabaseAsync(dbName);

            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);

            response.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Create_DB_With_Config_Test()
        {
            string dbName = Guid.NewGuid().ToString("N").Substring(10);

            InfluxDbApiCreateResponse response = await _db.CreateDatabaseAsync(new DatabaseConfiguration
            {
                Name = dbName
            });


            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);

            response.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Delete_Database_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);

            createResponse.Success.Should().BeTrue();

            InfluxDbApiDeleteResponse response = await _db.DeleteDatabaseAsync(dbName);

            response.Success.Should().BeTrue();
        }

        [Test]
        public async void DescribeDatabases_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            List<Database> databases = await _db.DescribeDatabasesAsync();


            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);

            databases.Should().NotBeNullOrEmpty();
            databases.Where(database => database.Name.Equals(dbName)).Should().NotBeNull();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Ping_Test()
        {
            Pong pong = await _db.PingAsync();
            pong.Should().NotBeNull();
            pong.Status.Should().BeEquivalentTo("ok");
        }

        [Test]
        public async void Query_DB_Test()
        {
            string dbName = GetNewDbName();

            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);

            const string TMP_SERIE_NAME = "testSeries";
            Serie serie = new Serie.Builder(TMP_SERIE_NAME)
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
            InfluxDbApiResponse writeResponse = await _db.WriteAsync(dbName, TimeUnit.Milliseconds, serie);

            List<Serie> series =
                await _db.QueryAsync(dbName, string.Format("select * from {0}", TMP_SERIE_NAME), TimeUnit.Milliseconds);

            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);

            series.Should().NotBeNull();
            series.Count.Should().Be(1);

            createResponse.Success.Should().BeTrue();
            writeResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Write_DB_Test()
        {
            string dbName = GetNewDbName();

            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);

            Serie serie = new Serie.Builder("testSeries")
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
            InfluxDbApiResponse writeResponse = await _db.WriteAsync(dbName, TimeUnit.Milliseconds, serie);

            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);

            createResponse.Success.Should().BeTrue();
            writeResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }
    }
}