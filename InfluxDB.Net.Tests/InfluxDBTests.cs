using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using InfluxDB.Net.Models;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System.Configuration;
using System.Net;

namespace InfluxDB.Net.Tests
{
	public class InfluxDbTests : TestBase
	{
		private IInfluxDb _db;
		private string _dbName = string.Empty;

		protected override void FinalizeSetUp()
		{
			_db = new InfluxDb(
				ConfigurationManager.AppSettings.Get("url"),
				ConfigurationManager.AppSettings.Get("username"),
				ConfigurationManager.AppSettings.Get("password"));

			_db.Should().NotBeNull();

			_dbName = GetRandomName(8);

			var createResponse = _db.CreateDatabaseAsync(_dbName).Result;
			createResponse.Success.Should().BeTrue();
		}

		protected override void FinalizeTearDown()
		{
			var deleteResponse = _db.DropDatabaseAsync(_dbName).Result;

			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async void Creates_Deletes_DB()
		{
			var dbName = GetRandomName(8);
			var createResponse = await _db.CreateDatabaseAsync(dbName);
			var deleteResponse = await _db.DropDatabaseAsync(dbName);

			createResponse.Success.Should().BeTrue();
			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async void Shows_DB()
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
		public async void Pings_Server()
		{
			var pong = await _db.PingAsync();

			pong.Should().NotBeNull();
			pong.Version.Should().NotBeEmpty();
			pong.Success.Should().BeTrue();
		}

		[Test]
		public async void Writes_Many_Series_With_Tags_Fields()
		{
			var points = NewPoints(5);

			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();
		}

		[Test]
		public void Throws_Exception_When_Writing_Series_Without_Fields()
		{
			var points = NewPoints(1);
			points.Single().Timestamp = null;
			points.Single().Fields.Clear();

			Assert.Throws<InfluxDbApiException>(async () =>
				await _db.WriteAsync(_dbName, points));
		}

		[Test]
		public void Throws_Exception_On_Malformed_Query()
		{
			Assert.Throws<InfluxDbApiException>(async () =>
				await _db.QueryAsync(_dbName, "blah"));
		}

		[Test]
		public void Throws_Exception_When_Querying_Nonexistent_Series()
		{
			Assert.Throws<InfluxDbApiException>(async () =>
				await _db.QueryAsync(_dbName, "select * from nonexistentseries"));
		}

		[Test]
		public async void Throws_Exception_On_Query_With_Nonexistant_Field()
		{
			var points = NewPoints(1);
			var response = await _db.WriteAsync(_dbName, points);

			response.Success.Should().BeTrue();

			Assert.Throws<InfluxDbApiException>(async () =>
				await _db.QueryAsync(_dbName, string.Format("select nonexistentfield from \"{0}\"", points.Single().Name)));
		}

		[Test]
		public async void Write_Query_Drop_Series_With_Tags_Fields()
		{
			var points = NewPoints(1);

			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

			var expected = points.First();

			// query
			Query(expected);

			var deleteSerieResponse = await _db.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		private async void Query(Point expected)
		{
			var result = await _db.QueryAsync(_dbName, string.Format("select * from \"{0}\"", expected.Name));

			result.Should().NotBeNull();
			result.Count().Should().Be(1);

			var actual = result.Single();

			actual.Name.Should().Be(expected.Name);
			actual.Tags.Count.Should().Be(expected.Tags.Count);
			actual.Columns.Count().Should().Be(expected.Fields.Count + 1); // time field is always included
			actual.Values[0].Count().Should().Be(expected.Fields.Count + 1); // time field is always included
			((DateTime)actual.Values[0][0]).ToUnixTime().Should().Be(expected.Timestamp.Value.ToUnixTime());
		}

		[Test]
		public async void Write_Query_Drop_Series_With_Fields()
		{
			var points = NewPoints(1);
			points.First().Tags.Clear();
			points.First().Tags.Count.Should().Be(0);

			// write
			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

			var expected = points.First();

			// query
			Query(expected);

			var deleteSerieResponse = await _db.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		[Test]
		public async void Query_Returns_Empty_Result()
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
			const string value = @"\=&,""*"" ";
			const string escapedValue = @"\\=&\,\""*\""\ ";
			const string tagName = @"tag";
			const string fieldName = @"field";
			var dt = DateTime.Now;

			var point = new Point
			{
				Name = value,
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
				string.Join(",", escapedValue, string.Join(@"=""", tagName, escapedValue + @"""")),
				string.Join(@"=""", fieldName, escapedValue + @""""),
				dt.ToUnixTime());

			var actual = point.ToString();

			actual.Should().Be(expected);
		}

		[Test]
		public void Gets_Lines()
		{
			var points = NewPoints(2);
			var request = new WriteRequest
			{
				Points = points
			};

			var actual = request.GetLines();
			var expected = string.Join("\n", points.Select(p => p.ToString()));

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

		private Dictionary<string, object> NewTags(Random rnd)
		{
			return new Dictionary<string, object>
			{
				{ "tag-string", rnd.NextPrintableString(50) },
				{ "tag-bool", rnd.Next(2) == 0 },
				{ "tag-int", rnd.Next() },
				{ "tag-decimal", (decimal)rnd.NextDouble() },
				{ "tag-float", (float)rnd.NextDouble() },
				{ "tag-datetime", DateTime.Now }
			};
		}

		private Dictionary<string, object> NewFields(Random rnd)
		{
			return new Dictionary<string, object>
			{
				{ "field-string", rnd.NextPrintableString(50) },
				{ "field-bool", rnd.Next(2) == 0 },
				{ "field-int", rnd.Next() },
				{ "field-decimal", (decimal)rnd.NextDouble() },
				{ "field-float", (float)rnd.NextDouble() },
				{ "field-datetime", DateTime.Now }
			};
		}
	}
}
