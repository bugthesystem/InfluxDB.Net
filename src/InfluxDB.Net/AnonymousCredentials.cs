using System;
using System.Net.Http;

namespace InfluxDB.Net
{
    public class AnonymousCredentials : Credentials
    {
        public AnonymousCredentials()
        {
        }

        public override HttpClient BuildHttpClient()
        {
            return new HttpClient();
        }

        public override bool IsTlsCredentials()
        {
            return false;
        }
    }

    public class BasicAuthCredentials : Credentials
    {
        public string Username { get; protected set; }
        public string Password { get; protected set; }

        private readonly bool _isTls;

        public BasicAuthCredentials(string username, string password, bool isTls = false)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password");
            }

            Username = username;
            Password = password;
            _isTls = isTls;
        }

        public override HttpClient BuildHttpClient()
        {
            return new HttpClient();
        }

        public override bool IsTlsCredentials()
        {
            return _isTls;
        }
    }
}