using System;
using System.Collections.Generic;

namespace LMS.ViewModels
{
    public class ChatRequestViewModel
    {
        public string Message { get; set; }
        public string SessionGuid { get; set; }
        public int? CourseId { get; set; }
    }

    public class CourseRecommendationCard
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string Level { get; set; }
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public string DetailUrl { get; set; }
        public string LearnUrl { get; set; }
        public bool IsEnrolled { get; set; }
        public int ProgressPercent { get; set; }
    }

    public class ChatResponseViewModel
    {
        public bool Success { get; set; } = true;
        public string Reply { get; set; }
        public string SessionGuid { get; set; }
        public string ModelUsed { get; set; }
        public List<string> Suggestions { get; set; } = new List<string>();
        public List<CourseRecommendationCard> RecommendedCourses { get; set; } = new List<CourseRecommendationCard>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ErrorMessage { get; set; }
    }

    public class ChatHistoryMessageDto
    {
        public int MessageId { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public List<CourseRecommendationCard> RecommendedCourses { get; set; } = new List<CourseRecommendationCard>();
    }
}
