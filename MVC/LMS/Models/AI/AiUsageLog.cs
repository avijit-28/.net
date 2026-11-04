using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_UsageLogs")]
    public class AiUsageLog
    {
        [Key]
        public int UsageId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int? SessionId { get; set; }

        [Required]
        [StringLength(100)]
        public string ModelName { get; set; }

        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }

        public decimal EstimatedCost { get; set; } = 0.0m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
