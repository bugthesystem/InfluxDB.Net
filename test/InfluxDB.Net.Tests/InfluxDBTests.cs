using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using InfluxDB.Net.Models;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System.Configuration;
using System.Threading.Tasks;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Helpers;
using InfluxDB.Net.Infrastructure.Influx;
using InfluxDB.Net.Enums;

namespace InfluxDB.Net.Tests
{
    public class InfluxDbTests : TestBase
    {
        private IInfluxDb _influx;
        private string _dbName = String.Empty;
        private static readonly string _fakeDbPrefix = "FakeDb";
        private static readonly string _fakeMeasurementPrefix = "FakeMeasurement";

        protected override async Task FinalizeSetUp()
        {
            //TODO: Have this injectable so it can be executed from the test server with different data
            InfluxVersion influxVersion;
            if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("version"), out influxVersion))
                influxVersion = InfluxVersion.Auto;

            _influx = new InfluxDb(
                ConfigurationManager.AppSettings.Get("url"),
                ConfigurationManager.AppSettings.Get("username"),
                ConfigurationManager.AppSettings.Get("password"),
                influxVersion);

            _influx.Should().NotBeNull();

            _dbName = CreateRandomDbName();

            //PurgeFakeDatabases();

            var createResponse = _influx.CreateDatabaseAsync(_dbName).Result;
            createResponse.Success.Should().BeTrue();

