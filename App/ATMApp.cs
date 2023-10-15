using ARMApp.Domain.Entities;
using ARMApp.Domain.Enums;
using ARMApp.Domain.Interfaces;
using ARMApp.UI;
using ATMApp.Domain.Entities;
using ATMApp.Domain.Enums;
using ATMApp.Domain.Interfaces;
using ATMApp.UI;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATMApp
{
    internal class ATMApp : IUserLogin, IUserAccountActions, ITransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppScreen screen;

        public ATMApp()
        {
           screen = new AppScreen();
        }

        public void Run()
        {
            AppScreen.Welcome();
            CheckUsercardNumberAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while (true)
            {
                AppScreen.DisplayAppMenu();
                ProcessMenuOption();
            }
            
        }
        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount{Id = 1 , FullName = "Gunel Qahramani" ,AccountNumber = 111111, CardNumber = 123123 , CardPIN = 321321 , AccountBalance = 5000.000m ,IsLocked =false},
                new UserAccount{Id = 2 , FullName = "Samira Kamilova" ,AccountNumber = 222222, CardNumber = 456456 , CardPIN = 654654 , AccountBalance = 500.000m ,IsLocked =false},
                new UserAccount{Id = 3 , FullName = "Sinara Qulieva" ,AccountNumber = 333333, CardNumber = 789789 , CardPIN = 987987, AccountBalance = 15000.000m ,IsLocked =true},
            };
            _listOfTransactions = new List<Transaction>();
        }

        public void CheckUsercardNumberAndPassword()
        {
           
            bool isCorrectLogin = false;
            while(isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach (var account in userAccountList)
                {
                    selectedAccount = account;
                    if(inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        inputAccount.TotalLogin++;
                        if(inputAccount.CardPIN.Equals(selectedAccount.CardPIN))
                        {
                            selectedAccount = account;
                            if(selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                inputAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                    var lastElement = userAccountList.Last();
                    if(isCorrectLogin == false && account == lastElement)
                    {
                        Utility.PrintMessage("Invalid card number or PIN", true);
                        selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                        if(selectedAccount.IsLocked)
                            AppScreen.PrintLockScreen();
                        Console.Clear();
                    }
                }
            }
            
        }
        private void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("an option:"))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();  
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    MakeWithdrawal();
                    break;
                case (int)AppMenu.Internaltransfer:
                    var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogoutProgress();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("You have succesfully logged out. Please collect your ATM card.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Utility.PressEnterToContinue();
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid option..", false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("\nOnly multiples of 500 and 1000 dollars allowed.\n");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");

            //simulate counting
            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            //some gaurd clause
            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount should be greater tjan zero. Try again", false);
                return;
            }
            if(transaction_amt % 500 != 0)
            {
                Utility.PrintMessage("Enter deposit amount in multiples of 500 or 1000. Tray again.", false);
                return;
            }
            if(PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage("You have cancelled your action.", false);
                return;
            }

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt,"");

            //update account balance
            selectedAccount.AccountBalance += transaction_amt;

            //print succes message
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was succesful", true);

        }

        public void MakeWithdrawal()
        {
            var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectAmount();
            
            if(selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            } 

            //input validation
            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs be greater than zero. Try again.", false);
                return;
            }
            if(transaction_amt % 500 != 0)
            {
                Utility.PrintMessage("You can only withdraw amount in multiple of 500 or 1000. Try again", false);
                return;
            }    

            //business logic validations
            if(transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. Your balance too low to withdraw " +
                    $"{Utility.FormatAmount(transaction_amt)}" ,false);
                return;
            }
            if((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum " +
                    $"{Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }
            //bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, -transaction_amt, "");
            //update account balance
            selectedAccount.AccountBalance -= transaction_amt;
            //success message
            Utility.PrintMessage($"You have succesfully withdrawal " +
                $"{Utility.FormatAmount(transaction_amt)}", true);
        }

        private bool PreviewBankNotesCount( int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fivethousandNotesCount = (amount % 1000) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("--------");
            Console.WriteLine($"{AppScreen.cur}1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 X {fivethousandNotesCount} = {500 * fivethousandNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);

        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _transactionType, decimal _transactionAmount, string _desc)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _transactionType,
                TransactionAmount = _transactionAmount,
                Description = _desc
            };
            //add transaction object to the list
            _listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            var filteredtransactionlist = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
            //check if there is a transaction
            if(filteredtransactionlist.Count <= 0 )
            {
                Utility.PrintMessage("you have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id","Transaction Date","Type","Descriptions","Amount" + AppScreen.cur);
                foreach (var tran in filteredtransactionlist)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredtransactionlist.Count} transaction(s)",true);
            }
        }

        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
            if(internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs be more than zero. try again.", false);
                return;
            }
            //check sender's account balance
            if(internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed. You do not have enough balance to transfer " +
                    $"{Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }
            //check the minimum kept amount
            if((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer failed. Your account needs to have minimum " +
                    $"{Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }
            //check if reciever's account number is valid
            var selectedBankAccountReciever = (from userAcc in userAccountList
                                               where userAcc.CardNumber == internalTransfer.ReciepentBankAccountNumber
                                               select userAcc).FirstOrDefault();
            if(selectedBankAccountReciever == null)
            {
                Utility.PrintMessage("Transfer failed. Reciever bank account number is invalid.", false);
                return;
            }
            //check reciever's name
            if(selectedBankAccountReciever.FullName != internalTransfer.ReciepentBankAccountName)
            {
                Utility.PrintMessage("Transfer failed. Recipient's bank account name does not match.", false);
                return;
            }

            if (selectedBankAccountReciever.IsLocked)
            {
                Utility.PrintMessage($"Transfer failed. {selectedBankAccountReciever.FullName}'s account is locked.", false);
                return;
            }
            //add transaction to transactions record-sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, -internalTransfer.TransferAmount, "Transfered " +
                $"to {selectedBankAccountReciever.CardNumber} ({selectedBankAccountReciever.FullName})");
            
            //update sender's account balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //add transaction record-reciever
            InsertTransaction(selectedBankAccountReciever.Id, TransactionType.Transfer, internalTransfer.TransferAmount,
               $"Transfered from {selectedAccount.AccountNumber} ({selectedAccount.FullName})");

            //update reciever's account balance
            selectedBankAccountReciever.AccountBalance += internalTransfer.TransferAmount;

            //print succes message
            Utility.PrintMessage($"You have succesfully transfered {Utility.FormatAmount(internalTransfer.TransferAmount)} " +
                $"to {internalTransfer.ReciepentBankAccountName}",true);

        }
    }
}
