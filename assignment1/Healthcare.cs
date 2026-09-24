using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    class Healthcare
    {
        private string medicalHistory;
        private double temperature;
        private bool billPaid;
        public int PatientId { get; private set; }
        public string PatientName { get; set; }

        public Healthcare(int id, string name)
        {
            PatientId = id;
            PatientName = name;
        }

        public string GetMedicalHistory()
        {
            return medicalHistory;
        }

        public void SetMedicalHistory(string history)
        {
            if (!string.IsNullOrEmpty(history))
            {
                medicalHistory = history;
            }
            else
            {
                Console.WriteLine("Medical history cannot be empty.");
            }
        }

        public double GetTemperature()
        {
            return temperature;
        }

        public void SetTemperature(double temp)
        {
            if (temp >= 35.0 && temp <= 42.0)
            {
                temperature = temp;
            }
            else
            {
                Console.WriteLine("Temperature must be between 35.0 and 42.0 degrees Celsius.");
            }
        }

        public bool BillPaid
        {
            get { return billPaid; } 
            set { billPaid = value; }
        }
        public void payBill()
        {
            if (billPaid)
            {
                Console.WriteLine("Bill has already been paid.");
            }
            else
            {
                billPaid = true;
                Console.WriteLine("Bill payment successful.");
            }
        }





    }
}
