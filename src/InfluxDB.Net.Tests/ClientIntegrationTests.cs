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
        private string _dbName;

        protected override void FinalizeSetUp()
        {
            _client = new InfluxDb("http://principalstrickland-delorean-1.c.influxdb.com:8086", "root", "root");
            _dbName = Guid.NewGuid().ToString("N").Substring(10);
        }

        [Test]
        public void Connect_Success_Test()
        {
            _client.Should().NotBeNull();
        }

        [Test]
        public void Create_DB_Success_Test()
        {
            CreateDbResponse response = _client.CreateDatabase(_dbName);

            response.Success.Should().BeTrue();
        }

        [Test]
        public void DescribeDatabases_Success_Test()
        {
            List<Database> databases = _client.DescribeDatabases();
            databases.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void DeleteDb_Success_Test()
        {
            InfluxDbResponse response = _client.DeleteDatabase("AT");

            response.Success.Should().BeTrue();
        }
    }
}