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
    public class QuizService
    {
        private readonly ApplicationDbContext _db;

        public QuizService(ApplicationDbContext db)
        {
            _db = db;
        }

        public QuizAttempt StartAttempt(int quizId, string studentId)
        {
            var quiz = _db.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.QuizId == quizId && q.IsActive);

            if (quiz == null) return null;

            int totalMarks = quiz.Questions.Sum(q => q.Marks);

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                StudentId = studentId,
                TotalMarks = totalMarks > 0 ? totalMarks : quiz.TotalMarks,
                Score = 0,
                ObtainedMarks = 0,
                IsPassed = false,
                StartedAt = DateTime.UtcNow
            };

            _db.QuizAttempts.Add(attempt);
            _db.SaveChanges();
            return attempt;
        }

        public QuizAttemptViewModel GetQuizAttemptViewModel(int attemptId, string studentId)
        {
            var attempt = _db.QuizAttempts
                .Include(a => a.Quiz.Questions.Select(q => q.Options))
                .FirstOrDefault(a => a.AttemptId == attemptId && a.StudentId == studentId);

            if (attempt == null || attempt.CompletedAt != null) return null;

            var questions = attempt.Quiz.Questions.OrderBy(q => q.QuestionOrder).ToList();
            foreach (var q in questions)
            {
                q.Options = q.Options.OrderBy(o => o.OptionOrder).ToList();
            }

            return new QuizAttemptViewModel
            {
                Quiz = attempt.Quiz,
                AttemptId = attempt.AttemptId,
                Questions = questions,
                TimeLimitMinutes = attempt.Quiz.TimeLimitMinutes
            };
        }

        public QuizResultViewModel SubmitQuiz(int attemptId, List<QuizAnswerInput> answers, string studentId)
        {
            var attempt = _db.QuizAttempts
                .Include(a => a.Quiz.Course)
                .Include(a => a.Quiz.Questions.Select(q => q.Options))
                .FirstOrDefault(a => a.AttemptId == attemptId && a.StudentId == studentId);

            if (attempt == null) return null;

            int obtainedMarks = 0;
            int totalMarks = 0;

            foreach (var q in attempt.Quiz.Questions)
            {
                totalMarks += q.Marks;
                var submitted = answers?.FirstOrDefault(a => a.QuestionId == q.QuestionId);
                var selectedOption = q.Options.FirstOrDefault(o => submitted != null && o.OptionId == submitted.SelectedOptionId);

                bool isCorrect = selectedOption != null && selectedOption.IsCorrect;
                int marks = isCorrect ? q.Marks : 0;
                obtainedMarks += marks;

                var existingAnswer = _db.QuizAnswers.FirstOrDefault(qa => qa.AttemptId == attemptId && qa.QuestionId == q.QuestionId);
                if (existingAnswer == null)
                {
                    _db.QuizAnswers.Add(new QuizAnswer
                    {
                        AttemptId = attemptId,
                        QuestionId = q.QuestionId,
                        SelectedOptionId = selectedOption?.OptionId,
                        IsCorrect = isCorrect,
                        MarksObtained = marks
                    });
                }
                else
                {
                    existingAnswer.SelectedOptionId = selectedOption?.OptionId;
                    existingAnswer.IsCorrect = isCorrect;
                    existingAnswer.MarksObtained = marks;
                }
            }

            decimal score = totalMarks > 0 ? Math.Round((decimal)obtainedMarks * 100m / totalMarks, 2) : 0;
            bool isPassed = score >= attempt.Quiz.PassingPercentage;

            attempt.TotalMarks = totalMarks;
            attempt.ObtainedMarks = obtainedMarks;
            attempt.Score = score;
            attempt.IsPassed = isPassed;
            attempt.CompletedAt = DateTime.UtcNow;

            // Add notification
            _db.Notifications.Add(new Notification
            {
                UserId = studentId,
                Title = isPassed ? "Quiz Passed!" : "Quiz Completed",
                Message = $"You scored {score}% ({obtainedMarks}/{totalMarks}) on '{attempt.Quiz.Title}'. Result: {(isPassed ? "PASSED" : "FAILED")}",
                NotificationType = "Quiz",
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            });

            _db.SaveChanges();

            return new QuizResultViewModel
            {
                AttemptId = attempt.AttemptId,
                QuizTitle = attempt.Quiz.Title,
                CourseTitle = attempt.Quiz.Course != null ? attempt.Quiz.Course.Title : "Course",
                CourseId = attempt.Quiz.CourseId,
                Score = score,
                TotalMarks = totalMarks,
                ObtainedMarks = obtainedMarks,
                IsPassed = isPassed,
                CompletedAt = attempt.CompletedAt
            };
        }

        public List<QuizAttempt> GetStudentQuizHistory(string studentId)
        {
            return _db.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.Quiz.Course)
                .Where(a => a.StudentId == studentId && a.CompletedAt != null)
                .OrderByDescending(a => a.CompletedAt)
                .ToList();
        }
    }
}
