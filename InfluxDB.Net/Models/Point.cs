using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InfluxDB.Net.Models
{
	/// <summary>
	/// A class representing a time series point for db writes
	/// <see cref="https://github.com/influxdb/influxdb/blob/master/tsdb/README.md" /></summary>
	public class Point
	{
		public string Name { get; set; }
		public Dictionary<string, string> Tags { get; set; }
		public DateTime? Timestamp { get; set; }
		public TimeUnit Precision { get; set; }
		public Dictionary<string, object> Fields { get; set; }

		public static readonly string LineTemplate = "{0} {1} {2}"; // [key] [fields] [timestamp]

		public Point()
		{
			Tags = new Dictionary<string, string>();
			Fields = new Dictionary<string, object>();
			Precision = TimeUnit.Milliseconds;
		}

		/// <summary>
		/// Returns a point represented in line protocol format for writing to the InfluxDb API endpoint
		/// </summary>
		/// <returns>A string that represents this instance.</returns>
		/// <remarks>
		/// Examples:
		/// cpu,host=serverA,region=us-west value=1.0 10000000000
		/// cpu,host=serverB,region=us-west value=3.3
		/// cpu,host=serverB,region=us-east user=123415235,event="overloaded" 20000000000
		/// mem,host=serverB,region=us-east swapping=true 2000000000
		/// my\ a\/\/esome\ series,tag=foo\ bar "my-field"="\=*\=" 2000000000
		/// </remarks>
		public override string ToString()
		{
			Check.NotNullOrEmpty(Name, "name");
			Check.NotNull(Tags, "tags");
			Check.NotNull(Fields, "fields");

			var tags = string.Join(",", Tags.Select(t => Format(t.Key, t.Value)));
			var fields = string.Join(",", Fields.Select(t => Format(t.Key, t.Value)));

			var key = string.IsNullOrEmpty(tags) ? Escape(Name) : string.Join(",", Escape(Name), tags);
			var ts = Timestamp.HasValue ? Timestamp.Value.ToUnixTime().ToString() : string.Empty;

			var result = string.Format(LineTemplate, key, fields, ts);

			return result;
		}

		private string Format(string key, object value)
		{
			Check.NotNullOrEmpty(key, "key");
			Check.NotNull(value, "value");

			// Format and escape the values
			var result = value.ToString();

			// surround strings with quotes
			if (value.GetType() == typeof(string))
			{
				result = Quote(Escape(value.ToString()));
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
				result = ((decimal)value).ToString(CultureInfo.InvariantCulture);
			}
			else if (value.GetType() == typeof(float))
			{
				result = ((float)value).ToString(CultureInfo.InvariantCulture);
			}

			return string.Join("=", Escape(key), result);
		}

		private string Quote(string value)
		{
			Check.NotNull(value, "value");

			return "\"" + value + "\"";
		}

		private string Escape(string value)
		{
			Check.NotNull(value, "value");

			var result = value
				// literal backslash escaping is broken
				// https://github.com/influxdb/influxdb/issues/3070
				//.Replace(@"\", @"\\")
				.Replace(@"""", @"\""")
				.Replace(@" ", @"\ ")
				.Replace(@"=", @"\=")
				.Replace(@",", @"\,");

			return result;
		}
	}
}
