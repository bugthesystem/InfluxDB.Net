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
        private IInfluxDb _client;

        protected override void FinalizeSetUp()
        {
            _client = new InfluxDb("http://192.168.59.103:8086", "root", "root");

            //TODO: Start docker container and kill it.
            //https://registry.hub.docker.com/u/tutum/influxdb/
            //https://github.com/ahmetalpbalkan/Docker.DotNet

            EnsureInfluxDbStarted();
        }

        [Test]
        public async void Ping_Test()
        {
            Pong pong = await _client.PingAsync();
            pong.Should().NotBeNull();
            pong.Status.Should().BeEquivalentTo("ok");
        }

        [Test]
        public async void Create_DB_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse response = await _client.CreateDatabaseAsync(dbName);

            InfluxDbApiDeleteResponse deleteResponse = await _client.DeleteDatabaseAsync(dbName);

            response.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Create_DB_With_Config_Test()
        {
            string dbName = Guid.NewGuid().ToString("N").Substring(10);

            InfluxDbApiCreateResponse response = await _client.CreateDatabaseAsync(new DatabaseConfiguration
            {
                Name = dbName
            });


            InfluxDbApiDeleteResponse deleteResponse = await _client.DeleteDatabaseAsync(dbName);

            response.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void DescribeDatabases_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse createResponse = await _client.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            List<Database> databases = await _client.DescribeDatabasesAsync();


            InfluxDbApiDeleteResponse deleteResponse = await _client.DeleteDatabaseAsync(dbName);

            databases.Should().NotBeNullOrEmpty();
            databases.Where(database => database.name.Equals(dbName)).Should().NotBeNull();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Delete_Database_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse createResponse = await _client.CreateDatabaseAsync(dbName);

            createResponse.Success.Should().BeTrue();

            InfluxDbApiDeleteResponse response = await _client.DeleteDatabaseAsync(dbName);

            response.Success.Should().BeTrue();
        }

        [Test]
        public async void Write_DB_Test()
        {
            var dbName = GetNewDbName();

            InfluxDbApiCreateResponse createResponse = await _client.CreateDatabaseAsync(dbName);

            Serie serie = new Serie.Builder("testSeries")
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
            InfluxDbApiResponse writeResponse = await _client.WriteAsync(dbName, TimeUnit.Milliseconds, serie);

            InfluxDbApiDeleteResponse deleteResponse = await _client.DeleteDatabaseAsync(dbName);

            createResponse.Success.Should().BeTrue();
            writeResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Query_DB_Test()
        {
            var dbName = GetNewDbName();

            InfluxDbApiCreateResponse createResponse = await _client.CreateDatabaseAsync(dbName);

            const string TMP_SERIE_NAME = "testSeries";
            Serie serie = new Serie.Builder(TMP_SERIE_NAME)
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
            InfluxDbApiResponse writeResponse = await _client.WriteAsync(dbName, TimeUnit.Milliseconds, serie);

            List<Serie> series = await _client.QueryAsync(dbName, string.Format("select * from {0}", TMP_SERIE_NAME), TimeUnit.Milliseconds);

            InfluxDbApiDeleteResponse deleteResponse = await _client.DeleteDatabaseAsync(dbName);

            series.Should().NotBeNull();
            series.Count.Should().Be(1);

            createResponse.Success.Should().BeTrue();
            writeResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
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
                    Pong response = await _client.PingAsync();
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
            Console.WriteLine("#  Connected to InfluxDB Version: " + await _client.VersionAsync() + " #");
            Console.WriteLine("##################################################################################");
        }

        private static string GetNewDbName()
        {
            return Guid.NewGuid().ToString("N").Substring(10);
        }
    }
}