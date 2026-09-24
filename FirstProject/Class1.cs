using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstProject
{
    public class Student
    {
        int id;
        string name;
        string address;
        string course;

        public Student(int id, string name, string address, string course)
        {
            this.id = id;
            this.name = name;
            this.address = address;
            this.course = course;
        }
        public int Id { 
            get { return id; } 
            set { id = value; }
        }
        public string Name { 
            get { return name; } 
            set { name = value; }
        }
        public string Course { 
            get { return course; } 
            set { course = value; }
        }
        public string Address { 
            get { return address; } 
            set { address = value; }
        }
    }
