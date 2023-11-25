using System;
using System.Text.RegularExpressions;

namespace PhishReport
{
    internal class Constants
    {
        /// <summary>
        /// The version of the API to send requests to.
        /// </summary>
        public const int Version = 0;
        /// <summary>
        /// The base URI to send requests to.
        /// </summary>
        public static readonly Uri BaseUri = new($"https://phish.report/");
        /// <summary>
        /// The preferred HTTP request version to use.
        /// </summary>
        public static readonly Version HttpVersion = new(2, 0);
        /// <summary>
        /// The <c>User-Agent</c> header value to send along requests.
        /// </summary>
        public const string UserAgent = "Phish.Report C# Client - actually-akac/PhishReport";
        /// <summary>
        /// The maximum string length when displaying a preview of a response body.
        /// </summary>
        public const int PreviewMaxLength = 500;
        /// <summary>
        /// How often Indicator of Kit matches should be polled for changes.
        /// </summary>
        public const int IoKPollInterval = 1000 * 60;

        public static readonly string[] LowPriorityIndicators = new string[]
        {
            "hex-encoded-body",
            "base64-encoded-body",
            "httrack",
            "recaptcha",
            "webscrapbook-cloner",
            "savepage-we"
        };

        public static readonly Regex IokIndicatorRegex = new(@"href=\""\/IOK\/indicators\/(.+)\"">", RegexOptions.Multiline | RegexOptions.Compiled);
    }
}