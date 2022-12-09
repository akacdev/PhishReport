using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PhishReport
{
    public static class API
    {
        public const int MaxRetries = 3;
        public const int RetryDelay = 3000;

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            object obj,
            HttpStatusCode target = HttpStatusCode.OK,
            bool absoluteUrl = false)
        => await Request(cl, method, url, new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, Constants.JsonContentType), target, absoluteUrl);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            HttpContent content = null,
            HttpStatusCode target = HttpStatusCode.OK,
            bool absoluteUrl = false)
        {
            int retries = 0;

            HttpResponseMessage res = null;

            while (res is null || !target.HasFlag(res.StatusCode))
            {
                HttpRequestMessage req = new(method, absoluteUrl ? url : $"api/v{Constants.Version}/{url}")
                {
                    Content = content
                };

                res = await cl.SendAsync(req);

                if (!target.HasFlag(res.StatusCode))
                {
                    if (res.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        string header = res.Headers.TryGetValues("X-Ratelimit-Reset", out IEnumerable<string> values) ? values.FirstOrDefault() : null;
                        if (header is null) throw new PhishReportException("Server returned no ratelimit header.", method, url, res.StatusCode);

                        long unix = long.Parse(header);

                        DateTime date = unix.ToDate();
                        TimeSpan remaining = date - DateTime.Now;

                        await Task.Delay(((int)remaining.TotalMilliseconds));
                    }
                    else if (
                        res.StatusCode == HttpStatusCode.SeeOther &&
                        res.Headers.Location is not null &&
                        res.Headers.Location.AbsolutePath == "/user/login"
                        )
                    {
                        throw new PhishReportException($"Unauthorized - invalid API key.", method, url, res.StatusCode);
                    }
                    else await Task.Delay(RetryDelay);
                }

                retries++;

                if (retries == MaxRetries)
                {
                    throw new PhishReportException(
                        $"Ran out of retry attempts while requesting {method} {url}, last status code: {res.StatusCode}" +
                        $"\nPreview: {await res.GetPreview()}", method, url, res.StatusCode);
                }
            }

            return res;
        }

        public static async Task<T> Deseralize<T>(this HttpResponseMessage res, JsonSerializerOptions options = null)
        {
            Stream stream = await res.Content.ReadAsStreamAsync();
            if (stream.Length == 0) throw new PhishReportException("Response content is empty, can't parse as JSON.",
                res.RequestMessage.Method, res.RequestMessage.RequestUri.AbsolutePath, res.StatusCode);

            try
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, options);
            }
            catch (Exception ex)
            {
                using StreamReader sr = new(stream);
                string text = await sr.ReadToEndAsync();

                throw new PhishReportException(
                    $"Exception while parsing JSON: {ex.GetType().Name} => {ex.Message}" +
                    $"\nPreview: {await res.GetPreview()}");
            }
        }

        public static async Task<string> GetPreview(this HttpResponseMessage res)
        {
            string text = await res.Content.ReadAsStringAsync();
            return text[..Math.Min(text.Length, Constants.MaxPreviewLength)];
        }
    }
}