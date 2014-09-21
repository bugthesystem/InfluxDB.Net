using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using FluentAssertions;
using InfluxDB.Net.Core;
using InfluxDB.Net.Models;
using System.Collections.Generic;

namespace InfluxDB.Net.Tests
{
    public class ClientIntegrationTests : TestBase
    {
        private IInfluxDb _client;

        protected override void FinalizeSetUp()
        {
            _client = new InfluxDb("http://192.168.59.103:8086", "root", "root");

            //TODO: Start docker container and kill it on tear down using Ahmet Alp BALKAN's Docker.Net library
            EnsureInfluxDbStarted();
        }

        [Test]
        public void Ping_Test()
        {
            Pong pong = _client.Ping();
            pong.Should().NotBeNull();
            pong.Status.Should().BeEquivalentTo("ok");
        }

        [Test]
        public void Create_DB_Test()
        {
            string dbToCreate = GetNewDbName();
            CreateDbResponse response = _client.CreateDatabase(dbToCreate);

            response.Success.Should().BeTrue();
        }

        [Test]
        public void Create_DB_With_Config_Test()
        {
            string dbToCreate = Guid.NewGuid().ToString("N").Substring(10);

            CreateDbResponse response = _client.CreateDatabase(new DatabaseConfiguration
            {
                Name = dbToCreate
            });

            response.Success.Should().BeTrue();
        }

        [Test]
        public void DescribeDatabases_Test()
        {
            string dbToCreate = GetNewDbName();
            CreateDbResponse createDbResponse = _client.CreateDatabase(dbToCreate);
            createDbResponse.Success.Should().BeTrue();

            List<Database> databases = _client.DescribeDatabases();
            databases.Should().NotBeNullOrEmpty();
            databases.Where(database => database.name.Equals(dbToCreate)).Should().NotBeNull();
        }

        [Test]
        public void Delete_Database_Test()
        {
            string dbToDelete = GetNewDbName();
            CreateDbResponse createDbResponse = _client.CreateDatabase(dbToDelete);

            createDbResponse.Success.Should().BeTrue();

            DeleteDbResponse response = _client.DeleteDatabase(dbToDelete);

            response.Success.Should().BeTrue();
        }

        [Test]
        public void Write_DB_Test()
        {

        }

        protected override void FinalizeTearDown()
        {
            //TODO: KILL CONTAINER
        }

        private void EnsureInfluxDbStarted()
        {
            //TODO: Start influxdb docker container.
            //
            bool influxDBstarted = false;
            do
            {
                try
                {
                    Pong response = _client.Ping();
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
            Console.WriteLine("#  Connected to InfluxDB Version: " + _client.Version() + " #");
            Console.WriteLine("##################################################################################");
        }

        private static string GetNewDbName()
        {
            return Guid.NewGuid().ToString("N").Substring(10);
        }
    }
}