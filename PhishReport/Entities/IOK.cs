using System.Text.Json.Serialization;

namespace PhishReport
{
    /// <summary>
    /// An Indicator of Kit match.
    /// More Info: <a href="https://phish.report/IOK/">https://phish.report/IOK/</a>
    /// </summary>
    public class IoKMatch
    {
        [JsonPropertyName("id")]
        public string IndicatorId { get; set; }

        [JsonPropertyName("urlscanUUID")]
        public string UrlscanUUID { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
    }

    /// <summary>
    /// A container that holds Indicator of Kit matches.
    /// </summary>
    public class IoKMatchContainer
    {
        [JsonPropertyName("matches")]
        public IoKMatch[] Matches { get; set; }
    }
}