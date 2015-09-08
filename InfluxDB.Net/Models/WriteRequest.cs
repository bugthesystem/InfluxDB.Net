using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InfluxDB.Net.Models
{
	/// <summary>
	/// Represents an API write request
	/// </summary>
	public class WriteRequest
	{
	    private readonly IFormatter _formatter;

	    public WriteRequest(IFormatter formatter)
	    {
	        _formatter = formatter;
	    }

	    public string Database { get; set; }
		public string RetentionPolicy { get; set; }
		public Point[] Points { get; set; }

		/// <summary>Gets the set of points in line protocol format.</summary>
		/// <returns></returns>
		public string GetLines()
		{
			return string.Join("\n", Points.Select(p => _formatter.PointToString(p)));
		}
	}

}
