using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstMVCProject.Models
{
    public class Employee
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int Salary { get; set; }

        public string Gender { get; set; }

        public string City { get; set; }

        public string Dept { get; set; }
    }
}