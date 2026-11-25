using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Assignment3.Models
{
    public class MeterReadingDto
    {
        [JsonPropertyName("meterSerialNumber")]
        public string MeterSerialNumber { get; set; } = string.Empty;

        [JsonPropertyName("externalReadingId")]
        public string? ExternalReadingId { get; set; }

        [JsonPropertyName("batchId")]
        public string BatchId { get; set; } = string.Empty;

        [JsonPropertyName("readingDate")]
        public DateTime ReadingDate { get; set; }

        [JsonPropertyName("cumulativeKWh")]
        public decimal CumulativeKWh { get; set; }
    }
}
