using System.Text.Json.Serialization;

namespace Assignment3.Models
{
    public class TariffUpdateDto
    {
        [JsonPropertyName("slabId")]
        public int SlabId { get; set; }

        [JsonPropertyName("feederId")]
        public string FeederId { get; set; } = string.Empty;

        [JsonPropertyName("slabName")]
        public string SlabName { get; set; } = string.Empty;

        [JsonPropertyName("minUnits")]
        public decimal MinUnits { get; set; }

        [JsonPropertyName("maxUnits")]
        public decimal? MaxUnits { get; set; }

        [JsonPropertyName("ratePerUnit")]
        public decimal RatePerUnit { get; set; }
    }
}
