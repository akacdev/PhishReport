using System.Text.Json.Serialization;

namespace PhishReport
{
    /// <summary>
    /// Parameters for a phishing takedown action.
    /// </summary>
    public class ActionParameters
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("cc")]
        public string CC { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }
    }

    /// <summary>
    /// Parameters for creating a new case.
    /// </summary>
    public class CreateCaseParameters
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// An action that can be performed by the user.
    /// </summary>
    public class PendingAction
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("params")]
        public ActionParameters Params { get; set; }
    }

    /// <summary>
    /// A newly created or existing phishing takedown.
    /// </summary>
    public class PhishingTakedown
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("pending_actions")]
        public PendingAction[] PendingActions { get; set; }
    }
}