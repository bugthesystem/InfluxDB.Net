using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfluxDB.Net.Tests
{
    /// <summary>
    /// InfluxDB connectivity settings, as defined in appsettings.json
    /// </summary>
    public class InfluxSettings
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientSettingsProviderServiceUri { get; set; }
    }
}
