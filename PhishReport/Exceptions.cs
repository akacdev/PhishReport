using System;
using System.Net.Http;
using System.Net;

namespace PhishReport
{
    /// <summary>
    /// An exception specific to Phish.Report for advanced catching. When caused by a HTTP request, you can access the exception's properties to get the context.
    /// </summary>
    public class PhishReportException : Exception
    {
        /// <summary>
        /// Milliseconds until you can retry this request again. Provided when there is a ratelimit.
        /// </summary>
        public int? RetryAfter { get; set; }

        /// <summary>
        /// The HTTP request method used that triggered this exception.
        /// </summary>
        public HttpMethod Method { get; set; }
        /// <summary>
        /// The HTTP path used that triggered this exception.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// The HTTP status code used that triggered this exception.
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }

        public PhishReportException(string message) : base(message) { }
        public PhishReportException(string message, HttpResponseMessage res) : base(message)
        {
            Method = res.RequestMessage.Method;
            Path = res.RequestMessage.RequestUri.AbsolutePath;
            StatusCode = res.StatusCode;
        }
        public PhishReportException(string message, HttpMethod method, string path, HttpStatusCode status) : base(message)
        {
            Method = method;
            Path = path;
            StatusCode = status;
        }
    }
}