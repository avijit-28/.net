
using StudentManagement;

while (true)
{
    Console.WriteLine("\n===== STUDENT MANAGEMENT SYSTEM =====");
    Console.WriteLine("1. Add Student");
    Console.WriteLine("2. Remove Student");
    Console.WriteLine("3. Search Student");
    Console.WriteLine("4. Placement Eligible Students");
    Console.WriteLine("5. Batch Statistics");
    Console.WriteLine("6. Exit");

    Console.Write("Enter Your Choice: ");
    int choice = Convert.ToInt32(Console.ReadLine());

    Students stdt = new Students();

    switch (choice)
    {
        case 1:
            Console.WriteLine("Enter Student details");
            Console.WriteLine("Enter Roll Number: ");
            stdt.SId = Console.ReadLine();

            Console.WriteLine("Enter Student Name: ");
            stdt.SName = Console.ReadLine();

            Console.WriteLine("Enter Semester: ");
            stdt.Semester = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter CGPA: ");
            stdt.Cgpa = Convert.ToDouble(Console.ReadLine());

            Students.AddStudent(stdt);

            Console.WriteLine("Student Added Successfully!");

            break;

        case 2:
            Console.WriteLine("Enter Roll Number: ");
            stdt.SId = Console.ReadLine();
            Students.RemoveStudent(stdt.SId);
            break;

        case 3:
            Console.WriteLine("Enter Roll Number: ");
            stdt.SId = Console.ReadLine();
            Students.StudentDetails(stdt.SId);
            break;

        case 4:
            Students.GetEligibleStudents();
            break;

        case 5:
            Students.BatchStatistics();
            break;

        case 6:
            Console.WriteLine("Thank You...");
            Environment.Exit(0);
            break;

        default:
            Console.WriteLine("Invalid Choice!");
            break;
    }
}
