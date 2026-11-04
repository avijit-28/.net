using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_ChatSessions")]
    public class AiChatSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int? CourseId { get; set; }

        [Required]
        [StringLength(255)]
        public string SessionTitle { get; set; }

        [Required]
        [StringLength(50)]
        public string Mode { get; set; } = "CourseAssistant"; // CourseAssistant, DocumentAssistant, StudyAssistant, QuizGenerator, CodingAssistant

        public bool IsArchived { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public virtual ICollection<AiChatMessage> Messages { get; set; } = new List<AiChatMessage>();
    }
}
