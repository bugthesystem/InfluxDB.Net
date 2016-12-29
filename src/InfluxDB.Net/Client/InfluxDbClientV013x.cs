using InfluxDB.Net.Contracts;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Infrastructure.Configuration;
using InfluxDB.Net.Infrastructure.Formatters;

namespace InfluxDB.Net.Client
{
    internal class InfluxDbClientV013x : InfluxDbClientBase
    {
        public InfluxDbClientV013x(InfluxDbClientConfiguration configuration)
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV013x();
        }

        public override InfluxVersion GetVersion()
        {
            return InfluxVersion.v013x;
        }
    }
}