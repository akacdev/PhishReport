using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhishReport
{
    internal static class API
    {
        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            object obj,
            HttpStatusCode target = HttpStatusCode.OK,
            bool absolutePath = false)
        => await Request(cl, method, path, await obj.Serialize(), target, absolutePath);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            HttpContent content = null,
            HttpStatusCode target = HttpStatusCode.OK,
            bool absolutePath = false)
        {
            using HttpRequestMessage req = new(method, absolutePath ? path : $"api/v{Constants.Version}/{path}")
            {
                Content = content
            };

            HttpResponseMessage res = await cl.SendAsync(req);
            content?.Dispose();

            if (target.HasFlag(res.StatusCode)) return res;

            if (res.StatusCode == HttpStatusCode.SeeOther && res.Headers.Location?.AbsolutePath == "/user/login")
            {
                throw new PhishReportException($"Unauthorized - invalid API key.", res);
            }

            PhishReportException ex = new($"Failed to request {method} {path}, received status code {res.StatusCode}\nPreview: {await res.GetPreview()}", res);

            if (res.StatusCode == HttpStatusCode.TooManyRequests)
            {
                string header =
                    (res.Headers.TryGetValues("X-Ratelimit-Reset", out IEnumerable<string> values) ? values.FirstOrDefault() : null)
                    ?? throw new PhishReportException("Server returned no ratelimit header.", method, path, res.StatusCode);

                long unix = long.Parse(header);

                DateTime date = unix.ToDate();
                TimeSpan remaining = date - DateTime.Now;

                ex.RetryAfter = (int)remaining.TotalMilliseconds;
            }

            throw ex;
        }
    }
}