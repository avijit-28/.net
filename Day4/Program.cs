namespace Day4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //PartialClass emp1 = new PartialClass();
            //emp1.FirstName = "Avijit";
            //emp1.LastName = "Pakhira";
            //emp1.Salary = 15000;
            //emp1.Gender = "Male";
            //emp1.DisplayName();
            //emp1.EmployeeActive(false);
            //emp1.DisplayDetails();


            Employee emp = new Employee();
            //emp.Eid = 101;
            //emp.Ename = "Avijit";
            //emp.Eaddress = "New Town";
            //emp.Eage = 23;
            
            emp.GetEmployeeDetails();
            emp.DisplayDetails();
            Console.ReadLine();


        }
    }
}
