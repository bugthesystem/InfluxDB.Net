using System.Net.Http;

namespace InfluxDB.Net.Contracts
{
    internal interface IRequestContent
    {
        HttpContent GetContent();
    }
}