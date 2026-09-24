using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    abstract class Loan
    {
        public void VerifyDocumentation()
        {
            Console.WriteLine("Verifying documentation for the loan.");
        }
        public void SanctionLoan()
        {
            Console.WriteLine("Sanctioning the loan.");
        }

        public abstract double CalculateInterest();
        public abstract bool CheckEligibility();


    }
    class HomeLoan : Loan
    {
        public override double CalculateInterest()
        {
            Console.WriteLine("Calculating interest for home loan.");
            return 8;
        }
        public override bool CheckEligibility()
        {
            Console.WriteLine("Checking eligibility for home loan.");
            return true; // Example eligibility check
        }
    }
    class CarLoan : Loan
    {
        public override double CalculateInterest()
        {
            Console.WriteLine("Calculating interest for car loan.");
            return 9;
        }
        public override bool CheckEligibility()
        {
            Console.WriteLine("Checking eligibility for car loan.");
            return true; // Example eligibility check
        }
    }
    class EducationLoan : Loan
    {
        public override double CalculateInterest()
        {
            Console.WriteLine("Calculating interest for education loan.");
            return 7;
        }
        public override bool CheckEligibility()
        {
            Console.WriteLine("Checking eligibility for education loan.");
            return true; // Example eligibility check
        }
    }
}
