// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WriteInfluxDb.cs">
// </copyright>
// <summary>
//   Writes data to an InfluxDb
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
    /// Writes data to an InfluxDb
    /// </summary>
    [Cmdlet(VerbsCommunications.Write, "InfluxDb")]
    public class WriteInfluxDb : Cmdlet
    {
        /// <summary>
        /// The connection for the InfluxDB instance
        /// </summary>
        [Parameter]
        public IInfluxDb dbConnection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string dbName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string seriesName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string[] columns { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public object[] values { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InfluxDbApiResponse Write()
        {
            Serie serie = new Serie.Builder(seriesName)
                .Columns(columns)
                .Values(values)
                .Build();
            InfluxDbApiResponse writeResponse = dbConnection.WriteAsync(dbName, TimeUnit.Milliseconds, serie).Result;
            return writeResponse;
        }

        /// <summary>
        /// Processes the pipeline
        /// </summary>
        protected override void ProcessRecord()
        {
            var writeResponse = this.Write();
            this.WriteObject(writeResponse.ToJson());
        }
    }
}
