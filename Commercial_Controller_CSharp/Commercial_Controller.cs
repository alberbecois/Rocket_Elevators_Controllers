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

    public Configuration(bool batteryOn, int totalColumns, int cagesPerColumn, int totalFloors)
    {
        this.batteryOn = batteryOn;
        this.totalColumns = totalColumns;
        this.cagesPerColumn = cagesPerColumn;
        this.totalFloors = totalFloors;
    }
}

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
            Console.WriteLine("Enter the total number of columns: ");
            int totalColumns = Convert.ToInt32(Console.ReadLine());
    }
}



