using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace InfluxDB.Net.Models
{
    public class Serie
    {
        public Serie()
        {
        }

        private Serie(string name)
        {
            Name = name;
            //TODO
            //Epoch = (long) ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
            
        }

        public string Name { get; set; }
        public String[] Columns { get; set; }
        public Object[][] Values { get; set; }
        public long[] Epoch { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            //we do this because influx now doesn't understand \r\n as NewLine. so we need to build new lines manually and don't lose performance.
            builder.Append(string.Format("{0} {1} {2}", Name, BuildValuesString(0), Epoch[0]));
            for (int i = 1; i < Values.Length; i++)
            {
                builder.AppendFormat("\n{0} {1} {2}", Name, BuildValuesString(i), Epoch[i]);
            }
            return builder.ToString();
        }

        private string BuildValuesString(int i)
        {
            var builder = new StringBuilder();
            for (int j = 0; j < Values[i].Length; j++)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture,"{0}={1}", Columns[j], Values[i][j]);
                if (j != Values[i].Length-1)
                    builder.Append(",");
            }
            return builder.ToString();
        }

        public class Builder
        {
            private readonly List<String> _columns;
            private readonly String _name;
            private readonly List<List<Object>> _valueRows;
            private readonly List<long> _epochRows;

            public Builder(String name)
            {
                _name = name;
                _columns = new List<string>();
                _valueRows = new List<List<object>>();
                _epochRows = new List<long>();
            }

            public Builder Columns(params String[] columnNames)
            {
                Check.IfTrue(_columns.Count > 0, "You can only call columns() once.");
                _columns.AddRange(columnNames);
                return this;
            }

            public Builder Values(params object[] values)
            {
                Check.IfTrue(values.Length != _columns.Count, "Value count differs from column count.");
                _valueRows.Add(values.ToList());
                _epochRows.Add((long) ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds));
                return this;
            }

            public Builder Values(long epoch, params object[] values)
            {
                Check.IfTrue(values.Length != _columns.Count, "Value count differs from column count.");
                _valueRows.Add(values.ToList());
                _epochRows.Add(epoch);
                return this;
            }

            public Serie Build()
            {
                Check.NotNullOrEmpty(_name, "Serie name must not be null or empty.");
                var serie = new Serie(_name)
                {
                    Columns = _columns.ToArray(),
                    Epoch = _epochRows.ToArray()
                };
                var points = new object[_valueRows.Count][];
                for (int i = 0; i < points.GetLength(0); i++)
                {
                    points[i] = new object[_columns.Count];
                }

                int row = 0;
                foreach (var values in _valueRows)
                {
                    points[row] = values.ToArray();
                    row++;
                }
                serie.Values = points;
                return serie;
            }
        }
    }
}