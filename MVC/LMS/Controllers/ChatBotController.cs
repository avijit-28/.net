using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LMS.Data;
using LMS.Models;
using LMS.Models.AI;
using LMS.Services;
using LMS.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IAiServiceClient _aiClient;
        private readonly string _internalApiKey;

        public ChatBotController()
        {
            _db = new ApplicationDbContext();
            _aiClient = new AiServiceClient();
            _internalApiKey = ConfigurationManager.AppSettings["AIService:InternalApiKey"] ?? "lms_rag_secret_auth_token_2026_xyz987";
        }

        public ChatBotController(ApplicationDbContext db, IAiServiceClient aiClient)
        {
            _db = db;
            _aiClient = aiClient;
            _internalApiKey = ConfigurationManager.AppSettings["AIService:InternalApiKey"] ?? "lms_rag_secret_auth_token_2026_xyz987";
        }

        // GET: /ChatBot or /Home/AiAssistant
        [AllowAnonymous]
        public async Task<ActionResult> Index(int? courseId = null, int? sessionId = null)
        {
            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            string userRole = GetCurrentUserRole();

            // Fetch available courses based on role & enrollment
            var courseOptions = new List<CourseOptionViewModel>();
            if (!string.IsNullOrEmpty(userId))
            {
                if (userRole == "Admin")
                {
                    courseOptions = await _db.Courses
                        .Select(c => new CourseOptionViewModel
                        {
                            CourseId = c.CourseId,
                            Title = c.Title,
                            CategoryName = c.Category != null ? c.Category.CategoryName : "General",
                            IsEnrolled = true
                        }).ToListAsync();
                }
                else if (userRole == "Instructor")
                {
                    courseOptions = await _db.Courses
                        .Where(c => c.InstructorId == userId)
                        .Select(c => new CourseOptionViewModel
                        {
                            CourseId = c.CourseId,
                            Title = c.Title,
                            CategoryName = c.Category != null ? c.Category.CategoryName : "General",
                            IsEnrolled = true
                        }).ToListAsync();
                }
                else // Student
                {
                    courseOptions = await _db.Enrollments
                        .Where(e => e.StudentId == userId && e.Status == "Active")
                        .Select(e => new CourseOptionViewModel
                        {
                            CourseId = e.CourseId,
                            Title = e.Course.Title,
                            CategoryName = e.Course.Category != null ? e.Course.Category.CategoryName : "General",
                            IsEnrolled = true
                        }).ToListAsync();
                }
            }

            AiChatSession session = null;
            if (sessionId.HasValue)
            {
                session = await _db.AiChatSessions
                    .Include(s => s.Messages.Select(m => m.Sources))
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId.Value);
            }
            else if (!string.IsNullOrEmpty(userId))
            {
                var query = _db.AiChatSessions
                    .Include(s => s.Messages.Select(m => m.Sources))
                    .Where(s => s.UserId == userId);

                if (courseId.HasValue)
                {
                    query = query.Where(s => s.CourseId == courseId.Value);
                }

                session = await query.OrderByDescending(s => s.LastActiveAt).FirstOrDefaultAsync();
            }

            if (session == null)
            {
                session = new AiChatSession
                {
                    UserId = userId ?? "anonymous_guest",
                    CourseId = courseId,
                    SessionTitle = courseId.HasValue ? "Course Discussion" : "General Academic Consultation",
                    LearningMode = "CourseAssistant",
                    CreatedAt = DateTime.UtcNow,
                    LastActiveAt = DateTime.UtcNow
                };

                if (!string.IsNullOrEmpty(userId))
                {
                    _db.AiChatSessions.Add(session);
                    await _db.SaveChangesAsync();
                }
            }

            var vm = new AiStudioViewModel
            {
                SessionGuid = session.SessionId.ToString(),
                ActiveCourseId = session.CourseId,
                ActiveCourseTitle = session.CourseId.HasValue ? (await _db.Courses.FindAsync(session.CourseId.Value))?.Title : "All Enrolled Curricula",
                ActiveLearningMode = session.LearningMode ?? "CourseAssistant",
                EnrolledCourses = courseOptions,
                IsUserLoggedIn = User.Identity.IsAuthenticated,
                UserRole = userRole
            };

            return View(vm);
        }

        // POST: /ChatBot/SendMessage (AJAX endpoint)
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> SendMessage(ChatRequestViewModel request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return Json(new ChatResponseViewModel
                {
                    Success = false,
                    ErrorMessage = "Please enter a message or question."
                });
            }

            try
            {
                string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "anonymous_guest";
                string userRole = GetCurrentUserRole();

                // 1. Enforce RBAC Course Isolation for Students
                if (request.CourseId.HasValue && userRole == "Student" && User.Identity.IsAuthenticated)
                {
                    bool isEnrolled = await _db.Enrollments
                        .AnyAsync(e => e.StudentId == userId && e.CourseId == request.CourseId.Value && e.Status == "Active");

                    if (!isEnrolled)
                    {
                        return Json(new ChatResponseViewModel
                        {
                            Success = false,
                            ErrorMessage = "Access Denied: You are not actively enrolled in this course."
                        });
                    }
                }

                // 2. Compute Authorized Course IDs for the retrieval security filter
                string authorizedCoursesHeader = await GetAuthorizedCoursesHeaderAsync(userId, userRole);

                // 3. Resolve or Create Chat Session
                int sessionId = 0;
                int.TryParse(request.SessionGuid, out sessionId);

                AiChatSession session = null;
                if (sessionId > 0)
                {
                    session = await _db.AiChatSessions
                        .Include(s => s.Messages)
                        .FirstOrDefaultAsync(s => s.SessionId == sessionId);
                }

                if (session == null && User.Identity.IsAuthenticated)
                {
                    session = new AiChatSession
                    {
                        UserId = userId,
                        CourseId = request.CourseId,
                        SessionTitle = TruncateTitle(request.Message),
                        LearningMode = request.LearningMode ?? "CourseAssistant",
                        CreatedAt = DateTime.UtcNow,
                        LastActiveAt = DateTime.UtcNow
                    };
                    _db.AiChatSessions.Add(session);
                    await _db.SaveChangesAsync();
                }
                else if (session != null)
                {
                    session.LastActiveAt = DateTime.UtcNow;
                    if (request.CourseId.HasValue) session.CourseId = request.CourseId;
                    if (!string.IsNullOrEmpty(request.LearningMode)) session.LearningMode = request.LearningMode;
                }

                // 4. Save User Message to Database
                AiChatMessage userMsg = null;
                if (session != null && session.SessionId > 0)
                {
                    userMsg = new AiChatMessage
                    {
                        SessionId = session.SessionId,
                        SenderRole = "User",
                        MessageText = request.Message.Trim(),
                        SentAt = DateTime.UtcNow
                    };
                    _db.AiChatMessages.Add(userMsg);
                    await _db.SaveChangesAsync();
                }

                // 5. Build History and Request for AI Microservice
                var recentHistory = new List<Dictionary<string, string>>();
                if (session != null && session.Messages != null)
                {
                    recentHistory = session.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Take(6)
                        .OrderBy(m => m.SentAt)
                        .Select(m => new Dictionary<string, string>
                        {
                            { "role", m.SenderRole.ToLower() == "assistant" ? "assistant" : "user" },
                            { "content", m.MessageText }
                        }).ToList();
                }

                var aiRequest = new AiChatRequestDto
                {
                    query = request.Message.Trim(),
                    user_id = userId,
                    user_role = userRole,
                    session_id = session?.SessionId.ToString(),
                    course_id = request.CourseId,
                    module_id = request.ModuleId,
                    lesson_id = request.LessonId,
                    learning_mode = request.LearningMode ?? "CourseAssistant",
                    history = recentHistory
                };

                // 6. Invoke FastAPI AI Service
                var aiResponse = await _aiClient.AskQuestionAsync(aiRequest, _internalApiKey, authorizedCoursesHeader);

                // 7. Save Assistant Message, Citations & Telemetry
                if (session != null && session.SessionId > 0)
                {
                    var assistantMsg = new AiChatMessage
                    {
                        SessionId = session.SessionId,
                        SenderRole = "Assistant",
                        MessageText = aiResponse.answer,
                        IsGroundedInRag = aiResponse.grounded_in_rag,
                        LlmProvider = aiResponse.llm_provider ?? "gemini",
                        ModelUsed = aiResponse.model_used ?? "gemini-3.1-flash-lite",
                        TotalTokens = aiResponse.tokens_used,
                        LatencyMs = aiResponse.duration_ms,
                        WasCacheHit = aiResponse.was_cache_hit,
                        SentAt = DateTime.UtcNow
                    };
                    _db.AiChatMessages.Add(assistantMsg);
                    await _db.SaveChangesAsync();

                    // Save Citations
                    if (aiResponse.sources != null && aiResponse.sources.Any())
                    {
                        foreach (var cite in aiResponse.sources)
                        {
                            _db.AiChatSources.Add(new AiChatSource
                            {
                                MessageId = assistantMsg.MessageId,
                                DocumentId = cite.document_id,
                                SourceTitle = cite.source_title ?? "Course Knowledge",
                                PageNumber = cite.page_number,
                                RelevanceScore = cite.relevance_score,
                                Excerpt = (cite.excerpt?.Length > 950 ? cite.excerpt.Substring(0, 950) + "..." : cite.excerpt) ?? ""
                            });
                        }
                    }

                    // Save Usage Log
                    _db.AiUsageLogs.Add(new AiUsageLog
                    {
                        UserId = userId,
                        CourseId = request.CourseId,
                        QueryType = "RAG",
                        LlmProvider = aiResponse.llm_provider ?? "gemini",
                        ModelName = aiResponse.model_used ?? "gemini-3.1-flash-lite",
                        PromptTokens = 0,
                        CompletionTokens = 0,
                        TotalTokens = aiResponse.tokens_used,
                        EstimatedCost = (decimal)(aiResponse.tokens_used * 0.00000015),
                        LatencyMs = aiResponse.duration_ms,
                        WasCacheHit = aiResponse.was_cache_hit,
                        Timestamp = DateTime.UtcNow
                    });

                    await _db.SaveChangesAsync();
                }

                // 8. Extract Relevant Course Recommendations
                var recommendedCourses = await ExtractCourseRecommendationsAsync(request.Message, userId);

                var telemetryDto = new AiTelemetryDto
                {
                    provider = aiResponse.llm_provider,
                    model = aiResponse.model_used,
                    total_tokens = aiResponse.tokens_used,
                    latency_ms = aiResponse.duration_ms,
                    was_cache_hit = aiResponse.was_cache_hit,
                    estimated_cost_usd = (aiResponse.tokens_used * 0.00000015).ToString("0.000000")
                };

                return Json(new ChatResponseViewModel
                {
                    Success = true,
                    Reply = aiResponse.answer,
                    SessionGuid = session?.SessionId.ToString() ?? "0",
                    ModelUsed = aiResponse.model_used ?? "gemini-3.1-flash-lite",
                    Provider = aiResponse.llm_provider ?? "gemini",
                    IsGrounded = aiResponse.grounded_in_rag,
                    AgenticIntent = "RAG",
                    Citations = aiResponse.sources ?? new List<AiCitationDto>(),
                    Telemetry = telemetryDto,
                    RecommendedCourses = recommendedCourses,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Json(new ChatResponseViewModel
                {
                    Success = false,
                    ErrorMessage = "An error occurred while processing your request: " + ex.Message
                });
            }
        }

        // POST: /ChatBot/SubmitFeedback
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> SubmitFeedback(int messageId, bool isHelpful, string feedbackText = null)
        {
            string userId = User.Identity.GetUserId();
            var feedback = new AiFeedback
            {
                MessageId = messageId,
                UserId = userId,
                IsHelpful = isHelpful,
                FeedbackText = feedbackText,
                CreatedAt = DateTime.UtcNow
            };
            _db.AiFeedbacks.Add(feedback);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: /ChatBot/GeneratePracticeQuiz
        [HttpPost]
        public async Task<JsonResult> GeneratePracticeQuiz(AiQuizRequestDto request)
        {
            if (request == null || request.course_id <= 0)
            {
                return Json(new { success = false, message = "Invalid course specified for practice quiz." });
            }

            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "guest";
            string userRole = GetCurrentUserRole();
            string authHeader = await GetAuthorizedCoursesHeaderAsync(userId, userRole);

            request.user_id = userId;
            request.user_role = userRole;

            var result = await _aiClient.GenerateQuizAsync(request, _internalApiKey, authHeader);
            if (result == null)
            {
                return Json(new { success = false, message = "Failed to generate practice quiz. Please verify course content is indexed." });
            }

            return Json(new { success = true, quiz = result });
        }

        // POST: /ChatBot/SummarizeContent
        [HttpPost]
        public async Task<JsonResult> SummarizeContent(AiSummarizeRequestDto request)
        {
            if (request == null || request.course_id <= 0)
            {
                return Json(new { success = false, message = "Invalid course specified for summarization." });
            }

            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "guest";
            string userRole = GetCurrentUserRole();
            string authHeader = await GetAuthorizedCoursesHeaderAsync(userId, userRole);

            request.user_id = userId;
            request.user_role = userRole;

            var result = await _aiClient.SummarizeAsync(request, _internalApiKey, authHeader);
            if (result == null)
            {
                return Json(new { success = false, message = "Failed to summarize content." });
            }

            return Json(new { success = true, summary = result });
        }

        // POST: /ChatBot/ExplainCode
        [HttpPost]
        public async Task<JsonResult> ExplainCode(AiCodeExplainRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.code_snippet))
            {
                return Json(new { success = false, message = "Please provide code to analyze." });
            }

            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "guest";
            string userRole = GetCurrentUserRole();
            string authHeader = await GetAuthorizedCoursesHeaderAsync(userId, userRole);

            request.user_id = userId;
            request.user_role = userRole;

            var result = await _aiClient.ExplainCodeAsync(request, _internalApiKey, authHeader);
            if (result == null)
            {
                return Json(new { success = false, message = "Failed to evaluate code snippet." });
            }

            return Json(new { success = true, explanation = result });
        }

        // POST: /ChatBot/GenerateStudyPlan
        [HttpPost]
        public async Task<JsonResult> GenerateStudyPlan(AiStudyPlanRequestDto request)
        {
            if (request == null || request.course_id <= 0)
            {
                return Json(new { success = false, message = "Invalid course specified for study planner." });
            }

            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "guest";
            string userRole = GetCurrentUserRole();
            string authHeader = await GetAuthorizedCoursesHeaderAsync(userId, userRole);

            request.user_id = userId;
            request.user_role = userRole;

            // Fetch actual student progress if logged in
            if (User.Identity.IsAuthenticated)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                request.student_name = user?.FullName ?? "Learner";

                int totalLessons = await _db.Lessons.CountAsync(l => l.Module.CourseId == request.course_id);
                int completedLessons = await _db.LessonProgresses
                    .CountAsync(lp => lp.StudentId == userId && lp.Lesson.Module.CourseId == request.course_id && lp.IsCompleted);

                request.total_lessons = Math.Max(totalLessons, 1);
                request.completed_lessons = completedLessons;
            }

            var result = await _aiClient.GenerateStudyPlanAsync(request, _internalApiKey, authHeader);
            if (result == null)
            {
                return Json(new { success = false, message = "Failed to generate study plan." });
            }

            return Json(new { success = true, plan = result });
        }

        // Helper: Compute Authorized Courses Header
        private async Task<string> GetAuthorizedCoursesHeaderAsync(string userId, string userRole)
        {
            if (userRole == "Admin") return "*";

            if (userRole == "Instructor" && !string.IsNullOrEmpty(userId))
            {
                var courseIds = await _db.Courses
                    .Where(c => c.InstructorId == userId)
                    .Select(c => c.CourseId)
                    .ToListAsync();
                return string.Join(",", courseIds);
            }

            if (!string.IsNullOrEmpty(userId) && userId != "anonymous_guest")
            {
                var enrolledCourseIds = await _db.Enrollments
                    .Where(e => e.StudentId == userId && e.Status == "Active")
                    .Select(e => e.CourseId)
                    .ToListAsync();
                return string.Join(",", enrolledCourseIds);
            }

            return "";
        }

        private string GetCurrentUserRole()
        {
            if (User.IsInRole("Admin")) return "Admin";
            if (User.IsInRole("Instructor")) return "Instructor";
            return "Student";
        }

        private string TruncateTitle(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return "New Learning Session";
            var clean = message.Trim();
            return clean.Length > 40 ? clean.Substring(0, 37) + "..." : clean;
        }

        private async Task<List<CourseRecommendationCard>> ExtractCourseRecommendationsAsync(string query, string userId)
        {
            var cards = new List<CourseRecommendationCard>();
            try
            {
                var tokens = (query ?? "").ToLower().Split(new[] { ' ', ',', '.', '?' }, StringSplitOptions.RemoveEmptyEntries);
                var courses = await _db.Courses.Include(c => c.Category).Where(c => c.Status == "Published").ToListAsync();

                foreach (var c in courses)
                {
                    bool match = tokens.Any(t => t.Length > 2 && (c.Title.ToLower().Contains(t) || (c.Category != null && c.Category.CategoryName.ToLower().Contains(t))));
                    if (match)
                    {
                        bool isEnrolled = false;
                        int progress = 0;
                        if (!string.IsNullOrEmpty(userId) && userId != "anonymous_guest")
                        {
                            var enrollment = await _db.Enrollments.FirstOrDefaultAsync(e => e.CourseId == c.CourseId && e.StudentId == userId);
                            if (enrollment != null)
                            {
                                isEnrolled = true;
                                progress = (int)enrollment.ProgressPercentage;
                            }
                        }

                        cards.Add(new CourseRecommendationCard
                        {
                            CourseId = c.CourseId,
                            Title = c.Title,
                            CategoryName = c.Category != null ? c.Category.CategoryName : "Technology",
                            Level = c.Level ?? "Beginner",
                            Price = c.Price,
                            ThumbnailUrl = c.Thumbnail ?? "/Content/images/default-course.jpg",
                            DetailUrl = Url.Action("Details", "Courses", new { id = c.CourseId }),
                            LearnUrl = Url.Action("Player", "Learning", new { id = c.CourseId }),
                            IsEnrolled = isEnrolled,
                            ProgressPercent = progress
                        });

                        if (cards.Count >= 2) break;
                    }
                }
            }
            catch { }
            return cards;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
