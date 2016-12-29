using System;
using System.Collections.Generic;
using InfluxDB.Net.Enums;

namespace InfluxDB.Net.Models
{
    /// <summary>
    /// A class representing a time series point for db writes
    /// <see cref="https://github.com/influxdb/influxdb/blob/master/tsdb/README.md" />
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Serie name. Measurement is Influxes convention for Serie name.
        /// <see cref="https://influxdb.com/docs/v0.9/write_protocols/write_syntax.html"/>
        /// </summary>
        public string Measurement { get; set; }
        public Dictionary<string, object> Tags { get; set; }  // string, string?
        public Dictionary<string, object> Fields { get; set; }
        public TimeUnit Precision { get; set; }
        public DateTime? Timestamp { get; set; }

        public Point()
        {
            Tags = new Dictionary<string, object>();
            Fields = new Dictionary<string, object>();
            Precision = TimeUnit.Milliseconds;
        }
    }
}
