using System;

namespace InfluxDB.Net
{
    public class InfluxDbException : Exception
    {
        public InfluxDbException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}