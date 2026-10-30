using System;
using System.Linq;
using LMS.Data;
using LMS.Models;

namespace LMS.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _db;
        private readonly EnrollmentService _enrollmentService;

        public PaymentService(ApplicationDbContext db)
        {
            _db = db;
            _enrollmentService = new EnrollmentService(db);
        }

        public bool ProcessCoursePayment(string studentId, int courseId, string paymentMethod, out string transactionId, out string errorMessage)
        {
            errorMessage = string.Empty;
            transactionId = "TXN-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            var course = _db.Courses.Find(courseId);
            if (course == null)
            {
                errorMessage = "Course not found.";
                return false;
            }

            if (_enrollmentService.IsEnrolled(studentId, courseId))
            {
                errorMessage = "You are already enrolled in this course.";
                return false;
            }

            var payment = new Payment
            {
                StudentId = studentId,
                CourseId = courseId,
                Amount = course.Price,
                PaymentMethod = paymentMethod ?? "Credit Card (Mock/Test)",
                TransactionId = transactionId,
                PaymentStatus = "Completed",
                PaymentDate = DateTime.UtcNow
            };

            _db.Payments.Add(payment);
            _db.SaveChanges();

            // Enroll student
            bool enrolled = _enrollmentService.EnrollStudent(studentId, courseId, out string enrollError);
            if (enrolled)
            {
                var enrollment = _db.Enrollments.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
                if (enrollment != null)
                {
                    payment.EnrollmentId = enrollment.EnrollmentId;
                    _db.SaveChanges();
                }

                _db.Notifications.Add(new Notification
                {
                    UserId = studentId,
                    Title = "Payment Successful",
                    Message = $"Your payment of ${course.Price:N2} for '{course.Title}' was successful. You now have full access!",
                    NotificationType = "Payment",
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow
                });
                _db.SaveChanges();
                return true;
            }
            else
            {
                errorMessage = enrollError;
                return false;
            }
        }
    }
}
