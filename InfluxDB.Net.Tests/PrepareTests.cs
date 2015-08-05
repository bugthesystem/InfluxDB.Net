using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using InfluxDB.Net.Models;
using NUnit.Framework;

namespace InfluxDB.Net.Tests
{
    public class PrepareTests
    {
        [Test]
        public void Formats_Point()
        {
            const string value = @"\=&,""*"" ";
            const string escapedValue = @"\\=&\,\""*\""\ ";
            const string seriesName = @"x";
            const string tagName = @"tag-string";
            const string fieldName = @"field-string";
            var dt = DateTime.Now;

            var point = new Point
            {
                Name = seriesName,
                Tags = new Dictionary<string, object>
                {
                    { tagName, value }
                },
                Fields = new Dictionary<string, object>
                {
                    { fieldName, value }
                },
                Timestamp = dt
            };

            var expected = string.Format(Point.LineTemplate,
                /* key */ seriesName + "," + "\"" + tagName + "\"" + "=" + "\"" + escapedValue + "\"",
                /* fields */ "\"" + fieldName + "\"" + "=" + "\"" + escapedValue + "\"",
                /* timestamp */ dt.ToUnixTime());

            var actual = point.ToString();

            actual.Should().Be(expected);
        }

        [Test]
        public void Gets_Lines()
        {
            var points = InfluxDbTests.NewPoints(2);
            var request = new WriteRequest
            {
                Points = points
            };

            var actual = request.GetLines();
            var expected = string.Join("\n", points.Select(p => p.ToString()));

            actual.Should().Be(expected);
        }

        [Test]
        public void Randomizes_String()
        {
            var actual = new Random().NextPrintableString(5000);

            actual.Should().NotContain(@"\");
        }
    }
}