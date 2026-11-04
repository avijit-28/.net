using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_IndexingJobs")]
    public class AiIndexingJob
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [Required]
        [StringLength(50)]
        public string JobStatus { get; set; } = "Queued"; // Queued, InProgress, Completed, Failed

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string ErrorMessage { get; set; }

        public int RetryCount { get; set; } = 0;

        [ForeignKey("DocumentId")]
        public virtual AiDocument Document { get; set; }
    }
}
