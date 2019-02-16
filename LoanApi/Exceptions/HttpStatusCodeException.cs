using System;
using Newtonsoft.Json.Linq;

namespace LoanApi.Exceptions
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCodeException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(int statusCode, Exception inner) : this(statusCode, inner.ToString())
        {
        }

        public HttpStatusCodeException(int statusCode, JObject errorObject) : this(statusCode, errorObject.ToString())
        {
            ContentType = @"application/json";
        }

        public int StatusCode { get; set; }

        public string ContentType { get; set; } = @"text/plain";
    }
}