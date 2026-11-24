using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using assignment1.AllAbstract;

namespace assignment1
{
    

    class HomeLoan : Loan
    {
        public HomeLoan(string applicantName, double loanAmount) 
            : base(applicantName, loanAmount){}

        public override bool CheckEligibility()
        {
            Console.WriteLine("Checking eligibility for home loan.");
            return true; // Example eligibility check
        }

        public override double CalculateInterestRate()
        {
            Console.WriteLine("Checking Home Loan: Verifying property registry...");
            return 8;
        }
        
    }
    class CarLoan : Loan
    {
        public CarLoan(string applicantName, double loanAmount) 
            : base(applicantName, loanAmount) { }

        public override bool CheckEligibility()
        {
            Console.WriteLine("Checking Car Loan: Verifying dealer quotation and minimum income criteria...");
            return true; // Example eligibility check
        }

        public override double CalculateInterestRate()
        {
            Console.WriteLine("Calculating interest for car loan.");
            return 9.2;
        }
        
    }
    class EducationLoan : Loan
    {
        public EducationLoan(string applicantName, double loanAmount) 
            : base(applicantName, loanAmount) { }

        public override bool CheckEligibility()
        {
            Console.WriteLine("Checking Education Loan: Verifying college admission letter and co-applicant details...");
            return true; // Example eligibility check
        }

        public override double CalculateInterestRate()
        {
            Console.WriteLine("Calculating interest for education loan.");
            return 7;
        }
        
    }
}
