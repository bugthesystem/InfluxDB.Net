using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxDB.Net.Models
{
    public class Results
    {
        public Results()
        {
            Series = new List<Serie>();
        }

        public List<Serie> Series { get; set; }

    }
}
