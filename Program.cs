using System;

namespace MyApplication
{
  class Program
  {
    static void Main(string[] args)
    {
        string userInput = string.Empty;
        string userDatNname = "hjvaldez";
        
        while( userInput != userDatNname )
        {
        	Console.WriteLine("Enter user data:");
        	userInput = Console.ReadLine() ?? string.Empty;
        
            if (userInput != userDatNname){

                Console.WriteLine($"Welcome Back User {userDatNname}!");

            }
            else
            {
                Console.WriteLine($"User Data Name was not FOUND please TRY AGAIN!");
            }
        }
    }
  }
}