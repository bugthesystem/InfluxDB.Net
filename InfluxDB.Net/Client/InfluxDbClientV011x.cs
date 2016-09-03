using InfluxDB.Net.Contracts;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Infrastructure.Configuration;
using InfluxDB.Net.Infrastructure.Formatters;

namespace InfluxDB.Net.Client
{
    internal class InfluxDbClientV011x : InfluxDbClientBase
    {
        public InfluxDbClientV011x(InfluxDbClientConfiguration configuration)
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV011x();
        }

        public override InfluxVersion GetVersion()
        {
            return InfluxVersion.v011x;
        }
    }
}