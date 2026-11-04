using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_DocumentChunks")]
    public class AiDocumentChunk
    {
        [Key]
        public int ChunkId { get; set; }

        [Required]
        public int DocumentId { get; set; }

        public int ChunkIndex { get; set; }

        [Required]
        public string ExtractedText { get; set; }

        public int? PageNumber { get; set; }

        [StringLength(255)]
        public string SectionTitle { get; set; }

        [Required]
        [StringLength(128)]
        public string VectorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("DocumentId")]
        public virtual AiDocument Document { get; set; }
    }
}
