using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("AI_ChatSources")]
    public class AiChatSource
    {
        [Key]
        public int SourceId { get; set; }

        [Required]
        public int MessageId { get; set; }

        public int? DocumentId { get; set; }
        public int? ChunkId { get; set; }

        [Required]
        [StringLength(255)]
        public string SourceTitle { get; set; }

        public int? PageNumber { get; set; }

        public double RelevanceScore { get; set; }

        [StringLength(1000)]
        public string Excerpt { get; set; }

        [ForeignKey("MessageId")]
        public virtual AiChatMessage Message { get; set; }

        [ForeignKey("DocumentId")]
        public virtual AiDocument Document { get; set; }

        [ForeignKey("ChunkId")]
        public virtual AiDocumentChunk Chunk { get; set; }
    }
}
