using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using InfluxDB.Net.Models;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System.Configuration;
using System.Threading.Tasks;

namespace InfluxDB.Net.Tests
{
    [TestFixture(null, InfluxDbVersion.Ver0_9X)]
    [TestFixture("V8", InfluxDbVersion.Ver0_8X)]
    public class InfluxDbTests : TestBase
	{
        private readonly string _dbName;
        private IInfluxDb _db;
        private string _inconclusive;

        public InfluxDbTests(string database, InfluxDbVersion influxDbVersion)
        {
            _dbName = GetRandomName(8);

            var influxDbClientConfiguration = GetInfluxDbClientConfigurationFromConfig(database, influxDbVersion);
            if (influxDbClientConfiguration != null)
            {
                _db = new InfluxDb(influxDbClientConfiguration);
                _db.Should().NotBeNull();
            }
            else
            {
                _inconclusive = "No database for this setup";
            }
        }

        private IInfluxDb Database
        {
            get
            {
                if (!string.IsNullOrEmpty(_inconclusive))
                {
                    Assert.Inconclusive(_inconclusive);
                }

                return _db;
            }
        }

        protected override void FinalizeTestFixtureSetUp()
        {
            CheckIfOnline();

            if (_db == null) return;

			var createResponse = _db.CreateDatabaseAsync(_dbName).Result;
			createResponse.Success.Should().BeTrue();

			// workaround for issue https://github.com/influxdb/influxdb/issues/3363
			// by first creating a single point in the empty db
			var writeResponse = _db.WriteAsync(_dbName, NewPoints(1));

			writeResponse.Result.Success.Should().BeTrue();
		}

        private void CheckIfOnline()
        {
            if (_db == null)
                return;

            try
            {
                if (!_db.PingAsync().Result.Success)
                {
                    _db = null;
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                _inconclusive = "Database is not online.";
                _db = null;
            }
        }

        private InfluxDbClientConfiguration GetInfluxDbClientConfigurationFromConfig(string database, InfluxDbVersion influxDbVersion)
        {
            var uriString = ConfigurationManager.AppSettings.Get(string.Format("url{0}", database));

            if (string.IsNullOrEmpty(uriString))
                return null;

            var response = new InfluxDbClientConfiguration(
                new Uri(uriString),
                ConfigurationManager.AppSettings.Get(string.Format("username{0}", database)),
                ConfigurationManager.AppSettings.Get(string.Format("password{0}", database)),
                influxDbVersion);

            return response;
        }

	    protected override void FinalizeTestFixtureTearDown()
	    {
	        if (_db == null) return;

			var deleteResponse = _db.DropDatabaseAsync(_dbName).Result;

			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async Task Creates_Deletes_DB()
		{
			var dbName = GetRandomName(8);
			var createResponse = await Database.CreateDatabaseAsync(dbName);
            var deleteResponse = await Database.DropDatabaseAsync(dbName);

			createResponse.Success.Should().BeTrue();
			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async Task Shows_DB()
		{
			var databases = await Database.ShowDatabasesAsync();

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
			var pong = await Database.PingAsync();

			pong.Should().NotBeNull();
			pong.Version.Should().NotBeEmpty();
			pong.Success.Should().BeTrue();
		}

		[Test]
		public async Task Writes_Many_Series_With_Tags_Fields()
		{
			var points = NewPoints(5);

            var writeResponse = await Database.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();
		}

		[Test]
		public void Throws_Exception_When_Writing_Series_Without_Fields()
		{
			var points = NewPoints(1);
			points.Single().Timestamp = null;
			points.Single().Fields.Clear();

		    var db = Database;
			Assert.Throws<InfluxDbApiException>(async () => await db.WriteAsync(_dbName, points));
		}

		[Test]
		public void Throws_Exception_On_Malformed_Query()
		{
		    var db = Database;
			Assert.Throws<InfluxDbApiException>(async () => await db.QueryAsync(_dbName, "blah"));
		}

		[Test]
		public void Throws_Exception_When_Querying_Nonexistent_Series()
		{
		    var db = Database;
			Assert.Throws<InfluxDbApiException>(async () => await db.QueryAsync(_dbName, "select * from nonexistentseries"));
		}

		[Test]
		public async Task Throws_Exception_On_Query_With_Nonexistant_Field()
		{
			var points = NewPoints(1);
            var response = await Database.WriteAsync(_dbName, points);

			response.Success.Should().BeTrue();

			Assert.Throws<InfluxDbApiException>(async () =>
                await Database.QueryAsync(_dbName, string.Format("select nonexistentfield from \"{0}\"", points.Single().Name)));
		}

		[Test]
        [Ignore("Fix failing test.")]
		public async Task Write_Query_Drop_Series_With_Tags_Fields()
		{
			var points = NewPoints(1);

            var writeResponse = await Database.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

			var expected = points.First();

			// query
			await Query(expected);

            var deleteSerieResponse = await Database.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		private async Task<List<Serie>> Query(Point expected)
		{
            var result = await Database.QueryAsync(_dbName, string.Format("select * from \"{0}\"", expected.Name));

			result.Should().NotBeNull();
			result.Count().Should().Be(1);

			var actual = result.Single();

			actual.Name.Should().Be(expected.Name);
			actual.Tags.Count.Should().Be(expected.Tags.Count);
			actual.Columns.Count().Should().Be(expected.Fields.Count + 1); // time field is always included
			actual.Values[0].Count().Should().Be(expected.Fields.Count + 1); // time field is always included
			((DateTime)actual.Values[0][0]).ToUnixTime().Should().Be(expected.Timestamp.Value.ToUnixTime());

			return result;
		}

		[Test]
        [Ignore("Fix failing test.")]
		public async Task Write_Query_Drop_Series_With_Fields()
		{
			var points = NewPoints(1);
			points.First().Tags.Clear();
			points.First().Tags.Count.Should().Be(0);

			// write
            var writeResponse = await Database.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

			var expected = points.First();

			// query
			await Query(expected);

            var deleteSerieResponse = await Database.DropSeriesAsync(_dbName, expected.Name);
			deleteSerieResponse.Success.Should().BeTrue();
		}

		[Test]
        [Ignore("Fix failing test.")]
		public async Task Write_Query_Drop_Simple_Single_Point()
		{
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

            var writeResponse = await Database.WriteAsync(_dbName, points);
			writeResponse.Success.Should().BeTrue();

			await Query(points.First());

            var deleteResponse = await Database.DropSeriesAsync(_dbName, points.First().Name);
			deleteResponse.Success.Should().BeTrue();
		}

		[Test]
		public async Task Write_Query_Returns_Empty_Result()
		{
			var points = NewPoints(1);

            var response = await Database.WriteAsync(_dbName, points);
			response.Success.Should().BeTrue();

            var actual = await Database.QueryAsync(_dbName, string.Format("select * from \"{0}\" where 0=1", points.Single().Name));
			actual.Count.Should().Be(0);
		}

		private static string GetRandomName(int length)
		{
			return Guid.NewGuid().ToString().Substring(length);
		}

		public static Point[] NewPoints(int count)
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

		private static Dictionary<string, object> NewTags(Random rnd)
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

		private static Dictionary<string, object> NewFields(Random rnd)
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
