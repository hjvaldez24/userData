using System;
using System.Collections.Generic;
using System.Linq; 
using System.IO;
using System.Text.Json;

namespace MyApplication
{
  class Program
  {
    public class User 
    {
      public int Id { get; set;}
      public string fullName { get; set; }
      public string userName { get; set; }
      public string password { get; set; }
      public decimal Balance { get; set; }
    }

    static void Main(string[] args)
    {
        string jsonfile = "userData/userDatabase.json";
        List<User> userData = new List<User>(); 

        // 1. Load the static database from JSON if it exists
        if (File.Exists(jsonfile))
        {
            try
            {
                string jsonString = File.ReadAllText(jsonfile);
                // Deserialize directly into our main userData list
                var loadedUsers = JsonSerializer.Deserialize<List<User>>(jsonString);
                
                if (loadedUsers != null)
                {
                    userData = loadedUsers;
                }
                Console.WriteLine("User database loaded successfully!\n"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading database: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Database file not found! Using fallback data."); 
            // Fallback default user if file doesn't exist yet
            userData.Add(new User { Id = 1, fullName = "shin", userName = "shin123", password = "password123", Balance = 12496m });
        }

        // 2. Login Prompt
        User currentUser = null;
        bool isLoggedIn = false;

        while (!isLoggedIn)
        {
            Console.WriteLine("=== BANK LOGIN ===");
            Console.Write("Enter Username: ");
            string inputUser = Console.ReadLine().Trim();

            Console.Write("Enter Password: ");
            string inputPass = Console.ReadLine().Trim();

            // Find user matching both username and password
            currentUser = userData.FirstOrDefault(u => u.userName.Equals(inputUser, StringComparison.OrdinalIgnoreCase) && u.password == inputPass);

            if (currentUser != null)
            {
                Console.WriteLine($"\nLogin Successful! Welcome, {currentUser.fullName}.");
                isLoggedIn = true;
            }
            else
            {
                Console.WriteLine("\nInvalid username or password. Please try again.\n");
            }
        }

        // 3. ATM / Banking Loop
        bool picking = true;
        while (picking)
        {
            Console.WriteLine($"\nUser Balance: ${currentUser.Balance:N2}");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("[1] Deposit");
            Console.WriteLine("[2] Withdraw");
            Console.WriteLine("[3] Exit");
            Console.Write("\nEnter your choice (1-3): ");
            
            string choice = Console.ReadLine().Trim();

            switch (choice)
            {
                case "1": 
                    Console.WriteLine("\n--- DEPOSIT ---");
                    Console.Write("Enter your deposit amount: ");
                    string inputAmount = Console.ReadLine();

                    if (decimal.TryParse(inputAmount, out decimal parsedAmount) && parsedAmount > 0)
                    {
                        currentUser.Balance += parsedAmount;
                        Console.WriteLine("Deposit successful!");
                        Console.WriteLine($"New Balance: ${currentUser.Balance:N2}");
                    }
                    else
                    {
                        Console.WriteLine("You entered an invalid amount, please try again.");
                    }
                    break;

                case "2":
                    Console.WriteLine("\n--- WITHDRAW ---");
                    Console.Write("Enter your withdraw amount: ");
                    string withdrawInput = Console.ReadLine();

                    if (decimal.TryParse(withdrawInput, out decimal withdrawAmount) && withdrawAmount > 0)
                    {
                        if (withdrawAmount <= currentUser.Balance)
                        {
                            currentUser.Balance -= withdrawAmount;
                            Console.WriteLine($"Withdrawal successful! New Balance: ${currentUser.Balance:N2}");
                        }
                        else
                        {
                            Console.WriteLine("Error: Insufficient funds!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount entered. Please try again.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Thank you for Banking with us!");
                    picking = false;
                    break;
                
                default:
                    Console.WriteLine("Invalid selection. Please choose 1, 2, or 3.");
                    break;
            }

            if (picking)
            {
                Console.Write("\nDo you want to perform another transaction? (y/n): ");
                string confirmationInput = Console.ReadLine()?.Trim().ToLower();
                
                if (confirmationInput == "y" || confirmationInput == "yes")
                {
                    Console.WriteLine("Continuing...\n");
                } 
                else
                {
                    Console.WriteLine("Exiting program. Goodbye!");
                    picking = false;
                }
            }
        }
    }
  }
}