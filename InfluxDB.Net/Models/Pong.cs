using System;

namespace InfluxDB.Net.Models
{
    public class Pong
    {
        public bool Success{ get; set; }
        public string Version { get; set; }
        public TimeSpan ResponseTime { get; set; }
    }
}