using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace PhishReport
{
    /// <summary>
    /// The primary class for interacting with Phish.Report APIs.
    /// </summary>
    public class PhishReportClient
    {
        /// <summary>
        /// The base URI for the API.
        /// </summary>
        public static readonly Uri BaseUri = new($"https://phish.report/api/v{Constants.Version}/");

        private static readonly HttpClientHandler HttpHandler = new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = false
        };

        private readonly HttpClient Client = new(HttpHandler)
        {
            DefaultRequestVersion = new Version(2, 0),
            BaseAddress = BaseUri,
            Timeout = TimeSpan.FromMinutes(1)
        };

        private EventHandler<IoKMatch> IoKHandler;
        /// <summary>
        /// Triggers whenever a new scan on Urlscan matches one of the kit indicators.
        /// </summary>
        public event EventHandler<IoKMatch> IoKMatched
        {
            add
            {
                IoKHandler += value;
                if (IoKHandler.GetInvocationList().Length == 1) StartPolling();
            }
            remove
            {
                IoKHandler -= value;
                if (IoKHandler is null || IoKHandler.GetInvocationList().Length == 0) StopPolling();
            }
        }

        private IoKMatch[] LastIoKMatches;
        private Timer IokTimer;

        /// <summary>
        /// Create a new instance of the Phish.Report client.
        /// </summary>
        /// <param name="key">Your API key for Phish.Report. You can find your key at <a href="https://phish.report/user">https://phish.report/user</a>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PhishReportClient(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "API key is null or empty.");

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
        public async Task<IoKMatch[]> GetIoKMatches(int page = 0)
        {
            if (page < 0) throw new ArgumentOutOfRangeException(nameof(page), "IoK page cannot be a negative value.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"iok/matches{(page == 0 ? "" : $"?page={page}")}");

            return (await res.Deseralize<IoKMatchContainer>()).Matches;
        }

        /// <summary>
        /// Start polling Indicator of Kit for new matches.
        /// </summary>
        private async void StartPolling()
        {
            if (IokTimer is null)
            {
                IokTimer = new()
                {
                    Interval = Constants.IoKPollInterval
                };

                IokTimer.Elapsed += async (o, e) => await PollIoK();
            }

            IokTimer.Start();

            LastIoKMatches = await GetIoKMatches();
        }

        /// <summary>
        /// Stop polling Indicator of Kit for new matches.
        /// </summary>
        private void StopPolling()
        {
            if (IokTimer is null) return;

            IokTimer.Stop();
            LastIoKMatches = null;
        }

        /// <summary>
        /// Poll Indicator of Kit to find new Indicator of Kit matches and trigger event handlers.
        /// </summary>
        private async Task PollIoK()
        {
            IoKMatch[] found;
            IoKMatch[] matches = await GetIoKMatches();

            if (LastIoKMatches is null) found = matches;
            else found = matches.Where(match => LastIoKMatches.All(last => match.UrlscanUUID != last.UrlscanUUID)).ToArray();

            LastIoKMatches = matches;

            found = found.OrderBy(match => Constants.LowPriorityIndicators.Contains(match.IndicatorId)).ToArray();

            foreach (IoKMatch match in found) IoKHandler.Invoke(this, match); 
        }
    }
}