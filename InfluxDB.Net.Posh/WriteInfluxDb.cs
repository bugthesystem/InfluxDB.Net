using System.Management.Automation;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Helpers;
using InfluxDB.Net.Models;

namespace InfluxDB.Net.Posh
{
    [Cmdlet(VerbsCommunications.Write, "InfluxDb")]
    public class WriteInfluxDb : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public IInfluxDb Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = true)]
        public string Data { get; set; }

        protected override void ProcessRecord()
        {
            var point = Data.FromJson<Point>();
            var response = Connection.WriteAsync(Name, point).Result;
            WriteObject(response.ToJson());
        }
    }
}