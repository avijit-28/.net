using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment3.Models
{
    public class ReconciliationRunResult
    {
        public Guid RunId { get; set; }

        public string BatchId { get; set; } = string.Empty;

        public int TotalReadings { get; set; }

        public int InsertedReadings { get; set; }

        public int DuplicateReadings { get; set; }

        public int RejectedReadings { get; set; }

        public int AnomalyReadings { get; set; }

        public int FailedReadings { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
