using InfluxDB.Net.Contracts;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Infrastructure.Configuration;
using InfluxDB.Net.Infrastructure.Formatters;

namespace InfluxDB.Net
{
    internal class InfluxDbClientV096 : InfluxDbClientBase
    {
        public InfluxDbClientV096(InfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV096();
        }

        public override InfluxVersion GetVersion()
        {
            return InfluxVersion.v096;
        }
    }
}