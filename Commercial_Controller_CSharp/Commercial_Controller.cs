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
    public readonly int id;
    public readonly string status;
    public readonly List<Cage> cages;
    public readonly List<int> floorsServed;

    public Column(int id, string status, List<Cage> cages, List<int> floorsServed)
    {
        this.id = id;
        this.status = status;
        this.cages = cages;
        this.floorsServed = floorsServed;
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
    public string status;

    public CallButton(string status)
    {
        this.status = status;
    }

    public void CallButtonPressed()
    {
        this.status = "Active";
    }
}

public class FloorButton
{
    public string status = "Inactive";

    public FloorButton(string status)
    {
        this.status = status;
    }

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

    public Floor(int id, CallButton button)
    {
        this.id = id;
        this.button = button;
    }
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

////////////////////////////////////////////////////////////////////////
// This object contains all the column and cage objects in the system //
// Only one CageManager should instantiated and only after Config has //
// been called during the initial setup.                              //
////////////////////////////////////////////////////////////////////////
public class CageManager
{
    public List<Column> colList = new List<Column>();
    
    public CageManager()
    {
        int floorRange;
        int extraFloors;
        if (Configuration.totalBasements > 0)
        {
            if((Configuration.totalFloors -1) % (Configuration.totalColumns -1) != 0)
            {
                floorRange = ((Configuration.totalFloors - 1) / (Configuration.totalColumns - 1));
                extraFloors = ((Configuration.totalFloors - 1) % (Configuration.totalColumns - 1));
            }
            else
            {
                floorRange = ((Configuration.totalFloors - 1) / (Configuration.totalColumns - 1));
                extraFloors = 0;
            }
        } 
        else 
        {
            if(Configuration.totalFloors % Configuration.totalColumns != 0)
            {
                floorRange = Configuration.totalFloors / Configuration.totalColumns;
                extraFloors = Configuration.totalFloors % Configuration.totalColumns;
            }
            else
            {
                floorRange = Configuration.totalFloors / Configuration.totalColumns;
                extraFloors = 0;
            }
        }

        List<Column> colList = this.colList;

        if (Configuration.totalBasements > 0)
        {
            int floorCounter = 2;
            List<int> bColumnFloors = new List<int>();
            for (int x = 0; x < Configuration.totalBasements; x++)
            {
                if (Configuration.floorList[x].id < 0)
                {
                    bColumnFloors.Add(Configuration.floorList[x].id);
                }
            }
            bColumnFloors.Add(1);
            colList.Add(new Column(1, "Active", this.GenerateCages(Configuration.cagesPerColumn), bColumnFloors));
            for (int x = 2; x <= Configuration.totalColumns; x++)
            {
                List<int> floorsServed = new List<int>();
                floorsServed.Add(1);
                if (Configuration.totalColumns - x < extraFloors)
                {
                    for (int i = 0; i < floorRange + 1; i++)
                    {
                        floorsServed.Add(floorCounter);
                        floorCounter++;
                    }
                    colList.Add(new Column(x, "Active", this.GenerateCages(Configuration.cagesPerColumn), floorsServed));
                }
                else
                {
                    for (int i = 0; i < floorRange; i++)
                    {
                        floorsServed.Add(floorCounter);
                        floorCounter++;
                    }
                    colList.Add(new Column(x, "Active", this.GenerateCages(Configuration.cagesPerColumn), floorsServed));
                }
            }
        }
        else 
        {
            int floorCounter = 2;
            for (int x = 1; x <= Configuration.totalColumns; x++)
            {
                List<int> floorsServed = new List<int>();
                floorsServed.Add(1);
                for (int i = 0; i < floorRange; i++)
                {
                    floorsServed.Add(floorCounter);
                    floorCounter++;
                }
                colList.Add(new Column(x, "Active", this.GenerateCages(Configuration.cagesPerColumn), floorsServed));
            }
        }
        
        for (int x = 1; x <= Configuration.totalColumns; x++)
        {
            // TO DO
        }
    }

    // Methods //

    // This method loops through all cages in a given column and returns //
    // the cage which can most efficiently fulfill the given request.    // 
    public Cage GetCage(string direction, int column, Floor reqFloor )
    {
        Cage curCage = this.colList[0].cages[0];
        for (int x = 0; x < this.colList[column].cages.Count; x++)
        {
            curCage = this.colList[column].cages[x];
            if (curCage.direction == direction && direction == "Up" && curCage.curFloor < reqFloor.id && (curCage.status == "In-Service" || curCage.status == "Loading"))
            {
                // Console.WriteLine("Same direction UP was selected"); // **For debugging**
                return curCage; // Returns the cage already going the same direction (UP) that has not yet passed the requested floor
            } else if (curCage.direction == direction && direction == "Down" && curCage.curFloor < reqFloor.id && (curCage.status == "In-Service" || curCage.status == "Loading"))
            {
                // Console.WriteLine("Same direction DOWN was selected"); // **For debugging**
                return curCage; // Returns the cage already going the same direction (DOWN) that has not yet passed the requested floor
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
                            curCage = compareCage; // Closest idle cage
                        }
                    }
                    // Console.WriteLine("Cage " + curCage.id + " is selected.") // **For debugging**
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
                return curCage; // Returns the least occupied cage
            }
        }
        return curCage; // Returns the idle cage closest to the requested pickup
    }

    // Returns a column where the requested floor is served //
    public Column GetColumn(Floor reqFloor)
    {
        for (int x = 0; x < this.colList.Count; x++)
        {
            for (int i = 0; i < this.colList[x].floorsServed.Count; i++)
            {
                if (reqFloor.id == this.colList[x].floorsServed[i])
                {
                    return this.colList[x];
                }
            }
        }
        return null;
    }

    // Instantiates cages based off a given number //
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

    // Prints all columns and their cages as well as their current floor and status //
    public void GetCageStatus()
    {
        for (int x = 0; x < this.colList.Count; x++)
        {
            for (int i = 0; i < this.colList[x].cages.Count; i++)
            {   
                Cage curCage = this.colList[x].cages[i];
                Console.WriteLine("Column " + this.colList[x].id + ": Cage " + curCage.id + " is " + curCage.status);
                Console.WriteLine("Current floor: " + curCage.curFloor + " Door status: " + curCage.doors);
            }
        }
    }

    // Returns a string of the floors served by a given column //
    public string GetFloorsServed(Column column)
    {
        string myFloors = string.Join(",", column.floorsServed);
        string myString = "Column " + column.id + ": " + myFloors;
        return myString;
    }
}


