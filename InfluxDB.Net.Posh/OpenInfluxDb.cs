// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenInfluxDb.cs">
// </copyright>
// <summary>
//   Opens a connection to an InfluxDb
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Management.Automation;

namespace InfluxDB.Net.Posh
{
    /// <summary>
    /// Opens a connection to an InfluxDB
    /// </summary>
    [Cmdlet(VerbsCommon.Open, "InfluxDb")]
    public class OpenInfluxDb : Cmdlet
    {
        /// <summary>
        /// The Uri of the InfluxDB instance
        /// </summary>
        [Parameter]
        public string Uri { get; set; }

        /// <summary>
        /// The user name with credentials to access the instance
        /// </summary>
        [Parameter]
        public string User { get; set; }

        /// <summary>
        /// The password for the user
        /// </summary>
        [Parameter]
        public string Password { get; set; }

        /// <summary>
        /// Processes the pipeline
        /// </summary>
        protected override void ProcessRecord()
        {
            var db = new InfluxDb(this.Uri, this.User, this.Password);

            this.WriteObject(db);
        }
    }
}
