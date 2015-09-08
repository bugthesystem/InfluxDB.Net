using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using InfluxDB.Net.Models;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System.Configuration;
using System.Threading.Tasks;
using System.Diagnostics;

namespace InfluxDB.Net.Tests
{
	public class InfluxDbTests : TestBase
	{
		private IInfluxDb _db;
		private string _dbName = string.Empty;

	    protected override void FinalizeTestFixtureSetUp()
		{
		    InfluxVersion influxVersion;
            if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("version"), out influxVersion))
		        influxVersion = InfluxVersion.Auto;

			_db = new InfluxDb(
				ConfigurationManager.AppSettings.Get("url"),
				ConfigurationManager.AppSettings.Get("username"),
				ConfigurationManager.AppSettings.Get("password"),
                influxVersion);

			_db.Should().NotBeNull();

			_dbName = GetRandomName(8);

			var createResponse = _db.CreateDatabaseAsync(_dbName).Result;
			createResponse.Success.Should().BeTrue();
		}

		protected override void FinalizeTestFixtureTearDown()
		{
			var deleteResponse = _db.DropDatabaseAsync(_dbName).Result;

			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async Task Creates_Deletes_DB()
		{
			var dbName = GetRandomName(8);
			var createResponse = await _db.CreateDatabaseAsync(dbName);
			var deleteResponse = await _db.DropDatabaseAsync(dbName);

			createResponse.Success.Should().BeTrue();
			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async Task Shows_DB()
		{
			var databases = await _db.ShowDatabasesAsync();

			databases
				.Should()
				.NotBeNullOrEmpty();

			databases
				.Where(db => db.Name.Equals(_dbName))
				.Single()
				.Should()
				.NotBeNull();
		}

		[Test]
		public async Task Pings_Server()
		{
			var pong = await _db.PingAsync();

			pong.Should().NotBeNull();
			pong.Version.Should().NotBeEmpty();
			pong.Success.Should().BeTrue();
		}

		[Test]
		[TestCase(100)]
		public async Task Writes_Multiple_Series_With_Tags_Fields(int count)
		{
			var points = NewPoints(count);

            var req = new WriteRequest(_db.GetFormatter()) { Points = points };
			Debug.WriteLine(req.GetLines());

			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();
		}

		[Test]
		public void Throws_Exception_When_Writing_Series_Without_Fields()
		{
			var points = NewPoints(1);
			points.Single().Timestamp = null;
			points.Single().Fields.Clear();

			Assert.That(async () => await _db.WriteAsync(_dbName, points), Throws.TypeOf<InfluxDbApiException>());
		}

		[Test]
		public void Throws_Exception_On_Malformed_Query()
		{
			Assert.That(async () => await _db.QueryAsync(_dbName, "blah"), Throws.TypeOf<InfluxDbApiException>());
		}

		[Test]
		public async Task Query_Nonexistent_Series()
		{
			var actual = await _db.QueryAsync(_dbName, "select * from nonexistentseries");

			actual.Count.Should().Be(0);
		}

		[Test]
		public async Task Write_Query_With_Nonexistant_Field()
		{
            if (_db.GetClientVersion() == InfluxVersion.v092)
            {
                Assert.Inconclusive("This scenario has not been implemented for InfluxDB version 0.9.2.");
            }

			var points = NewPoints(1);
			var response = await _db.WriteAsync(_dbName, points);

			response.Success.Should().BeTrue();

			var actual = await _db.QueryAsync(_dbName, string.Format("select nonexistentfield from \"{0}\"", points.Single().Name));

			actual.Count.Should().Be(0);
		}

		[Test]
		public async Task Write_Query_Drop_Series_With_Tags_Fields()
		{
            if (_db.GetClientVersion() == InfluxVersion.v092)
            {
                Assert.Inconclusive("This scenario has not been implemented for InfluxDB version 0.9.2.");
            }

			var points = NewPoints(1);

			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

            var expected = _db.GetFormatter().PointToSerie(points.First());

			// query
			var actual = await Query(expected);

			var deleteSerieResponse = await _db.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		private async Task<List<Serie>> Query(Serie expected)
		{
			// 0.9.3 need 'group by' to retrieve tags as tags when using select *
			var result = await _db.QueryAsync(_dbName, string.Format("select * from \"{0}\" group by *", expected.Name));

			result.Should().NotBeNull();
			result.Count().Should().Be(1);

			var actual = result.Single();

			actual.Name.Should().Be(expected.Name);
			actual.Tags.Count.Should().Be(expected.Tags.Count);
			actual.Tags.ShouldAllBeEquivalentTo(expected.Tags);
			actual.Columns.ShouldAllBeEquivalentTo(expected.Columns);
			actual.Columns.Count().Should().Be(expected.Columns.Count());
			actual.Values[0].Count().Should().Be(expected.Values[0].Count());
			((DateTime)actual.Values[0][0]).ToUnixTime().Should().Be(((DateTime)expected.Values[0][0]).ToUnixTime());

			return result;
		}

		[Test]
		public async Task Write_Query_Drop_Series_With_Fields()
		{
            if (_db.GetClientVersion() == InfluxVersion.v092)
            {
                Assert.Inconclusive("This scenario has not been implemented for InfluxDB version 0.9.2.");
            }

			var points = NewPoints(1);
			points.First().Tags.Clear();
			points.First().Tags.Count.Should().Be(0);

			// write
			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

		    var expected = _db.GetFormatter().PointToSerie(points.First());

			// query
			await Query(expected);

			var deleteSerieResponse = await _db.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		[Test]
		public async Task Write_Query_Drop_Simple_Single_Point()
		{
            if (_db.GetClientVersion() == InfluxVersion.v092)
            {
                Assert.Inconclusive("This scenario has not been implemented for InfluxDB version 0.9.2.");
            }

			var points = new Point[]
			{
				new Point
				{
					Name = "foo",
					Fields = new Dictionary<string, object>
					{
						{ "x", 1 }
					},
					Timestamp = DateTime.Now
				}
			};

			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

		    var serie = _db.GetFormatter().PointToSerie(points.First());
            await Query(serie);

			var deleteResponse = await _db.DropSeriesAsync(_dbName, points.First().Name);
			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async Task Write_Query_Returns_Empty_Result()
		{
			var points = NewPoints(1);

			var response = await _db.WriteAsync(_dbName, points);
			response.Success.Should().BeTrue();

			var actual = await _db.QueryAsync(_dbName, string.Format("select * from \"{0}\" where 0=1", points.Single().Name));
			actual.Count.Should().Be(0);
		}

		[Test]
		public void Randomizes_String()
		{
			var actual = new Random().NextPrintableString(5000);

			actual.Should().NotContain(@"\");
		}

		[Test]
		public void Formats_Point()
		{
			const string value = @"\=&,""*"" -";
			const string escapedFieldValue = @"\\=&\,\""*\""\ -";
			const string escapedTagValue = @"\\=&\,""*""\ -";
			const string seriesName = @"x";
			const string tagName = @"tag_string";
			const string fieldName = @"field_string";
			var dt = DateTime.Now;

			var point = new Point
			{
				Name = seriesName,
				Tags = new Dictionary<string, string>
				{
					{ tagName, value }
				},
				Fields = new Dictionary<string, object>
				{
					{ fieldName, value }
				},
				Timestamp = dt
			};

		    var formatter = _db.GetFormatter();
		    var expected = string.Format(formatter.GetLineTemplate(),
				/* key */ seriesName + "," + tagName + "=" + escapedTagValue,
				/* fields */ fieldName + "=" + "\"" + escapedFieldValue + "\"",
				/* timestamp */ dt.ToUnixTime());

            var actual = formatter.PointToString(point);

			actual.Should().Be(expected);
		}

		[Test]
		public void Gets_Lines()
		{
			var points = NewPoints(2);
		    var formatter = _db.GetFormatter();
		    var request = new WriteRequest(formatter)
			{
				Points = points
			};

			var actual = request.GetLines();
			var expected = string.Join("\n", points.Select(p => formatter.PointToString(p)));

			actual.Should().Be(expected);
		}

		private static string GetRandomName(int length)
		{
			return Guid.NewGuid().ToString().Substring(length);
		}

		private Point[] NewPoints(int count)
		{
			var rnd = new Random();
			var fixture = new Fixture();

			fixture.Customize<Point>(c => c
				.With(p => p.Name, GetRandomName(10))
				.Do(p => p.Tags = NewTags(rnd))
				.Do(p => p.Fields = NewFields(rnd))
				.With(p => p.Timestamp, DateTime.Now)
				.OmitAutoProperties());

			return fixture.CreateMany<Point>(count).ToArray();
		}

		private Dictionary<string, string> NewTags(Random rnd)
		{
			// return an alphanumeric sorted dictionary
		    return new Dictionary<string, string>
		    {
                {"tag_bool", (rnd.Next(2) == 0).ToString()},
                {"tag_datetime", DateTime.Now.ToString()},
                {"tag_decimal", ((decimal) rnd.NextDouble()).ToString()},
                {"tag_float", ((float) rnd.NextDouble()).ToString()},
                {"tag_int", rnd.Next().ToString()},
                // quotes in the tag value are creating problems
                // https://github.com/influxdb/influxdb/issues/3928
                {"tag_string", rnd.NextPrintableString(50).Replace("\"", string.Empty)}
		    };
		}

		private Dictionary<string, object> NewFields(Random rnd)
		{
			return new Dictionary<string, object>
			{
                { "field_string", rnd.NextPrintableString(50) },
                { "field_bool", rnd.Next(2) == 0 },
                { "field_int", rnd.Next() },
                { "field_decimal", (decimal)rnd.NextDouble() },
                { "field_float", (float)rnd.NextDouble() },
                { "field_datetime", DateTime.Now }
			};
		}
	}
}
