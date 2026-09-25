using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement
{
    public class Students
    {
        public string SId { get; set; }
        public string SName { get; set; }
        public int Semester { get; set; }
        public double Cgpa { get; set; }

        //public void Display()
        //{
        //    Console.WriteLine($"Roll No : {SId}");
        //    Console.WriteLine($"Name : {SName}");
        //    Console.WriteLine($"Semester: {Semester}");
        //    Console.WriteLine($"CGPA : {Cgpa}");
        //    Console.WriteLine("------------------------");
        //}
        public Students() 
        {
        }

        public static List<Students> lists = new List<Students>();

        public Students( string SId, string SName,int Semester, double Cgpa)
        {
            this.SId = SId;
            this.SName = SName;
            this.Semester = Semester;
            this.Cgpa = Cgpa;
        }

        public static void AddStudent(Students std )
        {
            lists.Add(std);
            Console.WriteLine($"Successfully add {std.SName}");
        }

        public static void RemoveStudent(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("No id found for removing the student");
            }

            Students Roll = lists.Find(r => r.SId == id);

            if(Roll != null)
            {
                lists.Remove(Roll);
                Console.WriteLine("Student Removed");
            }
        }

        public static void StudentDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("No id found to get details of the student");
            }

            Students student = lists.Find(r => r.SId == id);

            if( student != null)
            {
                Console.WriteLine(student.SId);
                Console.WriteLine(student.SName);
                Console.WriteLine(student.Semester);
                Console.WriteLine(student.Cgpa);
            }
        }
        public static void GetEligibleStudents()
        {
            var eligibleStudnets = lists.Where(s => s.Cgpa >= 8.0 && s.Semester >= 6).OrderByDescending(s => s.Cgpa );

            foreach (var item in eligibleStudnets)
            {
                Console.WriteLine($"{item.SId}||{item.SName}||{item.Semester}||{item.Cgpa} ");
            }
        }

        public static void BatchStatistics()
        {
            if(lists.Count == 0)
            {
                Console.WriteLine("No Students Available");
                return;
            }
            double totalCgpa = 0;
            foreach (var item in lists)
            {
                totalCgpa += item.Cgpa;
            }

            double averageCgpa = totalCgpa / lists.Count;

            Console.WriteLine("\n----- Batch Statistics -----");
            Console.WriteLine($"Total Enrolled Students : {lists.Count}");
            Console.WriteLine($"Average CGPA : {averageCgpa:F2}");
            Console.WriteLine($"Highest CGPA : {lists.Max(s => s.Cgpa)}");
            Console.WriteLine($"Lowest CGPA : {lists.Min(s => s.Cgpa)}");
        }


    }
}
