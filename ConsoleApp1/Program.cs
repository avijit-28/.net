namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            //emp e1 = new emp(101, "Sonoo", 890000f);
            //emp e2 = new emp(102, "Mahesh", 490000f);
            //e1.display();
            //e2.display();



            Console.WriteLine("Enter the bank name (BankA or BankB):");
            string bankName = Console.ReadLine();

            Console.WriteLine(bankName);

            IBank bank = new BankA(bankName);

            //if (bank == null)
            //{
            //    Console.WriteLine("Invalid bank name. Please enter either 'BankA' or 'BankB'.");
            //    return;
            //}
            //else
            //{
            bank.ValidateCard();
            bank.CheckBalance();
            bank.WithdrawCash();
            bank.BankTransfer();
            //}
            
            
        }
    }
}
