using System;
using System.Collections.Generic;
using System.Linq; 
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace MyApplication
{
    class Program
    {
        [Table("profiles")]
        public class Profile : BaseModel
        {
            [PrimaryKey("id", false)]
            [Column("id")]
            public string? Id { get; set; }

            [Column("fullname")]
            public string? FullName { get; set; }

            [Column("username")]
            public string? UserName { get; set; }

            [Column("password")]
            public string? Password { get; set; }

            [Column("balance")]
            public decimal Balance { get; set; }
        }

        static async Task Main(string[] args)
        {
            // 1. Load Environment Variables safely
            DotNetEnv.Env.Load();
            string url = Environment.GetEnvironmentVariable("SUPABASE_URL")?.TrimEnd('/') ?? "";
            string key = Environment.GetEnvironmentVariable("SUPABASE_KEY") ?? "";

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Error: SUPABASE_URL or SUPABASE_KEY is missing from your .env file!");
                return;
            }

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = false,
                Schema = "public"
            };

            var supabase = new Supabase.Client(url, key, options);
            await supabase.InitializeAsync();

            // 2. Fetch User Data using the typed generic model query
            Console.WriteLine("Connecting to Supabase and fetching profiles...");
            
            var response = await supabase.From<Profile>().Get();
            List<Profile> userData = response.Models;

            if (userData == null || userData.Count == 0)
            {
                Console.WriteLine("No user profiles found in the database! Please check your table data.");
                return;
            }

            // 3. Login Prompt
            Profile? currentUser = null;
            bool isLoggedIn = false;

            while (!isLoggedIn)
            {
                Console.Clear();
                Console.WriteLine("=== BANK LOGIN ===");
                Console.Write("Enter Username: ");
                string inputUser = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Enter Password: ");
                string inputPass = Console.ReadLine()?.Trim() ?? "";

                currentUser = userData.FirstOrDefault(u => 
                    u.UserName != null && u.UserName.Equals(inputUser, StringComparison.OrdinalIgnoreCase) && 
                    u.Password == inputPass);

                if (currentUser != null)
                {
                    Console.WriteLine($"\nLogin Successful! Welcome, {currentUser.FullName}.");
                    System.Threading.Thread.Sleep(1500);
                    isLoggedIn = true;
                }
                else
                {
                    Console.WriteLine("\nInvalid username or password. Please try again.\n");
                    Console.WriteLine("Press any key to retry...");
                    Console.ReadKey();
                }
            }

            // 4. ATM / Banking Loop
            bool picking = true;
            while (picking)
            {
                Console.Clear();
                if (currentUser == null) break;

                Console.WriteLine($"\nUser Balance: ${currentUser.Balance:N2}");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("[1] Deposit");
                Console.WriteLine("[2] Withdraw");
                Console.WriteLine("[3] Exit");
                Console.Write("\nEnter your choice (1-3): ");
                
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1": 
                        Console.WriteLine("\n--- DEPOSIT ---");
                        Console.Write("Enter your deposit amount: ");
                        string inputAmount = Console.ReadLine() ?? "";

                        if (decimal.TryParse(inputAmount, out decimal parsedAmount) && parsedAmount > 0)
                        {
                            currentUser.Balance += parsedAmount;
                            await supabase.From<Profile>().Update(currentUser);

                            Console.WriteLine("Deposit successful and saved to database!");
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
                        string withdrawInput = Console.ReadLine() ?? "";

                        if (decimal.TryParse(withdrawInput, out decimal withdrawAmount) && withdrawAmount > 0)
                        {
                            if (withdrawAmount <= currentUser.Balance)
                            {
                                currentUser.Balance -= withdrawAmount;
                                await supabase.From<Profile>().Update(currentUser);

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
                    string confirmationInput = Console.ReadLine()?.Trim().ToLower() ?? "";
                    
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