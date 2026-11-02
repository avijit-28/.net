using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using LMS.Models;

namespace LMS.ViewModels
{
    public class InstructorDashboardViewModel
    {
        public int TotalCourses { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int TotalStudents { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public decimal TotalRevenue { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public decimal AverageRating { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        public List<Course> Courses { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = new List<Course>();
        public List<Enrollment> RecentEnrollments { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = new List<Enrollment>();
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class CourseFormViewModel
    {
        public int CourseId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Description")]
        public string Description { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Thumbnail URL")]
        [StringLength(500)]
        public string Thumbnail { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required]
        [Range(0, 100000, ErrorMessage = "Price must be 0 or positive.")]
        [Display(Name = "Course Price (INR)")]
        public decimal Price { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 0;

        [Required]
        public string Level { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = "Beginner"; // Beginner, Intermediate, Advanced

        [Display(Name = "Duration In Hours")]
        [Range(0, 1000)]
        public decimal DurationInHours { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 0;

        [Display(Name = "Course Access Validity (Days)")]
        [Range(1, 3650, ErrorMessage = "Access validity must be between 1 and 3650 days.")]
        public int ValidityInDays { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 30; // Access validity in days set by instructor

        public string Status { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = "Draft"; // Draft, Published, Archived

        public IEnumerable<SelectListItem> Categories { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class ModuleFormViewModel
    {
        public int ModuleId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int CourseId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required]
        [StringLength(200)]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [StringLength(1000)]
        public string Description { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Order")]
        public int ModuleOrder { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 1;
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class LessonFormViewModel
    {
        public int LessonId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int ModuleId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int CourseId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required]
        [StringLength(200)]
        public string Title { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        public string Description { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required]
        [Display(Name = "Lesson Type")]
        public string LessonType { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = "Video"; // Video, PDF, Article, Link, Quiz, Assignment

        [Display(Name = "Video URL / Embed Link")]
        [StringLength(500)]
        public string VideoUrl { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Article Content / Notes")]
        [AllowHtml]
        public string Content { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Resource File URL / Document")]
        [StringLength(500)]
        public string ResourceUrl { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Duration (Minutes)")]
        public int DurationInMinutes { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 10;

        [Display(Name = "Lesson Order")]
        public int LessonOrder { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 1;

        [Display(Name = "Is Free Preview?")]
        public bool IsPreview { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = false;
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class QuizFormViewModel
    {
        public int QuizId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int CourseId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string CourseTitle { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int? ModuleId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int? LessonId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public bool IsFinalExam { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = false;

        public List<System.Web.Mvc.SelectListItem> ModuleList { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = new List<System.Web.Mvc.SelectListItem>();

        [Required]
        [StringLength(200)]
        public string Title { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        public string Description { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Passing Percentage (%)")]
        [Range(1, 100)]
        public decimal PassingPercentage { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 40;

        [Display(Name = "Time Limit (Minutes)")]
        public int? TimeLimitMinutes { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 30;

        public int TotalMarks { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 0;
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class QuestionOptionInput
    {
        public int OptionId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string OptionText { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public bool IsCorrect { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int OptionOrder { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class QuestionFormViewModel
    {
        public int QuestionId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int QuizId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required]
        [Display(Name = "Question Text")]
        public string QuestionText { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Range(1, 100)]
        public int Marks { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 1;

        public int QuestionOrder { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 1;

        public List<QuestionOptionInput> Options { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = new List<QuestionOptionInput>();
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class AssignmentFormViewModel
    {
        public int AssignmentId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int CourseId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string CourseTitle { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int? ModuleId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int? LessonId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        public List<System.Web.Mvc.SelectListItem> ModuleList { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = new List<System.Web.Mvc.SelectListItem>();

        [Required]
        [StringLength(200)]
        public string Title { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Instructions / Description")]
        public string Description { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Maximum Marks")]
        [Range(1, 1000)]
        public int MaxMarks { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
} = 100;
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

    public class GradeSubmissionViewModel
    {
        public int SubmissionId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public int AssignmentId { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string AssignmentTitle { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string CourseTitle { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string StudentName { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string SubmissionText { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public string FileUrl { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public DateTime SubmittedDate { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Required]
        [Range(0, 1000)]
        [Display(Name = "Marks Awarded")]
        public decimal Marks { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        public int MaxMarks { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        [Display(Name = "Feedback")]
        public string Feedback { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}

        public string Status { get; set;     public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
        public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
    public class InstructorShowcaseViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public int CourseCount { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
