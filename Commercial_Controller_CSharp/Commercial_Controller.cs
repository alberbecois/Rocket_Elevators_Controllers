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
using System.Collections.Generic;

/////////////
// Columns //
/////////////

public class Column
{
    public readonly string status;
    public readonly List<Cage> cages;
    public readonly List<Floor> floors;

    public Column(string status, List<Cage> cages, List<Floor> floors)
    {
        this.status = status;
        this.cages = cages;
        this.floors = floors;
    }
}

///////////
// Cages //
///////////

public class Cage
{
    public readonly int id;
    public string status;
    public string doors;
    public List<Request> requests = new List<Request>();
    public int curFloor = 1;
    public string direction = "Up";
    public int timer = 0;
    public string doorSensorStatus = "Clear";

    public Cage(int id, string status, string doors)
    {
        this.id = id;
        this.status = status;
        this.doors = doors;
    }

    // Door Methods //
    public void OpenDoors()
    {
        if (this.status == "Loading")
        {
            this.doors = "Open";
            Console.WriteLine("Cage doors are open for 8 seconds");
            this.timer = 8;
            while (this.timer > 0)
            {
                Console.WriteLine("Closing in " + this.timer + " seconds.");
                Thread.Sleep(1000);
                this.timer -= 1;
            }
            this.CloseDoors();
        }
    }

    public void OpenButtonPressed()
    {
        if (this.status != "In-Service")
        {
            this.OpenDoors();
        }
    }

    public void CloseDoors()
    {
        if (this.doorSensorStatus == "Clear" && this.timer < 5)
        {
            this.doors = "Closed";
            Console.WriteLine("Cage doors are closed");
            this.status = "Loading";
        }
    }

    public void CloseButtonPressed()
    {
        if (this.timer < 5)
        {
            this.CloseDoors();
        }
    }

    // Movement //
    public void MoveDown()
    {
        while (this.doors != "Closed")
        {
            this.CloseDoors();
        }
        this.status = "In-Service";
        this.direction = "Down";
        Console.WriteLine("Cage " + this.id + " going down at " + this.curFloor);
        this.curFloor -= 1;
        Console.WriteLine("Cage " + this.id + " at " + this.curFloor);
        this.status = "Loading";
    }

    public void MoveUp(Floor reqFloor)
    {
        while (this.doors != "Closed")
        {
            this.CloseDoors();
        }
        this.status = "In-Service";
        this.direction = "Up";
        Console.WriteLine("Cage " + this.id + " going up at " + this.curFloor);
        this.curFloor += 1;
        Console.WriteLine("Cage " + this.id + " at " + this.curFloor);
        this.status = "Loading";
    }
}


/////////////
// Buttons //
/////////////

public class CallButton
{
    public string status = "Inactive";

    public void CallButtonPressed()
    {
        this.status = "Active";
    }
}

public class FloorButton
{
    public string status = "Inactive";

    public void FloorButtonPressed()
    {
        this.status = "Active";
    }
}


///////////
// Panel //
///////////

public class Panel
{
    public readonly List<FloorButton> floorButtons;

    public Panel(List<FloorButton> floorButtons)
    {
        this.floorButtons = floorButtons;
    }
}


////////////
// Floors //
////////////

public class Floor
{
    public readonly int id;
    public CallButton button;
}


//////////////
// Requests //
//////////////

public class Request
{
    public string status;
    public Floor floor;
}


//////////////////
// Cage Manager //
//////////////////

public class CageManager
{
    public List<Column> colList = new List<Column>();
    public CageManager()
    {
        for (int x = 1; x <= Configuration.totalColumns; x++)
        {
            // TO DO
        }
    }

