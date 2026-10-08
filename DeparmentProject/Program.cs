using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeparmentProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DepartmentRepo Repo = new DepartmentRepo();
            //Repo.getallDepartmentByProcedure();
            //Repo.insertDepartmentByProcedure();
            //Repo.deleteDepartmentByProcedure();
            //Repo.updateDepartmentByProcedure();
            Repo.getDepartmentDetails(2);    
        }
    }
}
