using System;
using System.Globalization;
using System.Linq;
using InfluxDB.Net.Models;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Infrastructure.Validation;
using InfluxDB.Net.Helpers;

namespace InfluxDB.Net.Infrastructure.Formatters
{
    internal class FormatterBase : IFormatter
    {
        private static readonly string _queryTemplate = "{0} {1} {2}"; // [key] [fields] [time]

        public virtual string GetLineTemplate()
        {
            return _queryTemplate;
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
        public virtual string PointToString(Point point)
        {
            Validate.NotNullOrEmpty(point.Measurement, "measurement");
            Validate.NotNull(point.Tags, "tags");
            Validate.NotNull(point.Fields, "fields");

            var tags = String.Join(",", point.Tags.Select(t => String.Join("=", t.Key, EscapeTagValue(t.Value.ToString()))));
            var fields = String.Join(",", point.Fields.Select(t => FormatPointField(t.Key, t.Value)));

            var key = String.IsNullOrEmpty(tags) ? EscapeNonTagValue(point.Measurement) : String.Join(",", EscapeNonTagValue(point.Measurement), tags);
            var ts = point.Timestamp.HasValue ? point.Timestamp.Value.ToUnixTime().ToString() : string.Empty;

            var result = String.Format(GetLineTemplate(), key, fields, ts);

            return result;
        }

        public virtual Serie PointToSerie(Point point)
        {
            var s = new Serie
            {
                Name = point.Measurement
            };

            foreach (var key in point.Tags.Keys.ToList())
            {
                s.Tags.Add(key, point.Tags[key].ToString());
            }

            var sortedFields = point.Fields.OrderBy(k => k.Key).ToDictionary(x => x.Key, x => x.Value);

            s.Columns = new string[] { "time" }.Concat(sortedFields.Keys).ToArray();

            s.Values = new object[][]
            {
                new object[] { point.Timestamp }.Concat(sortedFields.Values).ToArray()
            };

            return s;
        }

        protected virtual string FormatPointField(string key, object value)
        {
            Validate.NotNullOrEmpty(key, "key");
            Validate.NotNull(value, "value");

            // Format and escape the values
            var result = value.ToString();

            // surround strings with quotes
            if (value.GetType() == typeof(string))
            {
                result = QuoteValue(EscapeNonTagValue(value.ToString()));
            }
            // api needs lowercase booleans
            else if (value.GetType() == typeof(bool))
            {
                result = value.ToString().ToLower();
            }
            // InfluxDb does not support a datetime type for fields or tags
            // convert datetime to unix long
            else if (value.GetType() == typeof(DateTime))
            {
                result = ((DateTime)value).ToUnixTime().ToString();
            }
            // For cultures using other decimal characters than '.'
            else if (value.GetType() == typeof(decimal))
            {
                result = ((decimal)value).ToString("0.0###################", CultureInfo.InvariantCulture);
            }
            else if (value.GetType() == typeof(float))
            {
                result = ((float)value).ToString("0.0###################", CultureInfo.InvariantCulture);
            }
            else if (value.GetType() == typeof(double))
            {
                result = ((double)value).ToString("0.0###################", CultureInfo.InvariantCulture);
            }
            else if (value.GetType() == typeof(long) || value.GetType() == typeof(int))
            {
                result = ToInt(result);
            }

            return String.Join("=", EscapeNonTagValue(key), result);
        }

        protected virtual string ToInt(string result)
        {
            return result + "i";
        }

        protected virtual string EscapeNonTagValue(string value)
        {
            Validate.NotNull(value, "value");

            var result = value
                // literal backslash escaping is broken
                // https://github.com/influxdb/influxdb/issues/3070
                //.Replace(@"\", @"\\")
                .Replace(@"""", @"\""") // TODO: check if this is right or if "" should become \"\"
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }

        protected virtual string EscapeTagValue(string value)
        {
            Validate.NotNull(value, "value");

            var result = value
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }

        protected virtual string QuoteValue(string value)
        {
            Validate.NotNull(value, "value");

            return "\"" + value + "\"";
        }
    }
}