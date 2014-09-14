using System;
using System.Collections.Generic;
using FluentAssertions;
using InfluxDB.Net.Core;
using InfluxDB.Net.Models;
using NUnit.Framework;

namespace InfluxDB.Net.Tests
{
    public class ClientIntegrationTests : TestBase
    {
        private IInfluxDb _client;

        protected override void FinalizeSetUp()
        {
            _client = InfluxDb.Connect("http://principalstrickland-delorean-1.c.influxdb.com:8086", "root", "root");
        }

        [Test]
        public void Connect_Success_Test()
        {
            _client.Should().NotBeNull();
        }

        [Test]
        public void Create_DB_Success_Test()
        {
            _client.CreateDatabase(Guid.NewGuid().ToString("N").Substring(10));
        }

        [Test]
        public void DescribeDatabases_Success_Test()
        {
            List<Database> databases = _client.DescribeDatabases();
            databases.Should().NotBeNullOrEmpty();
        }
    }
}