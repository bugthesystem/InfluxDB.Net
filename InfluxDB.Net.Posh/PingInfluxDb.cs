using System.Management.Automation;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Helpers;
using InfluxDB.Net.Models;

namespace InfluxDB.Net.Posh
{
    [Cmdlet(VerbsDiagnostic.Ping, "InfluxDb")]
    public class PingInfluxDb : Cmdlet
    {
        [Parameter(Mandatory = false)]
        public IInfluxDb Connection { get; set; }

        [Parameter(Mandatory = false)]
        public string Uri { get; set; }

        [Parameter(Mandatory = false)]
        public string User { get; set; }

        [Parameter(Mandatory = false)]
        public string Password { get; set; }

        protected override void ProcessRecord()
        {
            Pong response;

            if (Connection != null)
            {
                response = Connection.PingAsync().Result;
            }
            else if (Uri != null)
            {
                var db = new InfluxDb(Uri, User ?? "root", Password ?? "root");
                response = db.PingAsync().Result;
            }
            else
            {
                throw new InvalidJobStateException("Parameter Connection or Uri has to be provided.");
            }

            WriteObject(response.ToJson());
        }
    }
}