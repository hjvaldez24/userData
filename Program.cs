using System;
using System.Collections.Generic;
using System.Linq; 

namespace MyApplication
{
  class Program
  {
    public class User 
    {
      public int Id { get; set;}
      public string Name { get; set; }
      public decimal Balance { get; set; }
    }

    static void Main(string[] args)
    {
        List<User> userData = new List<User>(); 
        userData.Add(new User { Id = 1, Name = "shin", Balance = 12496m }); 

        Console.WriteLine("Enter User Data (Name): ");
        string inpput1 = Console.ReadLine().ToLower().Trim();

        bool picking = true;

        while (picking)
        {
            //THis line is to find the user first so we can use their details
            var currentUser = userData.FirstOrDefault(i => i.Name == inpput1);

            if (currentUser != null)
            {
                //Console.WriteLine("\nUser Data Found!");
                Console.WriteLine($"\n Welsome Use {currentUser.Name}");
                Console.WriteLine($"\nUser Balance: ${currentUser.Balance}");
                
                Console.WriteLine("What do you like to do?");
                Console.WriteLine("[1] Deposit");
                Console.WriteLine("[2] Withdraw");
                Console.WriteLine("\n[3] Exit");
                Console.Write("\nEnter your choice (1-3): ");
                
                string choice = Console.ReadLine().Trim();

                switch (choice)
                {
                  case "1": // this is the area where you will deposite your money
                      Console.WriteLine("\n DEPOSIT");
                      Console.WriteLine("\nEnter your deposit amount: ");

                      string inputAmount = Console.ReadLine();

                      if (decimal.TryParse(inputAmount, out decimal parsedAmount))
                      {
                          currentUser.Balance += parsedAmount;
                          Console.WriteLine("Deposit successful");
                          Console.WriteLine($"New Balance: ${currentUser.Balance}");
                      }
                      else
                      {
                          Console.WriteLine("You entered an invalid amount, please try again.");
                      }
                      break;;
                  case "2":
                      Console.WriteLine("\nWithdraw");

                      Console.WriteLine("Enter you withdraw amount: ");
                      string withdrawInput = Console.ReadLine();

                      if (decimal.TryParse(withdrawInput, out decimal withdrawAmount) && withdrawAmount > 0)
                      {
                        if (withdrawAmount <= currentUser.Balance)
                        {
                            currentUser.Balance -= withdrawAmount;
                            Console.WriteLine($"Withdrawal successful! New Balance: ${currentUser.Balance}");
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
                    Console.WriteLine("Invalid selection. PLease choose 1, 2, or 3,");
                    break;
                }
            }
            else
            {
                // If user isn't found, you might want to break or let them re-enter, 
                // but it will move to the confirmation prompt below.
                Console.WriteLine("User Data not Found!");
            }

            Console.WriteLine("\nDo you want to continue? (y/n):");
            string confirmationInput = Console.ReadLine()?.Trim().ToLower();
            
            if (confirmationInput == "y" || confirmationInput == "yes")
            {
                Console.WriteLine("Continuing...\n");//This line of code is to search for a new user or do another transaction, keep looping.
            } 
            else if (confirmationInput == "n" || confirmationInput == "no")
            {
                Console.WriteLine("Exiting program. Goodbye!");
                picking = false; // Safely ends the while loop
            }
            else
            {
                Console.WriteLine("\nInvalid Key. Exiting by default.");
                picking = false;
            }
        }
    }
  }
}