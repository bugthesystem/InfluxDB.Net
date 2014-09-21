using System;
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
        private string _sutDb;

        protected override void FinalizeSetUp()
        {
            _client = new InfluxDb("http://192.168.59.103:8086", "root", "root");
            _sutDb = Guid.NewGuid().ToString("N").Substring(10);

            //TODO: Start docker container and kill it on tear down.
            // Use Ahmet Alp BALKAN's Docker.Net library
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
            CreateDbResponse response = _client.CreateDatabase(_sutDb);

            response.Success.Should().BeTrue();
        }

        [Test]
        public void Create_DB_WithConfig_Test()
        {
            CreateDbResponse response = _client.CreateDatabase(new DatabaseConfiguration
            {
                Name = Guid.NewGuid().ToString("N").Substring(10)
            });

            response.Success.Should().BeTrue();
        }

        [Test]
        public void Write_DB_Test()
        {

        }

        [Test]
        public void DescribeDatabases_Test()
        {
            List<Database> databases = _client.DescribeDatabases();
            databases.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Delete_Database_Test()
        {
            InfluxDbResponse response = _client.DeleteDatabase("734b19891049933abe7ac9");

            response.Success.Should().BeTrue();
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

        protected override void FinalizeTearDown()
        {

        }
    }
}