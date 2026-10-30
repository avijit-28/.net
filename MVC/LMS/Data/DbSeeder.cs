using System;
using System.Linq;
using LMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Configure user validator
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Seed Roles
            string[] roles = new[] { "Admin", "Instructor", "Student" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@lms.com";
            var adminUser = userManager.FindByEmail(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };
                var result = userManager.Create(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }

            // Seed Instructor User
            var instructorEmail = "instructor@lms.com";
            var instructorUser = userManager.FindByEmail(instructorEmail);
            if (instructorUser == null)
            {
                instructorUser = new ApplicationUser
                {
                    UserName = instructorEmail,
                    Email = instructorEmail,
                    FullName = "Dr. Sarah Connor",
                    Bio = "Senior Software Architect & Data Science Consultant",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };
                var result = userManager.Create(instructorUser, "Instructor@123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(instructorUser.Id, "Instructor");
                }
            }

            // Seed Student User
            var studentEmail = "student@lms.com";
            var studentUser = userManager.FindByEmail(studentEmail);
            if (studentUser == null)
            {
                studentUser = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FullName = "Avijit Pakhira",
                    Bio = "Passionate learner and software developer",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };
                var result = userManager.Create(studentUser, "Student@123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(studentUser.Id, "Student");
                }
            }

            // Update sample courses with 'INSTRUCTOR-001' to point to instructorUser.Id if found
            if (instructorUser != null)
            {
                var sampleCourses = context.Courses.Where(c => c.InstructorId == "INSTRUCTOR-001").ToList();
                if (sampleCourses.Any())
                {
                    foreach (var c in sampleCourses)
                    {
                        c.InstructorId = instructorUser.Id;
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
