using System;

namespace InfluxDB.Net.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string ReadFrom { get; set; }
        public string WriteTo { get; set; }

        public void SetPermissions(params string[] permissions)
        {
            if (null != permissions)
            {
                switch (permissions.Length)
                {
                    case 0:
                        break;
                    case 2:
                        ReadFrom = permissions[0];
                        WriteTo = permissions[1];
                        break;
                    default:
                        throw new ArgumentException("You have to specify readFrom and writeTo permissions.");
                }
            }
        }
    }
}