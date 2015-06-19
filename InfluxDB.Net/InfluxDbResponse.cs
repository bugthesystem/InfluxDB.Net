﻿using System.Net;

namespace InfluxDB.Net
{
    public class InfluxDbApiResponse
    {
        public InfluxDbApiResponse(HttpStatusCode statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string Body { get; private set; }

        public virtual bool Success
        {
            get { return StatusCode == HttpStatusCode.OK 
                || StatusCode == HttpStatusCode.NoContent; }
        }
    }

    public class InfluxDbApiCreateResponse : InfluxDbApiResponse
    {
        public InfluxDbApiCreateResponse(HttpStatusCode statusCode, string body)
            : base(statusCode, body)
        {
        }

        public override bool Success
        {
            get { return StatusCode == HttpStatusCode.Created; }
        }
    }

    public class InfluxDbApiDeleteResponse : InfluxDbApiResponse
    {
        public InfluxDbApiDeleteResponse(HttpStatusCode statusCode, string body)
            : base(statusCode, body)
        {
        }

        public override bool Success
        {
            //TODO: Ask to influx db creators
            get { return StatusCode == HttpStatusCode.NoContent; }
        }
    }
}