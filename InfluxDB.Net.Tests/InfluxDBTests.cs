using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using InfluxDB.Net.Models;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace InfluxDB.Net.Tests
{
	public class InfluxDbTests : TestBase
	{
		private IInfluxDb _db;
		private string _dbName = string.Empty;

		protected override void FinalizeSetUp()
		{
			_db = new InfluxDb("http://suave.local:8086", "root", "root");

			_db.Should().NotBeNull();

			_dbName = GetNewDbName();

			var createResponse = _db.CreateDatabaseAsync(_dbName).Result;
			createResponse.Success.Should().BeTrue();
		}

		protected override void FinalizeTearDown()
		{
			var deleteResponse = _db.DropDatabaseAsync(_dbName).Result;

			deleteResponse.Success.Should().BeTrue();
		}

		private static string GetNewDbName()
		{
			return "T" + Guid.NewGuid().ToString("N").Substring(9);
		}

		[Test]
		public async void Create_Delete_DB_Test()
		{
			var dbName = GetNewDbName();
			var createResponse = await _db.CreateDatabaseAsync(dbName);
			var deleteResponse = await _db.DropDatabaseAsync(dbName);

			createResponse.Success.Should().BeTrue();
			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async void Show_DB_Test()
		{
			List<Database> databases = await _db.ShowDatabasesAsync();

			databases.Should().NotBeNullOrEmpty();
			databases.Where(database => database.Name.Equals(_dbName)).Should().NotBeNull();
		}

		[Test]
		public async void Ping_Test()
		{
			var pong = await _db.PingAsync();

			pong.Should().NotBeNull();
			pong.Version.Should().NotBeEmpty();
			pong.Success.Should().BeTrue();
		}

		[Test]
		public async void Write_Query_Drop_Series_With_Tags_Fields_Test()
		{
			var points = NewPoints(1);

			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

			var expected = points.First();

			// query
			var actual = await _db.QueryAsync(_dbName, string.Format("select * from \"{0}\"", expected.Name), TimeUnit.Milliseconds);

			actual.Should().NotBeNull();
			actual.Count.Should().Be(1);
			actual.Single().Tags.Count().Should().Be(expected.Tags.Count);
			actual.Single().Name.Should().Be(expected.Name);

			var deleteSerieResponse = await _db.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		[Test]
		public async void Write_Query_Drop_Series_With_Fields_Test()
		{
			var points = NewPoints(1);
			points.First().Tags.Clear();
			points.First().Tags.Count.Should().Be(0);

			// write
			var writeResponse = await _db.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

			var expected = points.First();

			// query
			var actual = await _db.QueryAsync(_dbName, string.Format("select * from \"{0}\"", expected.Name), TimeUnit.Milliseconds);

			actual.Should().NotBeNull();
			actual.Count.Should().Be(1);
			actual.Single().Name.Should().Be(expected.Name);
			actual.Single().Tags.Count.Should().Be(0);

			var deleteSerieResponse = await _db.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		[Test]
		public void Randomizes_String()
		{
			var actual = new Random().NextString(5000);

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

		private Point[] NewPoints(int count)
		{
			var rnd = new Random();
			var fixture = new Fixture();

			var points = fixture
				.Build<Point>()
				.With(p => p.Name, rnd.NextAlphanumericString(10))
				.With(p => p.Tags,
					new Dictionary<string, object>
					{
						{ "tag-string", rnd.NextString(50) },
						{ "tag-bool", rnd.Next(2) == 0 },
						{ "tag-int", fixture.Create<int>() },
						{ "tag-decimal", (decimal)rnd.NextDouble() },
						{ "tag-float", (float)rnd.NextDouble() },
						{ "tag-datetime", fixture.Create<DateTime>() }
					})
				.With(p => p.Fields,
					new Dictionary<string, object>
					{
						{ "field-string", rnd.NextString(50) },
						{ "field-bool", rnd.Next(2) == 0 },
						{ "field-int", fixture.Create<int>() },
						{ "field-decimal", (decimal)rnd.NextDouble() },
						{ "field-float", (float)rnd.NextDouble() },
						{ "field-datetime", fixture.Create<DateTime>() }
					})
				.CreateMany(count);

			return points.ToArray();
		}
	}
}
