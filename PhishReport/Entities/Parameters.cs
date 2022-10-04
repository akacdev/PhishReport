using System.Text.Json.Serialization;

namespace PhishReport
{
    public class CreateCaseParameters
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}