using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment3.Models
{
    public class AnomalousReadingRecord
    {
        public long ReadingId { get; set; }

        public string MeterSerialNumber { get; set; } = string.Empty;

        public int? FeederId { get; set; }

        public DateTime ReadingDate { get; set; }

        public decimal CumulativeKWh { get; set; }

        public decimal? PreviousCumulativeKWh { get; set; }

        public decimal? ConsumptionKWh { get; set; }

        public string ReadingStatus { get; set; } = string.Empty;
    }
}
