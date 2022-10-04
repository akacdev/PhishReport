using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhishReport
{
    /// <summary>
    /// The main class for interacting with the Phish.Report APIs.
    /// </summary>
    public class PhishReportClient
    {
        /// <summary>
        /// The current version of the API.
        /// </summary>
        public const int Version = 0;
        /// <summary>
        /// The base URL for the API.
        /// </summary>
        public static readonly string BaseUrl = $"https://phish.report/api/v{Version}/";
        /// <summary>
        /// The base URI for the API.
        /// </summary>
        public static readonly Uri BaseUri = new(BaseUrl);

        private readonly HttpClientHandler HttpHandler = new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = false
        };

        private readonly HttpClient Client;

        /// <summary>
        /// Create a new instance of the Phish.Report client.
        /// </summary>
        /// <param name="key">Your API key for Phish.Report. You can find your key at <a href="https://phish.report/user">https://phish.report/user</a>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PhishReportClient(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "API key is null or empty.");

            Client = new(HttpHandler)
            {
                DefaultRequestVersion = new Version(2, 0),
                BaseAddress = BaseUri,
                Timeout = TimeSpan.FromMinutes(1)
            };

            Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Phish.Report C# Client - actually-akac/PhishReport");
            Client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
        }

        /// <summary>
        /// Submit a new phishing report/takedown using the Phish.Report API.
        /// </summary>
        /// <param name="url">The full URL of the concerning website to report. Wherever possible, URLs will be automatically re-fanged.</param>
        /// <returns>An instance of <see cref="PhishingTakedown"/> with details about the new takedown.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PhishingTakedown> CreateTakedown(string url)
        {
            if (url is null) throw new ArgumentNullException(nameof(url), "URL to report is null or empty.");

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
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PhishingTakedown> GetTakedown(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id), "Takedown ID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"cases/{id}");

            return await res.Deseralize<PhishingTakedown>();
        }

        /// <summary>
        /// Get the Indicator of Kit matches for a specific page, the latest one is loaded by default.
        /// <para>Learn more about IoK: <a href="https://phish.report/IOK/">https://phish.report/IOK/</a></para>
        /// </summary>
        /// <param name="page">The index of a page that you want to get.</param>
        /// <returns>An array of <see cref="IoKMatch"/> with the requested Indicator of Kit matches, ordered by their submission date.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<IoKMatch[]> GetMatches(int page = 0)
        {
            if (page < 0) throw new ArgumentOutOfRangeException(nameof(page), "IoK page cannot be a negative value.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"iok/matches{(page == 0 ? "" : $"?page={page}")}");

            return (await res.Deseralize<IoKMatchContainer>()).Matches;
        }
    }
}