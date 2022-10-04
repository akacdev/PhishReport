using System;
using System.Collections.Generic;
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
        public const int RetryDelay = 1000 * 3;
        public const int ExtraDelay = 1000;
        public const int PreviewMaxLength = 500;

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            object obj,
            HttpStatusCode target = HttpStatusCode.OK)
        => await Request(cl, method, url, new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json"), target);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            HttpContent content = null,
            HttpStatusCode target = HttpStatusCode.OK,
            string sid = null)
        {
            int retries = 0;

            HttpResponseMessage res = null;

            while (res is null || !target.HasFlag(res.StatusCode))
            {
                HttpRequestMessage req = new(method, url)
                {
                    Content = content
                };

                res = await cl.SendAsync(req);

                if (!target.HasFlag(res.StatusCode))
                {
                    if (res.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        string header = res.Headers.TryGetValues("X-Ratelimit-Reset", out IEnumerable<string> values) ? values.FirstOrDefault() : null;
                        if (header is null) throw new PhishReportException("Server returned no ratelimit header.");

                        long unix = long.Parse(header);

                        DateTime date = unix.ToDateTime();
                        TimeSpan remaining = date - DateTime.Now;

                        await Task.Delay(((int)remaining.TotalMilliseconds) + ExtraDelay);
                    }
                    else if (
                        res.StatusCode == HttpStatusCode.SeeOther &&
                        res.Headers.Location is not null &&
                        res.Headers.Location.AbsolutePath == "/user/login"
                        )
                    {
                        throw new PhishReportException($"Unauthorized - invalid API key.");
                    }
                    else await Task.Delay(RetryDelay);
                }

                retries++;
                if (retries == MaxRetries)
                {
                    string text = await res.Content.ReadAsStringAsync();
                    throw new PhishReportException(
                        $"Ran out of retry attempts while requesting {method} {url}, last status code: {res.StatusCode}" +
                        $"\nPreview: {text[..Math.Min(text.Length, PreviewMaxLength)]}");
                }
            }

            return res;
        }

        public static async Task<T> Deseralize<T>(this HttpResponseMessage res, JsonSerializerOptions options = null)
        {
            string json = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) throw new PhishReportException("Response content is empty, can't parse as JSON.");

            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (Exception ex)
            {
                throw new PhishReportException($"Exception while parsing JSON: {ex.GetType().Name} => {ex.Message}\nJSON preview: {json[..Math.Min(json.Length, PreviewMaxLength)]}");
            }
        }
    }
}