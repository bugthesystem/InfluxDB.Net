using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace InfluxDB.Net
{
    internal class JsonRequestContent : IRequestContent
    {
        private const string JSON_MIME_TYPE = "application/json";

        public JsonRequestContent(object val, JsonSerializer serializer)
        {
            if (EqualityComparer<object>.Default.Equals(val))
            {
                throw new ArgumentNullException("val");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            Value = val;
            Serializer = serializer;
        }

        private object Value { get; set; }

        private JsonSerializer Serializer { get; set; }

        public HttpContent GetContent()
        {
            string serializedObject = Serializer.SerializeObject(Value);
            return new StringContent(serializedObject, Encoding.UTF8, JSON_MIME_TYPE);
        }
    }
}