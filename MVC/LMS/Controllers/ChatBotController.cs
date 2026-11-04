using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LMS.Data;
using LMS.Models;
using LMS.Services;
using LMS.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IRagKnowledgeService _ragService;
        private readonly IGeminiChatService _geminiService;

        public ChatBotController()
        {
            _db = new ApplicationDbContext();
            _ragService = new RagKnowledgeService(_db);
            _geminiService = new GeminiChatService();
        }

        public ChatBotController(ApplicationDbContext db, IRagKnowledgeService ragService, IGeminiChatService geminiService)
        {
            _db = db;
            _ragService = ragService;
            _geminiService = geminiService;
        }

        // GET: /ChatBot or /Home/AiAssistant
        [AllowAnonymous]
        public async Task<ActionResult> Index(string sessionGuid = null)
        {
            await _ragService.EnsureIndexPopulatedAsync();

            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            ChatSession session = null;

            if (!string.IsNullOrEmpty(sessionGuid))
            {
                session = await _db.ChatSessions
                    .Include(s => s.Messages)
                    .FirstOrDefaultAsync(s => s.SessionGuid == sessionGuid);
            }
            else if (!string.IsNullOrEmpty(userId))
            {
                session = await _db.ChatSessions
                    .Include(s => s.Messages)
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.LastActiveAt)
                    .FirstOrDefaultAsync();
            }

            if (session == null)
            {
                session = new ChatSession
                {
                    SessionGuid = Guid.NewGuid().ToString("N"),
                    UserId = userId,
                    Title = "Academic Consultation",
                    CreatedAt = DateTime.UtcNow,
                    LastActiveAt = DateTime.UtcNow
                };
                _db.ChatSessions.Add(session);
                await _db.SaveChangesAsync();
            }

            ViewBag.SessionGuid = session.SessionGuid;
            ViewBag.ActiveUserId = userId;

            return View(session);
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
                await _ragService.EnsureIndexPopulatedAsync();

                string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
                string sessionGuid = request.SessionGuid;

                ChatSession session = null;
                if (!string.IsNullOrEmpty(sessionGuid))
                {
                    session = await _db.ChatSessions
                        .Include(s => s.Messages)
                        .FirstOrDefaultAsync(s => s.SessionGuid == sessionGuid);
                }

                if (session == null)
                {
                    session = new ChatSession
                    {
                        SessionGuid = string.IsNullOrEmpty(sessionGuid) ? Guid.NewGuid().ToString("N") : sessionGuid,
                        UserId = userId,
                        Title = TruncateTitle(request.Message),
                        CreatedAt = DateTime.UtcNow,
                        LastActiveAt = DateTime.UtcNow
                    };
                    _db.ChatSessions.Add(session);
                }
                else
                {
                    session.LastActiveAt = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(session.UserId) && !string.IsNullOrEmpty(userId))
                    {
                        session.UserId = userId;
                    }
                }

                // 1. Save User Message
                var userMsg = new ChatMessage
                {
                    Session = session,
                    Sender = "User",
                    Content = request.Message.Trim(),
                    Timestamp = DateTime.UtcNow
                };
                _db.ChatMessages.Add(userMsg);

                // 2. RAG Retrieval Step: Retrieve most relevant knowledge chunks
                var retrievedChunks = await _ragService.SearchKnowledgeAsync(request.Message, topK: 4);
                string chunkIdsStr = string.Join(",", retrievedChunks.Select(c => c.ChunkId));

                // 3. User Learning State Extraction
                string userContext = await _ragService.GetUserLearningContextAsync(userId);

                // 4. Extract Interactive Course Cards
                var courseCards = await _ragService.ExtractCourseRecommendationsAsync(request.Message, retrievedChunks, userId);
                string actionPayload = courseCards.Any() ? JsonConvert.SerializeObject(courseCards) : null;

                // 5. LLM Synthesis Step: Call Gemini API (with smart local fallback)
                var recentHistory = session.Messages.OrderByDescending(m => m.Timestamp).Take(6).OrderBy(m => m.Timestamp).ToList();
                var aiResult = await _geminiService.GenerateResponseAsync(request.Message, retrievedChunks, userContext, recentHistory);

                // 6. Save Assistant Response
                var assistantMsg = new ChatMessage
                {
                    Session = session,
                    Sender = "Assistant",
                    Content = aiResult.Reply,
                    RetrievedChunkIds = chunkIdsStr,
                    ActionPayloadJson = actionPayload,
                    Timestamp = DateTime.UtcNow
                };
                _db.ChatMessages.Add(assistantMsg);

                await _db.SaveChangesAsync();

                aiResult.SessionGuid = session.SessionGuid;
                aiResult.RecommendedCourses = courseCards;

                return Json(aiResult);
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

        // GET: /ChatBot/GetHistory
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetHistory(string sessionGuid)
        {
            if (string.IsNullOrEmpty(sessionGuid))
            {
                return Json(new { success = false, message = "Session ID required" }, JsonRequestBehavior.AllowGet);
            }

            var session = await _db.ChatSessions
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.SessionGuid == sessionGuid);

            if (session == null)
            {
                return Json(new { success = true, messages = new List<ChatHistoryMessageDto>() }, JsonRequestBehavior.AllowGet);
            }

            var dtos = session.Messages
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatHistoryMessageDto
                {
                    MessageId = m.MessageId,
                    Sender = m.Sender,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    RecommendedCourses = !string.IsNullOrEmpty(m.ActionPayloadJson)
                        ? JsonConvert.DeserializeObject<List<CourseRecommendationCard>>(m.ActionPayloadJson)
                        : new List<CourseRecommendationCard>()
                })
                .ToList();

            return Json(new { success = true, sessionGuid = session.SessionGuid, messages = dtos }, JsonRequestBehavior.AllowGet);
        }

        // POST: /ChatBot/ClearHistory
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ClearHistory(string sessionGuid)
        {
            if (!string.IsNullOrEmpty(sessionGuid))
            {
                var session = await _db.ChatSessions
                    .Include(s => s.Messages)
                    .FirstOrDefaultAsync(s => s.SessionGuid == sessionGuid);

                if (session != null)
                {
                    _db.ChatMessages.RemoveRange(session.Messages);
                    await _db.SaveChangesAsync();
                }
            }

            return Json(new { success = true, message = "Chat history cleared." });
        }

        // POST: /ChatBot/Reindex (Admin Only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> Reindex()
        {
            await _ragService.RebuildIndexAsync();
            int count = await _db.KnowledgeChunks.CountAsync();
            return Json(new { success = true, message = $"Knowledge index rebuilt successfully with {count} chunks." });
        }

        private static string TruncateTitle(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "Academic Consultation";
            string clean = input.Trim();
            return clean.Length > 40 ? clean.Substring(0, 37) + "..." : clean;
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
