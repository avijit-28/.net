using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    [Table("Enrollments")]
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        [StringLength(450)]
        public string StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Active"; // Active, Completed, Cancelled

        public decimal CompletionPercentage { get; set; } = 0;

        public DateTime? CompletionDate { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
