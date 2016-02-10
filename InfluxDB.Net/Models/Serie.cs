using System;
using System.Collections.Generic;
using System.Linq;

namespace InfluxDB.Net.Models
{
    public class Serie
    {
        public Serie()
        {
            Tags = new Dictionary<string, string>();
        }

        private Serie(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public string[] Columns { get; set; }
        public object[][] Values { get; set; }
    }
}