// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingInfluxDb.cs">
// </copyright>
// <summary>
//   Pings an InfluxDb
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Management.Automation;
using InfluxDB.Net.Models;

namespace InfluxDB.Net.Posh
{
    /// <summary>
    /// Opens a connection to an InfluxDB
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Ping, "InfluxDb")]
    public class PingInfluxDb : Cmdlet
    {
        /// <summary>
        /// A connection object to the InfluxDB instance
        /// </summary>
        [Parameter]
        public IInfluxDb dbConnection { get; set; }

        /// <summary>
        /// Ping the db 
        /// </summary>
        /// <returns>A pong object containing status and response time</returns>
        private Pong Ping()
        {
            var pong = this.dbConnection.PingAsync().Result;

            return pong;
        }

        /// <summary>
        /// Processes the pipeline
        /// </summary>
        protected override void ProcessRecord()
        {
            var pong = this.Ping();
            this.WriteObject(pong.ToJson());
        }
    }
}
