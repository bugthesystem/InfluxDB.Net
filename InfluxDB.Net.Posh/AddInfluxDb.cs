using System.Management.Automation;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Helpers;

namespace InfluxDB.Net.Posh
{
    [Cmdlet(VerbsCommon.Add, "InfluxDb")]
    public class AddInfluxDb : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public IInfluxDb Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var response = Connection.CreateDatabaseAsync(Name).Result;
            WriteObject(response.ToJson());
        }
    }
}