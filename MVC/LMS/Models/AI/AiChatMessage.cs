using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_ChatMessages")]
    public class AiChatMessage
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        [StringLength(20)]
        public string Sender { get; set; } // User, Assistant

        [Required]
        public string MessageText { get; set; }

        public int TokenCount { get; set; } = 0;
        public int ResponseDuration { get; set; } = 0; // ms

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("SessionId")]
        public virtual AiChatSession Session { get; set; }

        public virtual ICollection<AiChatSource> Sources { get; set; } = new List<AiChatSource>();
        public virtual ICollection<AiFeedback> Feedbacks { get; set; } = new List<AiFeedback>();
    }
}
