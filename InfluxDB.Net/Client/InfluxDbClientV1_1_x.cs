using InfluxDB.Net.Contracts;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Infrastructure.Configuration;
using InfluxDB.Net.Infrastructure.Formatters;

namespace InfluxDB.Net.Client
{
    internal class InfluxDbClientV1_1_x : InfluxDbClientBase
    {
        public InfluxDbClientV1_1_x(InfluxDbClientConfiguration configuration)
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV1_1_x();
        }

        public override InfluxVersion GetVersion()
        {
            return InfluxVersion.v1_1_x;
        }
    }
}