            // workaround for issue https://github.com/influxdb/influxdb/issues/3363
            // by first creating a single point in the empty db
            var writeResponse = _influx.WriteAsync(_dbName, CreateMockPoints(1));
            writeResponse.Result.Success.Should().BeTrue();
        }

        private async void PurgeFakeDatabases()
        {
            var databasesResponse = _influx.ShowDatabasesAsync();
            var dbs = databasesResponse.Result;

            foreach (var db in dbs)
            {
                if (db.Name.StartsWith(_fakeDbPrefix))
                    await _influx.DropDatabaseAsync(db.Name);
            }
        }

        protected override async Task FinalizeTearDown()
        {
            var deleteResponse = _influx.DropDatabaseAsync(_dbName).Result;

            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async Task Influx_OnPing_ShouldReturnVersion()
        {
            var pong = await _influx.PingAsync();

            pong.Should().NotBeNull();
            pong.Success.Should().BeTrue();
            pong.Version.Should().NotBeEmpty();
        }

        [Test]
        public async Task Influx_OnFakeDbName_ShouldCreateAndDropDb()
        {
            // Arrange
            var dbName = CreateRandomDbName();

            // Act
            var createResponse = await _influx.CreateDatabaseAsync(dbName);
            var deleteResponse = await _influx.DropDatabaseAsync(dbName);

            // Assert
            createResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async Task DbShowDatabases_OnDatabaseExists_ShouldReturnDatabaseList()
        {
            // Arrange
            var dbName = CreateRandomDbName();
            var createResponse = await _influx.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            // Act
            var databases = await _influx.ShowDatabasesAsync();

            // Assert
            databases
                .Should()
                .NotBeNullOrEmpty();

            databases
                .Where(db => db.Name.Equals(dbName))
                .Single()
                .Should()
                .NotBeNull();
        }

        [Test]
        public async Task DbWrite_OnMultiplePoints_ShouldWritePoints()
        {
            var points = CreateMockPoints(5);

            var writeResponse = await _influx.WriteAsync(_dbName, points);
            writeResponse.Success.Should().BeTrue();
        }

        [Test]
        public void DbWrite_OnPointsWithoutFields_ShouldThrowException()
        {
            var points = CreateMockPoints(1);
            points.Single().Timestamp = null;
            points.Single().Fields.Clear();

            Func<Task> act = async () => { await _influx.WriteAsync(_dbName, points); };
            act.ShouldThrow<InfluxDbApiException>();
        }

        [Test]
        public void DbQuery_OnInvalidQuery_ShouldThrowException()
        {
            Func<Task> act = async () => { await _influx.QueryAsync(_dbName, "blah"); };
            act.ShouldThrow<InfluxDbApiException>();
        }

        [Test]
        public async Task DbQuery_OnNonExistantSeries_ShouldReturnEmptyList()
        {
            var result = await _influx.QueryAsync(_dbName, "select * from nonexistentseries");
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task DbQuery_OnNonExistantFields_ShouldReturnEmptyList()
        {
            var points = CreateMockPoints(1);
            var response = await _influx.WriteAsync(_dbName, points);

            response.Success.Should().BeTrue();

            var result = await _influx.QueryAsync(_dbName, String.Format("select nonexistentfield from \"{0}\"", points.Single().Measurement));
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task DbDropSeries_OnExistingSeries_ShouldDropSeries()
        {
            var points = CreateMockPoints(1);
            var writeResponse = await _influx.WriteAsync(_dbName, points);
            writeResponse.Success.Should().BeTrue();

            var expected = _influx.GetFormatter().PointToSerie(points.First());
            // query
            await Query(expected);

            var deleteSerieResponse = await _influx.DropSeriesAsync(_dbName, points.First().Measurement);
            deleteSerieResponse.Success.Should().BeTrue();
        }

        [Test]
        public async Task DbQuery_OnWhereClauseNotMet_ShouldReturnNoSeries()
        {
            // Arrange
            var points = CreateMockPoints(1);
            var writeResponse = await _influx.WriteAsync(_dbName, points);
            writeResponse.Success.Should().BeTrue();

            // Act
            var queryResponse = await _influx.QueryAsync(_dbName, String.Format("select * from \"{0}\" where 0=1", points.Single().Measurement));

            // Assert
            queryResponse.Count.Should().Be(0);
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
                Measurement = seriesName,
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

            var formatter = _influx.GetFormatter();
            var expected = String.Format(formatter.GetLineTemplate(),
                /* key */ seriesName + "," + tagName + "=" + escapedTagValue,
                /* fields */ fieldName + "=" + "\"" + escapedFieldValue + "\"",
                /* timestamp */ dt.ToUnixTime());

            var actual = formatter.PointToString(point);

            actual.Should().Be(expected);
        }

        [Test]
        public void WriteRequestGetLines_OnCall_ShouldReturnNewLineSeparatedPoints()
        {
            var points = CreateMockPoints(2);
            var formatter = _influx.GetFormatter();
            var request = new WriteRequest(formatter)
            {
                Points = points
            };

            var actual = request.GetLines();
            var expected = String.Join("\n", points.Select(p => formatter.PointToString(p)));

            actual.Should().Be(expected);
        }

        private async Task<List<Serie>> Query(Serie expected)
        {
            // 0.9.3 need 'group by' to retrieve tags as tags when using select *
            var result = await _influx.QueryAsync(_dbName, String.Format("select * from \"{0}\" group by *", expected.Name));

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

        private static string CreateRandomDbName()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            return String.Format("{0}{1}", _fakeDbPrefix, timestamp);
        }

        private static string CreateRandomMeasurementName()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            return String.Format("{0}{1}", _fakeMeasurementPrefix, timestamp);
        }

        private Point[] CreateMockPoints(int amount)
        {
            var rnd = new Random();
            var fixture = new Fixture();

            fixture.Customize<Point>(c => c
                .With(p => p.Measurement, CreateRandomMeasurementName())
                .Do(p => p.Tags = NewTags(rnd))
                .Do(p => p.Fields = NewFields(rnd))
                .OmitAutoProperties());

            var points = fixture.CreateMany<Point>(amount).ToArray();
            var timestamp = DateTime.UtcNow.AddDays(-5);
            foreach (var point in points)
            {
                timestamp = timestamp.AddMinutes(1);
                point.Timestamp = timestamp;
            }

            return points;
        }

        private Dictionary<string, object> NewTags(Random rnd)
        {
            return new Dictionary<string, object>
            {
                // quotes in the tag value are creating problems
                // https://github.com/influxdb/influxdb/issues/3928
                //{"tag_string", rnd.NextPrintableString(50).Replace("\"", string.Empty)},
                {"tag_bool", (rnd.Next(2) == 0).ToString()},
                {"tag_datetime", DateTime.Now.ToString()},
                {"tag_decimal", ((decimal) rnd.NextDouble()).ToString()},
                {"tag_float", ((float) rnd.NextDouble()).ToString()},
                {"tag_int", rnd.Next().ToString()}
            };
        }

        private Dictionary<string, object> NewFields(Random rnd)
        {
            return new Dictionary<string, object>
            {
                //{ "field_string", rnd.NextPrintableString(50) },
                { "field_bool", rnd.Next(2) == 0 },
                { "field_int", rnd.Next() },
                { "field_decimal", (decimal)rnd.NextDouble() },
                { "field_float", (float)rnd.NextDouble() },
                { "field_datetime", DateTime.Now }
            };
        }
    }
}
