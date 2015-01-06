using System;
using System.Collections.Generic;
using System.Linq;

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
        }

        public string Name { get; set; }
        public String[] Columns { get; set; }
        public Object[][] Points { get; set; }

        public class Builder
        {
            private readonly List<String> _columns;
            private readonly String _name;
            private readonly List<List<Object>> _valueRows;

            public Builder(String name)
            {
                _name = name;
                _columns = new List<string>();
                _valueRows = new List<List<object>>();
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
                return this;
            }

            public Serie Build()
            {
                Check.NotNullOrEmpty(_name, "Serie name must not be null or empty.");
                var serie = new Serie(_name)
                {
                    Columns = _columns.ToArray()
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
                serie.Points = points;
                return serie;
            }
        }
    }
}