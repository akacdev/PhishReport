using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhishReport
{
    /// <summary>
    /// The primary class for interacting with Phish.Report APIs.
    /// </summary>
    public class PhishReportClient
    {
        private static readonly HttpClientHandler HttpHandler = new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = false
        };

        private readonly HttpClient Client = new(HttpHandler)
        {
            BaseAddress = Constants.BaseUri,
            DefaultRequestVersion = Constants.HttpVersion
        };

        /// <summary>
        /// Create a new instance of the Phish.Report client.
        /// </summary>
        /// <param name="key">Your API key for Phish.Report. You can find your key at <a href="https://phish.report/user">https://phish.report/user</a>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PhishReportClient(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "API key is null or empty.");

            Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgent);
            Client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            Client.DefaultRequestHeaders.Authorization = new("Bearer", key);
        }

        /// <summary>
        /// Submit a new phishing report/takedown using the Phish.Report API.
        /// </summary>
        /// <param name="url">The full URL of the concerning website to report. Wherever possible, URLs will be automatically re-fanged.</param>
        /// <returns>An instance of <see cref="PhishingTakedown"/> with details about the new takedown.</returns>
        /// <exception cref="PhishReportException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PhishingTakedown> CreateTakedown(string url)
        {
            if (url is null) throw new ArgumentNullException(nameof(url), "URL is null or empty.");

            CreateCaseParameters parameters = new()
            {
                Url = url
            };

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "cases", parameters);

            return await res.Deseralize<PhishingTakedown>();
        }

        /// <summary>
        /// Get an existing phishing report/takedown using the Phish.Report API.
        /// </summary>
        /// <param name="id">The ID of the takedown, for example <c>case_4kr52xX9zZA</c>.</param>
        /// <returns>An instance of <see cref="PhishingTakedown"/> with details about the existing takedown.</returns>
        /// <exception cref="PhishReportException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PhishingTakedown> GetTakedown(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id), "Takedown ID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"cases/{id}");

            return await res.Deseralize<PhishingTakedown>();
        }

        /// <summary>
        /// Get IOK matches of an existing Urlscan result.
        /// <para>
        ///     This currently isn't implemented in the main API, and instead makes use of the web API routes, that communicate with AJAX.<br/>
        ///     In the event that this breaks, please capture the context and open an issue at the Github repository.
        /// </para>
        /// </summary>
        /// <param name="uuid">The UUID of the Urlscan result to analyse.</param>
        /// <returns>An array of <see cref="string"/> with the names of IOK rules that match this Urlscan result.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PhishReportException"></exception>
        public async Task<string[]> GetIokMatches(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "UUID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "IOK/analyse-urlscan", new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "url", $"https://urlscan.io/result/{uuid}/" }
            }), absolutePath: true);

            string html = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(html)) throw new PhishReportException("Returned AJAX HTML is null or empty.");

            bool success = html.Contains("article class=\"message is-link\"");

            string messageBodyFrom = "<div class=\"message-body\">";
            string messageBodyTo = "</div>";

            int messageBodyStart = html.IndexOf(messageBodyFrom);
            if (messageBodyStart == -1) throw new PhishReportException("Missing 'message-body' start.");

            int messageBodyEnd = html.IndexOf(messageBodyTo, messageBodyStart);
            if (messageBodyEnd == -1) throw new PhishReportException("Missing 'message-body' end.");

            string messageBody = html[(messageBodyStart + messageBodyFrom.Length)..messageBodyEnd].Trim();
            if (!success) throw new PhishReportException($"Failed to analyse result for IOKs: {messageBody.HtmlDecode()}");

            return Constants.IokIndicatorRegex.Matches(messageBody).Select(match => match.Groups.Values.Last().Value).ToArray();
        }
    }
}