using System;

namespace InfluxDB.Net.Core
{
    public class InfluxDbException : Exception
    {
        public InfluxDbException(string message, Exception innerException):base(message,innerException)
        {
            
        }
    }
}