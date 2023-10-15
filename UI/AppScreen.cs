using ARMApp.Domain.Entities;
using ARMApp.UI;
using ATMApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATMApp.UI
{
    public  class AppScreen
    {
        internal const string cur = "$ ";
        internal static void Welcome()
        {
            Console.Clear();
            Console.Title = "My ATM app";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n----------------- Welcome yo MY ATM app -----------------\n");
            Console.WriteLine("Please enter your ATM card");
            Console.WriteLine("Note: Actual ATM machine will accept and validate " +
                "a physical ATM card, read the card number and validate it.");
            Utility.PressEnterToContinue();
        }

        internal static UserAccount UserLoginForm()
        {
            UserAccount tempUserAccount = new UserAccount();

            tempUserAccount.CardNumber = Validator.Convert<long>("your card number: ");
            tempUserAccount.CardPIN = Convert.ToInt32(Utility.GetSecretInput("Enter your PIN: "));

            return tempUserAccount;
        }

        internal static void LoginProgress()
        {
            Console.Write("\nCheking card number and PIN..");
            Utility.PrintDotAnimation();
        }

        internal static void PrintLockScreen()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked. Please go to the nearest Branch to unlock. Thank you.",true);
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }

        internal static void WelcomeCustomer( string fullName)
        {
            Console.WriteLine($"Welcome back, {fullName}");
            Utility.PressEnterToContinue();
        }

        internal static void DisplayAppMenu()
        {
            Console.Clear();
            Console.WriteLine("-------My ATM App Menu-------");
            Console.WriteLine(":                            :");
            Console.WriteLine("1. Account Balance           :");
            Console.WriteLine("2. Cach Deposit              :");
            Console.WriteLine("3. Withdrawal                :");
            Console.WriteLine("4. Transfer                  :");
            Console.WriteLine("5. Transaction               :");
            Console.WriteLine("6. Logout                    :");
        }

        internal static void LogoutProgress()
        {
            Console.WriteLine("Thank you for using MY ATM App");
            Utility.PrintDotAnimation();
            Console.Clear();
        }

        internal static int SelectAmount()
        {
            Console.WriteLine("");
            Console.WriteLine($":1.{cur}500      5.{cur}10,000");
            Console.WriteLine($":2.{cur}1000     6.{cur}15,000");
            Console.WriteLine($":3.{cur}2000     7.{cur}20,000");
            Console.WriteLine($":4.{cur}5000     8.{cur}40,000");
            Console.WriteLine($":0.Other");
            Console.WriteLine("");

            int selectedAmount = Validator.Convert<int>("option:");
            switch (selectedAmount)
            {
                case 1:
                    return 500; 
                case 2:
                    return 1000; 
                case 3:
                    return 2000; 
                case 4:
                    return 5000;                                                
                case 5:
                    return 10000; 
                case 6:
                    return 15000; 
                case 7:
                    return 20000; 
                case 8:
                    return 40000; 
                case 0:
                    return 0; 
                default:
                    Utility.PrintMessage("Invalid input. Try again", false);
                    SelectAmount();
                    return -1;
            }
        }

        internal InternalTransfer InternalTransferForm()
        {
            var internalTransfer = new InternalTransfer();
            internalTransfer.ReciepentBankAccountNumber = Validator.Convert<long>("recipient's card number:");
            internalTransfer.ReciepentBankAccountName = Utility.GetUserInput("recipient's name:");
            internalTransfer.TransferAmount = Validator.Convert<decimal>($"amount {cur}");

            return internalTransfer;
        }
    }
}
