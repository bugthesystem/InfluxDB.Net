// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingInfluxDb.cs">
// </copyright>
// <summary>
//   Pings an InfluxDb
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace InfluxPS
{
    using System;
    using System.Management.Automation;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using InfluxDB.Net;
    using InfluxDB.Net.Models;

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
