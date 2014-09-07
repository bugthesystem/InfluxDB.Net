using FluentAssertions;
using InfluxDB.Net.Core;
using NUnit.Framework;

namespace InfluxDB.Net.Tests
{
    public class ClientTests : TestBase
    {
        protected override void FinalizeSetUp()
        {
        }

        [Test]
        public void Connect_Success_Test()
        {
            IInfluxDb client = InfluxDbFactory.Connect("http://sandbox.influxdb.com:8083/", "root", "root");

            client.Should().NotBeNull();
        }
    }
}