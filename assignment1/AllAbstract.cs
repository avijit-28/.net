using System;
namespace assignment1
{
    // Loan abstract class
    public abstract class Loan
    {
        public string ApplicantName { get; set; }
        public double LoanAmount { get; set; }
        public Loan(string applicantName, double loanAmount)
        {
            ApplicantName = applicantName;
            LoanAmount = loanAmount;
        }
        public void VerifyDocumentation()
        {
            Console.WriteLine($"Verifying KYC and identity documents for {ApplicantName}...");
        }
        public void SanctionLoan()
        {
            Console.WriteLine($"SUCCESS: Loan of ${LoanAmount} sanctioned for {ApplicantName}!");
        }

        public abstract virtual double CalculateInterestRate();
        public abstract virtual bool CheckEligibility();

        public void ProcessLoan()
        {
            Console.WriteLine($"\n--- Processing Loan for {ApplicantName} (${LoanAmount}) ---");
            VerifyDocumentation();
            if (CheckEligibility())
            {
                double interestRate = CalculateInterestRate();
                Console.WriteLine($"Eligibility approved! Applied Interest Rate: {interestRate}%");
                SanctionLoan();
            }
            else
            {
                Console.WriteLine("REJECTED: Eligibility criteria not satisfied.");
            }
        }

    }

    // BaseRobot abstract class
    public abstract class BaseRobot
    {
        public string Name { get; set; }
        public BaseRobot(string name)
        {
            Name = name;
        }
        public void Start()
        {
            Console.WriteLine($"Robot {Name} is starting.");
        }
        // Must be implemented by specific robot types
        public abstract void PerformTask();
    }

}

   