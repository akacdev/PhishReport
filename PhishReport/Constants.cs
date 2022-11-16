namespace PhishReport
{
    internal class Constants
    {
        public const string JsonContentType = "application/json";

        /// <summary>
        /// The current version of the API.
        /// </summary>
        public const int Version = 0;
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
    }
}
