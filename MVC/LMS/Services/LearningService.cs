using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using LMS.Data;
using LMS.Models;
using LMS.ViewModels;

namespace LMS.Services
{
    public class LearningService
    {
        private readonly ApplicationDbContext _db;
        private readonly CertificateService _certificateService;

        public LearningService(ApplicationDbContext db)
        {
            _db = db;
            _certificateService = new CertificateService(db);
        }

        public LearningViewModel GetLearningState(int courseId, string studentId, int? lessonId = null)
        {
            var course = _db.Courses
                .Include(c => c.Modules.Select(m => m.Lessons))
                .Include(c => c.Quizzes)
                .Include(c => c.Assignments)
                .FirstOrDefault(c => c.CourseId == courseId);

            if (course == null) return null;

            var enrollment = _db.Enrollments
                .FirstOrDefault(e => e.CourseId == courseId && e.StudentId == studentId && e.Status != "Cancelled");

            if (enrollment == null)
            {
                // Allow course instructor or admin to preview classroom
                bool isInstructor = !string.IsNullOrEmpty(studentId) && course.InstructorId == studentId;
                bool isAdmin = false;
                if (!string.IsNullOrEmpty(studentId))
                {
                    var adminRoleId = _db.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();
                    if (!string.IsNullOrEmpty(adminRoleId))
                    {
                        isAdmin = _db.Database.SqlQuery<int>(
                            "SELECT COUNT(*) FROM AspNetUserRoles WHERE UserId = @uid AND RoleId = @rid",
                            new System.Data.SqlClient.SqlParameter("@uid", studentId),
                            new System.Data.SqlClient.SqlParameter("@rid", adminRoleId)
                        ).FirstOrDefault() > 0;
                    }
                }

                if (!isInstructor && !isAdmin)
                {
                    return null;
                }
            }

            var modules = course.Modules.OrderBy(m => m.ModuleOrder).ToList();
            var allLessons = new List<Lesson>();
            foreach (var m in modules)
            {
                m.Lessons = m.Lessons.OrderBy(l => l.LessonOrder).ToList();
                allLessons.AddRange(m.Lessons);
            }

            // Find current lesson
            Lesson currentLesson = null;
            if (lessonId.HasValue && lessonId.Value > 0)
            {
                currentLesson = allLessons.FirstOrDefault(l => l.LessonId == lessonId.Value);
            }

            // If not found or not specified, pick first incomplete lesson or first lesson
            var allLessonIds = allLessons.Select(al => al.LessonId).ToList();
            var completedLessonIds = allLessonIds.Count > 0
                ? new HashSet<int>(
                    _db.LessonProgresses
                        .Where(lp => lp.StudentId == studentId && lp.IsCompleted && allLessonIds.Contains(lp.LessonId))
                        .Select(lp => lp.LessonId)
                )
                : new HashSet<int>();

            if (currentLesson == null)
            {
                currentLesson = allLessons.FirstOrDefault(l => !completedLessonIds.Contains(l.LessonId)) ?? allLessons.FirstOrDefault();
            }

            Module currentModule = null;
            if (currentLesson != null)
            {
                currentModule = modules.FirstOrDefault(m => m.ModuleId == currentLesson.ModuleId);
            }

            int currentIndex = currentLesson != null ? allLessons.IndexOf(currentLesson) : -1;
            int? prevLessonId = currentIndex > 0 ? allLessons[currentIndex - 1].LessonId : (int?)null;
            int? nextLessonId = (currentIndex >= 0 && currentIndex < allLessons.Count - 1) ? allLessons[currentIndex + 1].LessonId : (int?)null;

            decimal progress = 0;
            if (allLessons.Count > 0)
            {
                progress = Math.Round((decimal)completedLessonIds.Count * 100m / allLessons.Count, 1);
            }

            bool isCourseCompleted = (allLessons.Count > 0 && completedLessonIds.Count >= allLessons.Count);
            Certificate certificate = null;
            if (isCourseCompleted)
            {
                certificate = _db.Certificates.FirstOrDefault(c => c.CourseId == courseId && c.StudentId == studentId);
            }

            return new LearningViewModel
            {
                Course = course,
                CurrentModule = currentModule,
                CurrentLesson = currentLesson,
                Modules = modules,
                CompletedLessonIds = completedLessonIds,
                ProgressPercentage = progress,
                PreviousLessonId = prevLessonId,
                NextLessonId = nextLessonId,
                IsCurrentCompleted = currentLesson != null && completedLessonIds.Contains(currentLesson.LessonId),
                IsCourseCompleted = isCourseCompleted,
                Certificate = certificate
            };
        }

        public decimal MarkLessonComplete(string studentId, int lessonId, out bool isCourseCompleted)
        {
            isCourseCompleted = false;

            var lesson = _db.Lessons.Include(l => l.Module).FirstOrDefault(l => l.LessonId == lessonId);
            if (lesson == null) return 0;

            int courseId = lesson.Module.CourseId;

            // Execute stored procedure sp_MarkLessonComplete
            try
            {
                _db.Database.ExecuteSqlCommand(
                    "EXEC sp_MarkLessonComplete @StudentId, @LessonId",
                    new SqlParameter("@StudentId", studentId),
                    new SqlParameter("@LessonId", lessonId)
                );
            }
            catch
            {
                var progressRecord = _db.LessonProgresses.FirstOrDefault(p => p.StudentId == studentId && p.LessonId == lessonId);
                if (progressRecord != null)
                {
                    progressRecord.IsCompleted = true;
                    progressRecord.CompletedDate = DateTime.UtcNow;
                    progressRecord.LastAccessedDate = DateTime.UtcNow;
                }
                else
                {
                    _db.LessonProgresses.Add(new LessonProgress
                    {
                        StudentId = studentId,
                        LessonId = lessonId,
                        IsCompleted = true,
                        CompletedDate = DateTime.UtcNow,
                        LastAccessedDate = DateTime.UtcNow
                    });
                }
                _db.SaveChanges();
            }

            // Recalculate Course Completion
            var allLessons = _db.Lessons.Where(l => l.Module.CourseId == courseId).Select(l => l.LessonId).ToList();
            int totalLessons = allLessons.Count;
            int completedCount = totalLessons > 0
                ? _db.LessonProgresses.Count(p => p.StudentId == studentId && p.IsCompleted && allLessons.Contains(p.LessonId))
                : 0;

            decimal completionPercentage = totalLessons > 0 ? Math.Round((decimal)completedCount * 100m / totalLessons, 2) : 100;

            var enrollment = _db.Enrollments.FirstOrDefault(e => e.CourseId == courseId && e.StudentId == studentId);
            if (enrollment != null)
            {
                // If course validity has expired, block completing and certificate issuance until re-enrolled
                if (enrollment.IsExpired && enrollment.Status != "Completed")
                {
                    return enrollment.CompletionPercentage;
                }

                enrollment.CompletionPercentage = completionPercentage;
                if (completionPercentage >= 100)
                {
                    enrollment.Status = "Completed";
                    enrollment.CompletionDate = DateTime.UtcNow;
                    isCourseCompleted = true;

                    // Automatically generate certificate
                    _certificateService.GenerateCertificate(studentId, courseId, 100);
                }
                _db.SaveChanges();
            }

            return completionPercentage;
        }
    }
}
