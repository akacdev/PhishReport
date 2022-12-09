using System;
using System.Text.RegularExpressions;

namespace PhishReport
{
    internal class Constants
    {
        public const string AcceptEncoding = "gzip, deflate, br";
        public const string UserAgent = "Phish.Report C# Client - actually-akac/PhishReport";

        public const int MaxPreviewLength = 500;
        public const string JsonContentType = "application/json";

        /// <summary>
        /// The current version of the API.
        /// </summary>
        public const int Version = 0;
        /// <summary>
        /// The base URI for the API.
        /// </summary>
        public static readonly Uri BaseUri = new($"https://phish.report/");
        /// <summary>
        /// How often Indicator of Kit matches should be polled for changes.
        /// </summary>
        public const int IoKPollInterval = 1000 * 60;
        /// <summary>
        /// Low priority Indicators of Kit.
        /// </summary>
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