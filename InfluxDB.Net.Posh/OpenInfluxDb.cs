using System.Management.Automation;

namespace InfluxDB.Net.Posh
{
    [Cmdlet(VerbsCommon.Open, "InfluxDb")]
    public class OpenInfluxDb : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Uri { get; set; }

        [Parameter(Mandatory = false)]
        public string User { get; set; }

        [Parameter(Mandatory = false)]
        public string Password { get; set; }

        protected override void ProcessRecord()
        {
            var response = new InfluxDb(Uri, User ?? "root", Password ?? "root");
            WriteObject(response);
        }
    }
}