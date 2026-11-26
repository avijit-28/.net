using System.Text.Json.Serialization;

namespace Assignment3.Models
{
    public class DispatchAlertDto
    {
        [JsonPropertyName("alertId")]
        public string AlertId { get; set; } = string.Empty;

        [JsonPropertyName("feederId")]
        public string FeederId { get; set; } = string.Empty;

        [JsonPropertyName("severity")]
        public string Severity { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
