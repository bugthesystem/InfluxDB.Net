using System;
using System.Collections.Generic;
using System.Linq;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Helpers;
using InfluxDB.Net.Infrastructure;
using InfluxDB.Net.Infrastructure.Validation;

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
        public Dictionary<string, object> Tags { get; set; }
        public Dictionary<string, object> Fields { get; set; }
        public TimeUnit Precision { get; set; }
        public DateTime? Timestamp { get; set; }

        public static readonly string QueryTemplate = "{0} {1} {2}"; // [key] [fields] [time]

        public Point()
        {
            Tags = new Dictionary<string, object>();
            Fields = new Dictionary<string, object>();
            Precision = TimeUnit.Milliseconds;
        }

        /// <summary>
        /// Returns a point represented in line protocol format for writing to the InfluxDb API endpoint
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        /// <remarks>
        /// Example outputs:
        /// cpu,host=serverA,region=us_west value = 0.64
        /// payment,device=mobile,product=Notepad,method=credit billed = 33, licenses = 3i 1434067467100293230
        /// stock,symbol=AAPL bid = 127.46, ask = 127.48
        /// temperature,machine=unit42,type=assembly external = 25,internal=37 1434067467000000000
        /// </remarks>
        public override string ToString()
        {
            Check.NotNullOrEmpty(Measurement, "measurement");
            Check.NotNull(Tags, "tags");
            Check.NotNull(Fields, "fields");

            var tags = string.Join(",", Tags.Select(t => Format(t.Key, t.Value)));
            var fields = string.Join(",", Fields.Select(t => Format(t.Key, t.Value)));

            // TODO: refactor - split key into measurement + tags
            var key = string.IsNullOrEmpty(tags) ? Escape(Measurement) : string.Join(",", Escape(Measurement), tags);
            var time = Timestamp.HasValue ? Timestamp.Value.ToUnixTime().ToString() : string.Empty;

            var result = string.Format(QueryTemplate, key, fields, time);

            return result;
        }

        private string Format(string key, object value)
        {
            Check.NotNullOrEmpty(key, "key");
            Check.NotNull(value, "value");

            var valueType = value.GetType();

            // Format and escape the values
            var stringValue = value.ToString();

            // surround strings with quotes
            if (valueType == typeof(string))
            {
                stringValue = Escape(value.ToString());
            }
            // api needs lowercase booleans
            else if (valueType == typeof(bool))
            {
                stringValue = value.ToString();
            }
            // InfluxDb does not support a datetime type for fields or tags
            // convert datetime to unix long
            else if (valueType == typeof(DateTime))
            {
                stringValue = ((DateTime)value).ToUnixTime().ToString();
            }
            // TODO: what about number types?

            return string.Join("=", Escape(key), stringValue);
        }

        //private string Quote(string value)
        //{
        //    return "\"" + value + "\"";
        //}

        private string Escape(string value)
        {
            var result = value
                // literal backslash escaping is broken
                // https://github.com/influxdb/influxdb/issues/3070
                //.Replace(@"\", @"\\")
                .Replace(@"\", @"") // NOTE: temporary fix - fully remove \ from string
                .Replace(@"""", @"\""") // TODO: check if this is right or if "" should become \"\"
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }
    }
}
