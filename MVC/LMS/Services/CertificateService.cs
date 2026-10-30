using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LMS.Data;
using LMS.Models;
using LMS.ViewModels;

namespace LMS.Services
{
    public class CertificateService
    {
        private readonly ApplicationDbContext _db;

        public CertificateService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Certificate GenerateCertificate(string studentId, int courseId, decimal finalScore = 100)
        {
            var existing = _db.Certificates.FirstOrDefault(c => c.StudentId == studentId && c.CourseId == courseId);
            if (existing != null)
            {
                return existing;
            }

            // Generate unique Certificate Number
            string certNumber = $"LMS-{DateTime.UtcNow.Year}-{new Random().Next(100000, 999999)}";
            while (_db.Certificates.Any(c => c.CertificateNumber == certNumber))
            {
                certNumber = $"LMS-{DateTime.UtcNow.Year}-{new Random().Next(100000, 999999)}";
            }

            var certificate = new Certificate
            {
                CertificateNumber = certNumber,
                StudentId = studentId,
                CourseId = courseId,
                FinalScore = finalScore,
                IssueDate = DateTime.UtcNow
            };

            _db.Certificates.Add(certificate);

            // Also create a notification for the student
            var course = _db.Courses.Find(courseId);
            string courseTitle = course != null ? course.Title : "Course";
            _db.Notifications.Add(new Notification
            {
                UserId = studentId,
                Title = "Certificate Issued!",
                Message = $"Congratulations! You have completed '{courseTitle}' and your certificate #{certNumber} is now ready.",
                NotificationType = "Certificate",
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            });

            _db.SaveChanges();
            return certificate;
        }

        public CertificateViewModel GetCertificateViewModel(int certificateId)
        {
            var cert = _db.Certificates
                .Include(c => c.Course)
                .FirstOrDefault(c => c.CertificateId == certificateId);

            if (cert == null) return null;

            var user = _db.Users.Find(cert.StudentId);
            string studentName = user != null && !string.IsNullOrEmpty(user.FullName) ? user.FullName : (user != null ? user.UserName : "Student");

            return new CertificateViewModel
            {
                CertificateId = cert.CertificateId,
                CertificateNumber = cert.CertificateNumber,
                StudentName = studentName,
                CourseTitle = cert.Course != null ? cert.Course.Title : "Course",
                IssueDate = cert.IssueDate,
                FinalScore = cert.FinalScore
            };
        }

        public List<Certificate> GetStudentCertificates(string studentId)
        {
            return _db.Certificates
                .Include(c => c.Course)
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.IssueDate)
                .ToList();
        }

        public Certificate GetCertificateByNumber(string certNumber)
        {
            return _db.Certificates
                .Include(c => c.Course)
                .FirstOrDefault(c => c.CertificateNumber == certNumber);
        }
    }
}
