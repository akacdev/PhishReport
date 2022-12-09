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
        public HttpMethod Method { get; set; }
        public string Path { get; set; }
        public HttpStatusCode? StatusCode { get; set; }

        public PhishReportException(string message) : base(message) { }
        public PhishReportException(string message, HttpMethod method, string path, HttpStatusCode status) : base(message)
        {
            Method = method;
            Path = path;
            StatusCode = status;
        }
    }
}