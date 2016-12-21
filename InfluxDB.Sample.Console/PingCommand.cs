using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InfluxDB.Net;
using InfluxDB.Net.Enums;
using Tharga.Toolkit.Console.Command.Base;

namespace InfluxDB.Sample.Console
{
    internal class PingCommand : ActionCommandBase
    {
        public PingCommand()
            : base("ping", "Ping InfluxDB server.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var url = QueryParam<string>("Url", GetParam(paramList, index++));
            var username = QueryParam<string>("Username", GetParam(paramList, index++));
            var password = QueryPassword("Password", GetParam(paramList, index++));
            var version = QueryParam<InfluxVersion>("Version", GetParam(paramList, index++), GetVersions());

            var client = new InfluxDb(url, username, password, version);
            var response = await client.PingAsync();

            OutputInformation($"Server ({response.Version}) responded in {response.ResponseTime.TotalMilliseconds.ToString("N2")} milliseconds.");

            return true;
        }

        private static Dictionary<InfluxVersion, string> GetVersions()
        {
            return new Dictionary<InfluxVersion,string>
            {
                { InfluxVersion.Auto, InfluxVersion.Auto.ToString() },
                { InfluxVersion.v1_1_x, InfluxVersion.v1_1_x.ToString() },
                { InfluxVersion.v013x, InfluxVersion.v013x.ToString() },
                { InfluxVersion.v012x, InfluxVersion.v012x.ToString() },
                { InfluxVersion.v011x, InfluxVersion.v011x.ToString() },
                { InfluxVersion.v010x, InfluxVersion.v010x.ToString() },
                { InfluxVersion.v09x, InfluxVersion.v09x.ToString() },
            };
        }
    }
}