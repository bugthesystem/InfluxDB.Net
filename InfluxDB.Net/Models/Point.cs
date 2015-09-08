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

		public Point()
		{
			Tags = new Dictionary<string, string>();
			Fields = new Dictionary<string, object>();
			Precision = TimeUnit.Milliseconds;
		}

        ///// <summary>
        ///// Returns a point represented in line protocol format for writing to the InfluxDb API endpoint
        ///// </summary>
        ///// <returns>A string that represents this instance.</returns>
        ///// <remarks>
        ///// Examples:
        ///// cpu,host=serverA,region=us-west value=1.0 10000000000
        ///// cpu,host=serverB,region=us-west value=3.3
        ///// cpu,host=serverB,region=us-east user=123415235,event="overloaded" 20000000000
        ///// mem,host=serverB,region=us-east swapping=true 2000000000
        ///// my\ a\/\/esome\ series,tag=foo\ bar "my-field"="\=*\=" 2000000000
        ///// </remarks>
        public override string ToString()
        {
        //    Check.NotNullOrEmpty(Name, "name");
        //    Check.NotNull(Tags, "tags");
        //    Check.NotNull(Fields, "fields");

        //    var tags = string.Join(",", Tags.Select(t => string.Join("=", t.Key, EscapeTagValue(t.Value))));
        //    var fields = string.Join(",", Fields.Select(t => Format(t.Key, t.Value)));

        //    var key = string.IsNullOrEmpty(tags) ? Escape(Name) : string.Join(",", Escape(Name), tags);
        //    var ts = Timestamp.HasValue ? Timestamp.Value.ToUnixTime().ToString() : string.Empty;

        //    var result = string.Format(LineTemplate, key, fields, ts);

        //    return result;
            throw new NotImplementedException();
        }

		/// <summary>Converts a <see cref="Point"/> to a <see cref="Serie"/>.</summary>
		/// <remarks>
		/// For a Serie select result returned from the server:
		///   The time field and value are implicitly added by server
		/// </remarks>
		/// <returns><see cref="Serie"/></returns>
        //public Serie ToSerie()
        //{
        //    var s = new Serie
        //    {
        //        Name = Name
        //    };

        //    foreach (var key in Tags.Keys.ToList())
        //    {
        //        s.Tags.Add(key, Tags[key]);
        //    }

        //    var sortedFields = Fields.OrderBy(k => k.Key).ToDictionary(x => x.Key, x => x.Value);

        //    s.Columns = new string[] { "time" }.Concat(sortedFields.Keys).ToArray();

        //    s.Values = new object[][]
        //    {
        //        new object[] { Timestamp }.Concat(sortedFields.Values).ToArray()
        //    };

        //    return s;
        //}
	}
}