    // Methods //
    public Cage GetCage(string direction, int column, Floor reqFloor )
    {
        for (int x = 0; x < this.colList[column].cages.Count; x++)
        {
            Cage curCage = this.colList[column].cages[x];
            if (curCage.direction == direction && direction == "Up" && curCage.curFloor < reqFloor.id && (curCage.status == "In-Service" || curCage.status == "Loading"))
            {
                // Console.WriteLine("Same direction UP was selected"); // **For debugging**
                return curCage; // Going same direction (UP) before requested floor
            } else if (curCage.direction == direction && direction == "Down" && curCage.curFloor < reqFloor.id && (curCage.status == "In-Service" || curCage.status == "Loading"))
            {
                // Console.WriteLine("Same direction DOWN was selected"); // **For debugging**
                return curCage; // Going same direction (DOWN) before requested floor
            } else if (curCage.status == "Idle")
            {
                for (int i = 0; i < this.colList[column].cages.Count; i++)
                {
                    if (curCage != this.colList[column].cages[i])
                    {
                        Cage compareCage = this.colList[column].cages[i];
                        // Console.WriteLine("Cage " + curCage.id + " to be compared to " + compareCage.id); // **For debugging**
                        int gapA = Math.Abs(curCage.curFloor - reqFloor.id);
                        int gapB = Math.Abs(compareCage.curFloor - reqFloor.id);
                        if (gapB < gapA)
                        {
                            curCage = compareCage;
                        }
                    }
                    // Console.WriteLine("Cage " + curCage.id + " is selected.") // **For debugging**
                    return curCage; // Closest idle cage
                }
            } else 
            {
                for (int i = 0; i < this.colList[column].cages.Count; i++)
                {
                    if (this.colList[column].cages[i].requests.Count < curCage.requests.Count)
                    {
                        curCage = this.colList[column].cages[i];
                    }
                }
                // Console.WriteLine("Least occupied cage is selected"); // **For debugging**
                return curCage; // Least occupied cage
            }
        }
        return null;
    }

    public Column GetColumn(Floor reqFloor)
    {
        for (int x = 0; x < this.colList.Count; x++)
        {
            for (int i = 0; i < this.colList[x].floors.Count; i++)
            {
                if (reqFloor == this.colList[x].floors[i])
                {
                    return this.colList[x];
                }
            }
        }
        return null;
    }

    public List<Cage> GenerateCages(int numCages)
    {
        List<Cage> cageList = new List<Cage>();
        for (int x = 1; x <= numCages; x++)
        {
            cageList.Add(new Cage(x, "Idle", "Closed"));
        }
        return cageList;
    }

    // Reports //
    public void GetCageStatus()
    {
        for (int x = 0; x < this.colList.Count; x++)
        {
            for (int i = 0; i < this.colList[x].cages.Count; i++)
            {   
                Cage curCage = this.colList[x].cages[i];
                Console.WriteLine("Column " + x + ": Cage " + i + " is " + curCage.status);
                Console.WriteLine("Current floor: " + curCage.curFloor + " Door status: " + curCage.doors);
            }
        }
    }
}


//////////////////////////
// System Configuration //
//////////////////////////

public static class Configuration
{
    public static bool batteryOn;
    public static int totalColumns;
    public static int cagesPerColumn;
    public static int totalFloors;
    public static int totalBasements;

    public static void Config()
    {
        ConsoleKeyInfo cki;
        do {
            Console.WriteLine("Activate battery? [y/n]");
            while (Console.KeyAvailable == false)
            {
                Thread.Sleep(250); // Loop until valid input is entered.
            }    
        
            cki = Console.ReadKey(true);
            if(cki.Key != ConsoleKey.Y && cki.Key != ConsoleKey.N)
            {
                Console.WriteLine("You pressed the '{0}' key. Please make a valid selection.", cki.Key);
            } else if(cki.Key == ConsoleKey.N)
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

        // Set Configuration Values //
        Configuration.batteryOn = true;
        Configuration.totalColumns = totalColumns;
        Configuration.cagesPerColumn = cagesPerColumn;
        Configuration.totalFloors = totalFloors;
        Configuration.totalBasements = totalBasements;

        // Confirm Setup Conditions //
        Console.WriteLine("\n-------HARDWARE SIMULATION-------");
        Console.WriteLine(String.Format("\n{0, -17} {1, 15}\n", "Hardware", "Value"));
        Console.WriteLine(String.Format("{0, -17} {1, 15}", "Battery", "On"));
        Console.WriteLine(String.Format("{0, -17} {1, 15}", "Total Columns", Configuration.totalColumns));
        Console.WriteLine(String.Format("{0, -17} {1, 15}", "Cages Per Column", Configuration.cagesPerColumn));
        Console.WriteLine(String.Format("{0, -17} {1, 15}", "Total Floors", Configuration.totalFloors));
        Console.WriteLine(String.Format("{0, -17} {1, 15}", "Total Basements", Configuration.totalBasements));
    }
}


//////////
// Main //
//////////

class Program
{
    static void Main(string[] args)
    {
        Configuration.Config();
    }
}

