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
                else
                {
                    Console.WriteLine("Invalid card number. It must be 16 digits.");

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
                    Console.WriteLine("Bank API Security Token configured successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid security token.");
                }
            }
            get
            {
                return security_token;
            }
        }
        public void ProcessTransaction(decimal amount)
        {
            Console.WriteLine($"\n--- Processing Payment of ${amount} ---");
            // Verify prerequisites using encapsulated state
            if (string.IsNullOrEmpty(security_token))
            {
                Console.WriteLine("Transaction Failed: Bank API token is missing!");
                return;
            }
            if (string.IsNullOrEmpty(card_number))
            {
                Console.WriteLine("Transaction Failed: No card registered!");
                return;
            }
            if (amount <= 0)
            {
                Console.WriteLine("Transaction Failed: Amount must be greater than ₹0.");
                return;
            }
            if (amount > account_balance)
            {
                Console.WriteLine($"Transaction Declined: Insufficient balance. Available: ${account_balance:N2}");
                return;
            }
            // Deduct safely
            account_balance -= amount;
            Console.WriteLine($"SUCCESS: ${amount} debited from Card {CardNumber}.");
            Console.WriteLine($"Remaining Balance: ${account_balance}");
        }
    }
    
} 