//////////////////////////
// System Configuration //
//////////////////////////

////////////////////////////////////////////////////////////////////////////////
// This static object generates a hardware configuration from user input and  //
// the corresponding floor list.                                              //
////////////////////////////////////////////////////////////////////////////////
public static class Configuration
{
    public static bool batteryOn;
    public static int totalColumns;
    public static int cagesPerColumn;
    public static int totalFloors;
    public static int totalBasements;

    public static List<Floor> floorList = new List<Floor>();

    // Gets integer value from the user - takes a user prompt and minimum value //
    //          ---IMPORTANT: minValue should never be less than 0---           //
    public static int GetIntInput(string prompt, int minValue)
    {
        Console.WriteLine(prompt);
        int myInt = -1;
        string userInt = Console.ReadLine();
        while(myInt == -1)
        {
            try
            {
                myInt = Convert.ToInt32(userInt);
                if(myInt < minValue)
                {
                    Console.WriteLine("Value cannot be less than " + minValue + ".");
                    myInt = -1;
                    userInt = "";
                }
            }
            catch(System.FormatException)
            {
                if(userInt == "")
                {
                    Console.WriteLine("Please enter a valid number.");
                    userInt = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(userInt + " is not a valid number.\nPlease enter a valid number.");
                    userInt = Console.ReadLine();
                }
            }
        }
        return myInt;
    }

    // To be called once upon startup: Generates a hardware configuration based on user input //
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
        int totalColumns = GetIntInput("Enter the total number of columns", 1);

        // Set cages per column //
        int cagesPerColumn = GetIntInput("How many cages are installed per column?", 1);

        // Set number of floors //
        int totalFloors = GetIntInput("How many floors (excluding basements) are there in the building?", 2);

        // Set number of basements //
        int totalBasements = GetIntInput("How many basements are there?", 0);

        // Set Configuration Values //
        Configuration.batteryOn = true;
        Configuration.totalColumns = totalColumns;
        Configuration.cagesPerColumn = cagesPerColumn;
        Configuration.totalFloors = totalFloors;
        Configuration.totalBasements = totalBasements;

        // Confirm Setup Conditions //
        Console.WriteLine("\n-------HARDWARE SIMULATION-------");
        Console.WriteLine($"\n{"Hardware",-17} {"Value",15}\n");
        Console.WriteLine($"{"Battery", -17} {"On", 15}");
        Console.WriteLine($"{"Total Columns", -17} {Configuration.totalColumns, 15}");
        Console.WriteLine($"{"Cages Per Column", -17} {Configuration.cagesPerColumn, 15}");
        Console.WriteLine($"{"Total Floors", -17} {Configuration.totalFloors, 15}");
        Console.WriteLine($"{"Total Basements", -17} {Configuration.totalBasements, 15}");
    }

    // To be called after Config: Generates floor objects and adds them to floor list //
    public static void GenerateFloors()
    {
        // Checks if building has basements and adds them to the floor list //
        if (totalBasements > 0)
        {
            for (int x = 0 - totalBasements; x < 0; x++)
            {
                floorList.Add(new Floor(x, new CallButton("Inactive")));
            }
        }

        // Adds remaining floors to the floor list //
        for (int x = 1; x < 1 + totalFloors; x++)
        {
            floorList.Add(new Floor(x, new CallButton("Inactive")));
        }
    }

    // Reports //
    public static void GetFloorStatus()
    {
        Console.WriteLine("\n-----------------FLOOR STATUS------------------\n");
        for (int x = 0; x < floorList.Count; x++)
        {
            Console.WriteLine(String.Format("{0, -6} {1, 2} {2, -26} {3, -8}", "Floor ", floorList[x].id, ":  Active  //  Call Status: ", floorList[x].button.status));
        }
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
        Configuration.GenerateFloors();
        Configuration.GetFloorStatus();
        CageManager myCageManager = new CageManager();
        myCageManager.GetCageStatus();
        for (int x = 0; x < myCageManager.colList.Count; x++)
        {
            Console.WriteLine(myCageManager.GetFloorsServed(myCageManager.colList[x]));
        }
    }
}

