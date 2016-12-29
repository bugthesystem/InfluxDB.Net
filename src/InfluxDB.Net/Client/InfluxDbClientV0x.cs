using InfluxDB.Net.Contracts;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Infrastructure.Configuration;
using InfluxDB.Net.Infrastructure.Formatters;

namespace InfluxDB.Net.Client
{
    internal class InfluxDbClientV0x : InfluxDbClientBase
    {
        public InfluxDbClientV0x(InfluxDbClientConfiguration configuration)
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV0x();
        }

        public override InfluxVersion GetVersion()
        {
            return InfluxVersion.v0x;
        }
    }
}