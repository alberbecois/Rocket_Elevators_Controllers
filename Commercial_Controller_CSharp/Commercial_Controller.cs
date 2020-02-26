///////////////////////////////////////////////////////
// {Commercial Controller}                          //
///////////////////////////////////////////////////////
// Author: {Joshua Knutson}                          //
// License: {GNUGPLv3}                               //
// Link: {https://www.gnu.org/licenses/gpl-3.0.html} //
// Version: {0.01}                                   //
// Contact: {github.com/alberbecois}                 //
///////////////////////////////////////////////////////

using System;
using System.Threading;

//////////////////
// Cage Manager //
//////////////////

public class CageManager
{
    
}


//////////////////////////
// System Configuration //
//////////////////////////

public class Configuration
{
    public readonly bool batteryOn;
    public readonly int totalColumns;
    public readonly int cagesPerColumn;
    public readonly int totalFloors;
    public readonly int totalBasements;

    public Configuration(bool batteryOn, int totalColumns, int cagesPerColumn, int totalFloors, int totalBasements)
    {
        this.batteryOn = batteryOn;
        this.totalColumns = totalColumns;
        this.cagesPerColumn = cagesPerColumn;
        this.totalFloors = totalFloors;
        this.totalBasements = totalBasements;
    }
}


//////////
// Main //
//////////

class Program
{
    static void Main(string[] args)
    {
        ConsoleKeyInfo cki;
        do {
            Console.WriteLine("Activate battery? [y/n]");
            while (Console.KeyAvailable == false)
            {
                Thread.Sleep(250); // Loop until valid input is entered.
            }    
        
            cki = Console.ReadKey(true);
            Console.WriteLine("You pressed the '{0}' key. Please make a valid selection.", cki.Key);
            if(cki.Key == ConsoleKey.N)
            {
                Console.WriteLine("Startup Aborted!");
                return;
            }
        } while(cki.Key != ConsoleKey.Y);
        
        Console.WriteLine("Initializing...");

        // Set total number of columns //
        Console.WriteLine("Enter the total number of columns");
        int totalColumns = 0;
        string userColumns = Console.ReadLine();
        while(totalColumns == 0)
        {
            try
            {
                totalColumns = Convert.ToInt32(userColumns);
                if(totalColumns < 1)
                {
                    Console.WriteLine("Value cannot be less than one.");
                    totalColumns = 0;
                    userColumns = "";
                }
            }
            catch(System.FormatException)
            {
                if(userColumns == "")
                {
                    Console.WriteLine("Please enter a valid number.");
                    userColumns = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(userColumns + " is not a valid number.");
                    Console.WriteLine("Please enter a valid number.");
                    userColumns = Console.ReadLine();
                }
            }
        }

        // Set cages per column //
        Console.WriteLine("How many cages are installed per column?");
        int cagesPerColumn = 0;
        string userCagesPerColumn = Console.ReadLine();
        while(cagesPerColumn == 0)
        {
            try
            {
                cagesPerColumn = Convert.ToInt32(userCagesPerColumn);
                if(cagesPerColumn < 1)
                {
                    Console.WriteLine("Value cannot be less than one.");
                    cagesPerColumn = 0;
                    userCagesPerColumn = "";
                }
            }
            catch(System.FormatException)
            {
                if(userCagesPerColumn == "")
                {
                    Console.WriteLine("Please enter a valid number.");
                    userCagesPerColumn = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(userCagesPerColumn + " is not a valid number.");
                    Console.WriteLine("Please enter a valid number.");
                    userCagesPerColumn = Console.ReadLine();
                }
            }
        }

        // Set number of floors //
        Console.WriteLine("How many floors (excluding basements) are there in the building?");
        int totalFloors = 0;
        string userFloors = Console.ReadLine();
        while(totalFloors == 0)
        {
            try
            {
                totalFloors = Convert.ToInt32(userFloors);
                if(totalFloors < 1)
                {
                    Console.WriteLine("Value cannot be less than one.");
                    totalFloors = 0;
                    userFloors = "";
                }
            }
            catch(System.FormatException)
            {
                if(userFloors == "")
                {
                    Console.WriteLine("Please enter a valid number.");
                    userFloors = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(userFloors + " is not a valid number.");
                    Console.WriteLine("Please enter a valid number.");
                    userFloors = Console.ReadLine();
                }
            }
        }

        // Set number of basements //
        Console.WriteLine("How many basements are there?");
        int totalBasements = 0;
        string userBasements = Console.ReadLine();
        while(totalBasements == 0)
        {
            try
            {
                totalBasements = Convert.ToInt32(userBasements);
                if(totalBasements < 1)
                {
                    Console.WriteLine("Value cannot be less than one.");
                    totalBasements = 0;
                    userBasements = "";
                }
            }
            catch(System.FormatException)
            {
                if(userBasements == "")
                {
                    Console.WriteLine("Please enter a valid number.");
                    userBasements = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(userBasements + " is not a valid number.");
                    Console.WriteLine("Please enter a valid number.");
                    userBasements = Console.ReadLine();
                }
            }
        }

        // Confirm Setup Conditions //
        Console.WriteLine("\n-------HARDWARE SIMULATION-------");
        Console.WriteLine(String.Format("\n{0, -17} {1, 8}\n", "Hardware", "Value"));
        Console.WriteLine(String.Format("{0, -17} {1, 8}", "Battery", "On"));
        Console.WriteLine(String.Format("{0, -17} {1, 8}", "Total Columns", userColumns));
        Console.WriteLine(String.Format("{0, -17} {1, 8}", "Cages Per Column", userCagesPerColumn));
        Console.WriteLine(String.Format("{0, -17} {1, 8}", "Total Floors", userFloors));
        Console.WriteLine(String.Format("{0, -17} {1, 8}", "Total Basements", userBasements));
    }
}

