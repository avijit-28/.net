 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    public class PaymentProcessor
    {
        private string card_number;
        private decimal account_balance;
        private string security_token;

        public string CardNumber
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 16)
                {
                    card_number = value;
                }
            }
            get
            {
                return "************" + card_number.Substring(card_number.Length - 4);
            }
        }

        public decimal AccountBalance
        {
            get
            {
                return account_balance;
            }

            set
            {
                if (value >= 0)
                {
                    account_balance = value;
                }
                else
                {
                    Console.WriteLine("Balance cannot be negative.");
                }
            }
        }
        public string SecurityToken
        {
                        set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    security_token = value;
                }
            }
            get
            {
                return security_token;
            }
        }

    }
    
} 
