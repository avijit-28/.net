using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using LMS.Data;
using LMS.Models;

namespace LMS.Services
{
    public class EnrollmentService
    {
        private readonly ApplicationDbContext _db;

        public EnrollmentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool IsEnrolled(string studentId, int courseId)
        {
            return _db.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId && e.Status != "Cancelled");
        }

        public bool EnrollStudent(string studentId, int courseId, out string errorMessage)
        {
            errorMessage = string.Empty;

            var existing = _db.Enrollments.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
            if (existing != null)
            {
                if (existing.Status != "Cancelled")
                {
                    errorMessage = "You are already enrolled in this course.";
                    return false;
                }
                else
                {
                    existing.Status = "Active";
                    existing.EnrollmentDate = DateTime.UtcNow;
                    _db.SaveChanges();
                    return true;
                }
            }

            try
            {
                // Execute stored procedure sp_EnrollStudent
                _db.Database.ExecuteSqlCommand(
                    "EXEC sp_EnrollStudent @StudentId, @CourseId",
                    new SqlParameter("@StudentId", studentId),
                    new SqlParameter("@CourseId", courseId)
                );
                return true;
            }
            catch (Exception)
            {
                // Fallback to direct EF save
                try
                {
                    var course = _db.Courses.Find(courseId);
                    if (course == null)
                    {
                        errorMessage = "Course not found.";
                        return false;
                    }

                    var enrollment = new Enrollment
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        Status = "Active",
                        EnrollmentDate = DateTime.UtcNow,
                        CompletionPercentage = 0
                    };
                    _db.Enrollments.Add(enrollment);
                    course.TotalStudents += 1;
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception inner)
                {
                    errorMessage = inner.Message;
                    return false;
                }
            }
        }

        public List<Enrollment> GetStudentEnrollments(string studentId)
        {
            return _db.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Category)
                .Where(e => e.StudentId == studentId && e.Status != "Cancelled")
                .OrderByDescending(e => e.EnrollmentDate)
                .ToList();
        }
    }
}
