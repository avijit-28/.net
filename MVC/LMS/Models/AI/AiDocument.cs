using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_Documents")]
    public class AiDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public int? ModuleId { get; set; }
        public int? LessonId { get; set; }

        [Required]
        [StringLength(128)]
        public string UploadedBy { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [Required]
        [StringLength(20)]
        public string FileType { get; set; } // PDF, DOCX, PPTX, TXT

        public long FileSize { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentStatus { get; set; } = "Uploaded"; // Uploaded, Processing, Indexed, Failed

        public string ProcessingError { get; set; }

        public int DocumentVersion { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? IndexedAt { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }

        [ForeignKey("UploadedBy")]
        public virtual ApplicationUser Uploader { get; set; }

        public virtual ICollection<AiDocumentChunk> Chunks { get; set; } = new List<AiDocumentChunk>();
        public virtual ICollection<AiIndexingJob> IndexingJobs { get; set; } = new List<AiIndexingJob>();
    }
}
