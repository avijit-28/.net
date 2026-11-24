using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using assignment1.AllAbstract;

namespace assignment1
{
    public void ProcessLoan()
    {
        Console.WriteLine($"\n--- Processing Loan for {ApplicantName} (${LoanAmount}) ---");
        VerifyDocumentation();
        if (CheckEligibility())
        {
            double interestRate = CalculateInterestRate();
            Console.WriteLine($" Eligibility approved! Applied Interest Rate: {rate}%");
            SanctionLoan();
        }
        else
        {
            Console.WriteLine(" REJECTED: Eligibility criteria not satisfied.");
        }
    }

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
            Console.WriteLine("Checking Car Loan: Verifying dealer quotation and minimum income criteria...);
            return true; // Example eligibility check
        }

        public override double CalculateInterest()
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

        public override double CalculateInterest()
        {
            Console.WriteLine("Calculating interest for education loan.");
            return 7;
        }
        
    }
}
