using System.Net;
using System.Net.Http;

namespace InfluxDB.Net
{
    public class InfluxDbResponse
    {
        public HttpResponseMessage Raw { get; private set; }

        public virtual bool Success
        {
            get { return Raw.StatusCode == HttpStatusCode.OK; }
        }

        public InfluxDbResponse(HttpResponseMessage response)
        {
            Raw = response;
        }
    }

    public class CreateResponse : InfluxDbResponse
    {
        public CreateResponse(HttpResponseMessage response)
            : base(response)
        {
        }

        public override bool Success
        {
            get { return Raw.StatusCode == HttpStatusCode.Created; }
        }
    }
    public class DeleteResponse : InfluxDbResponse
    {
        public DeleteResponse(HttpResponseMessage response)
            : base(response)
        {
        }

        public override bool Success
        {
            //TODO: Ask to influx db creators
            get { return Raw.StatusCode == HttpStatusCode.NoContent; }
        }
    }
}