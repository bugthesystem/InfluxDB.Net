using System;
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
            _client = new InfluxDb("http://principalstrickland-delorean-1.c.influxdb.com:8086", "root", "root");
            _sutDb = Guid.NewGuid().ToString("N").Substring(10);
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
            InfluxDbResponse response = _client.DeleteDatabase("AT");

            response.Success.Should().BeTrue();
        }
    }
}