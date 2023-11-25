using System.Text.Json.Serialization;

namespace PhishReport
{
    /// <summary>
    /// An Indicator of Kit match.
    /// More Info: <a href="https://phish.report/IOK/">https://phish.report/IOK/</a>
    /// </summary>
    public class IokMatch
    {
        [JsonPropertyName("id")]
        public string IndicatorId { get; set; }

        [JsonPropertyName("urlscanUUID")]
        public string UrlscanUuid { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
    }

    /// <summary>
    /// A container that holds Indicator of Kit matches.
    /// </summary>
    public class IokMatchContainer
    {
        [JsonPropertyName("matches")]
        public IokMatch[] Matches { get; set; }
    }
}