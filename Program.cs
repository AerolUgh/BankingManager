using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankingOOPpractice
{
    class CustomerAccount
    {
        private string _Name;
        private double _Balance;
        protected List<string> transactionHistory = new List<string>();

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public double Balance
        {
            get { return _Balance; }
            set { _Balance = value; }
        }

        public CustomerAccount(string name, double balance)
        {
            Name = name;
            Balance = balance;
        }

        public void Deposit(double amount)
        {
            Balance += amount;
            transactionHistory.Add($"{Name} deposited {amount}, New balance: {Balance}");
            Console.WriteLine($"{Name} deposited {amount}, New balance: {Balance}");
        }

        public virtual void Withdraw(double amount)
        {
            if (amount <= Balance)
            {
                Balance -= amount;
                transactionHistory.Add($"{Name} withdrew {amount}, New balance: {Balance}");
                Console.WriteLine($"{Name} withdrew {amount}, New balance: {Balance}");
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
            }
        }

        public double GetBalance()
        {
            Console.WriteLine("New Balance: " + Balance);
            return Balance;
        }

        public void ShowTransactionHistory()
        {
            Console.WriteLine($"Transaction History for {Name}: ");
            foreach (var transaction in transactionHistory)
            {
                Console.WriteLine(transaction);
            }
        }
    }

    class SavingsAccount : CustomerAccount
    {
        private double interestRate;

        public SavingsAccount(string Name, double Balance, double interest) : base(Name, Balance)
        {
            interestRate = interest;
        }

        public void ApplyInterest()
        {
            double interest = Balance * interestRate;
            Deposit(interest);
            Console.WriteLine($"{Name} earned interest, New balance: {Balance}");
        }
    }

    class CurrentAccount : CustomerAccount
    {
        private double overDraftLimit;

        public CurrentAccount(string Name, double Balance, double overdraft) : base(Name, Balance)
        {
            overDraftLimit = overdraft;
        }

        public override void Withdraw(double amount)
        {
            if (amount <= Balance + overDraftLimit)
            {
                Balance -= amount;
                transactionHistory.Add($"{Name} withdrew {amount}, New balance: {Balance}");
                Console.WriteLine($"{Name} withdrew {amount}, New balance: {Balance}");
            }
            else
            {
                Console.WriteLine("Overdraft limit exceeded.");
            }
        }
    }

    internal class Program
    {
        static Dictionary<string, CustomerAccount> Accounts = new Dictionary<string, CustomerAccount>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Banking System ---");
                Console.WriteLine("[1] Create Account");
                Console.WriteLine("[2] Transfer Funds");
                Console.WriteLine("[3] Check Balance");
                Console.WriteLine("[4] Deposit");
                Console.WriteLine("[5] Withdraw");
                Console.WriteLine("[6] Open Savings");
                Console.WriteLine("[7] Open Overdraft");
                Console.WriteLine("[8] Show Transactions");
                Console.WriteLine("[9] Exit");
                Console.Write("Choose: ");

                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        CreateAccount();
                        break;
                    case 2:
                        TransferFunds();
                        break;
                    case 3:
                        CheckBalance();
                        break;
                    case 4:
                        Deposit();
                        break;
                    case 5:
                        Withdraw();
                        break;
                    case 6:
                        OpenSavings();
                        break;
                    case 7:
                        OpenOverdraft();
                        break;
                    case 8:
                        ShowTransactions();
                        break;
                    case 9:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }

        static void CreateAccount()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            Console.Write("Enter the initial deposit amount: ");
            double balance = Convert.ToDouble(Console.ReadLine());

            Accounts[name] = new CustomerAccount(name, balance);
            Console.WriteLine($"Account for {name} created successfully!");
        }

        static void TransferFunds()
        {
            Console.Write("Enter your name: ");
            string sender = Console.ReadLine();

            if (!Accounts.ContainsKey(sender))
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter recipient name: ");
            string receiver = Console.ReadLine();

            if (!Accounts.ContainsKey(receiver) || sender == receiver)
            {
                Console.WriteLine("Invalid recipient.");
                return;
            }

            Console.Write("Enter amount to transfer: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            if (Accounts[sender].Balance >= amount)
            {
                Accounts[sender].Withdraw(amount);
                Accounts[receiver].Deposit(amount);
                Console.WriteLine($"Transferred {amount} from {sender} to {receiver}.");
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
            }
        }

        static void CheckBalance()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            if (Accounts.ContainsKey(name))
                Accounts[name].GetBalance();
            else
                Console.WriteLine("Account not found.");
        }

        static void Deposit()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            if (!Accounts.ContainsKey(name))
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter amount to deposit: ");
            double amount = Convert.ToDouble(Console.ReadLine());
            Accounts[name].Deposit(amount);
        }

        static void Withdraw()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            if (!Accounts.ContainsKey(name))
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter amount to withdraw: ");
            double amount = Convert.ToDouble(Console.ReadLine());
            Accounts[name].Withdraw(amount);
        }

        static void OpenSavings()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            if (!Accounts.ContainsKey(name))
            {
                Console.WriteLine("Account not found.");
                return;
            }

            SavingsAccount savings = new SavingsAccount(name, Accounts[name].Balance, 0.05);
            savings.ApplyInterest();
            Accounts[name] = savings;
        }

        static void OpenOverdraft()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            if (!Accounts.ContainsKey(name))
            {
                Console.WriteLine("Account not found.");
                return;
            }

            CurrentAccount currentAccount = new CurrentAccount(name, Accounts[name].Balance, 500);
            Console.Write("Enter amount to withdraw: ");
            double amountToWithdraw = Convert.ToDouble(Console.ReadLine());
            currentAccount.Withdraw(amountToWithdraw);
            Accounts[name] = currentAccount;
        }

        static void ShowTransactions()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            if (!Accounts.ContainsKey(name))
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Accounts[name].ShowTransactionHistory();
        }
    }
}
