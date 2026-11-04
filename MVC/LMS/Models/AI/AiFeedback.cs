using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_Feedback")]
    public class AiFeedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int MessageId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int Rating { get; set; } // 1 (Helpful) or -1 (Unhelpful)

        [StringLength(1000)]
        public string FeedbackText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("MessageId")]
        public virtual AiChatMessage Message { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
