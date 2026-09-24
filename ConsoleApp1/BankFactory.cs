using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    public interface IBank
    {
        void ValidateCard();
        void CheckBalance();
        void WithdrawCash();
        void BankTransfer();
    }
    //public class BankFactory
    //{

    //    public static IBank GetBankObject(string bankName)
    //    {
    //        IBank bank = null;
    //        if (bankName == "BankA")
    //        {
    //            bank = new BankA();
    //        }
    //        else if (bankName != "BankA")
    //        {
    //            //bank = new BankB();
    //            Console.WriteLine("entered bank name is not valid");
    //        }

    //        return bank;
    //    }
    //}

    public class BankA : IBank
    {   string name;
        public BankA(string name)
        {
               this.name = name;
            Console.WriteLine("BankA object created with name: " + name);
        }
        public void ValidateCard()
        {
            Console.WriteLine("Validating card for Bank A");
        }
        public void CheckBalance()
        {
            Console.WriteLine("Checking balance for Bank A");
        }
        public void WithdrawCash()
        {
            Console.WriteLine("Withdrawing cash from Bank A");
        }
        public void BankTransfer()
        {
            Console.WriteLine("Transferring money from Bank A");
        }
    }
    //public class BankB : IBank 
    //{ 
    //    public void ValidateCard()
    //    {
    //        Console.WriteLine("Validating card for Bank B");
    //    }
    //    public void CheckBalance()
    //    {
    //        Console.WriteLine("Checking balance for Bank B");
    //    }
    //    public void WithdrawCash()
    //    {
    //        Console.WriteLine("Withdrawing cash from Bank B");
    //    }
    //    public void BankTransfer()
    //    {
    //        Console.WriteLine("Transferring money from Bank B");
    //    }
    //}

}
