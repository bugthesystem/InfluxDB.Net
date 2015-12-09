namespace InfluxDB.Net.Constants
{
    internal static class QueryStatements
    {
        internal const string AlterRetentionPolicy = "alter retention policy {0} on {1} {2} {3} {4} {5}";
        internal const string CreateContinuousQuery = "create continuous query {0} on {1} begin {2} end;";
        internal const string CreateDatabase = "create database \"{0}\"";
        internal const string CreateRetentionPolicy = "create retention policy \"{0}\" on {1} {2} {3} {4} {5}";
        internal const string CreateUser = "create user {0} with password {1} {2}";
        internal const string DropContinuousQuery = "drop continuous query {0}";
        internal const string DropDatabase = "drop database \"{0}\"";
        internal const string DropMeasurement = "drop measurement \"{0}\"";
        internal const string DropRetentionPolicy = "drop retention policy \"{0}\" on {1}";
        internal const string DropSeries = "drop series from \"{0}\"";
        internal const string DropUser = "drop user {0}";
        internal const string Grant = "grant {0} on {1} to {2}";
        internal const string GrantAll = "grant all to {0}";
        internal const string Revoke = "revoke {0} on {1} from {2}";
        internal const string RevokeAll = "revoke all privleges from {0}";
        internal const string ShowContinuousQueries = "show continuous queries";
        internal const string ShowDatabases = "show databases";
        internal const string ShowFieldKeys = "show field keys {0} {1}";
        internal const string ShowMeasurements = "show measurements";
        internal const string ShowRetentionPolicies = "show retention policies {0}";
        internal const string ShowSeries = "show series";
        internal const string ShowTagKeys = "show tag keys";
        internal const string ShowTagValues = "show tag values";
        internal const string ShowUsers = "show users";
    }
}