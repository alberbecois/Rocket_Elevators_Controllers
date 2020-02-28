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
using System.Linq;

/////////////
// Columns //
/////////////

////////////////////////////////////////////////////////////////////////
// The column object has a list of cages as well as list of served    //
// floors.                                                            //
////////////////////////////////////////////////////////////////////////

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

////////////////////////////////////////////////////////////////////////
// Each Column object has a list of Cage objects which contain all    //
// the necessary methods for moving up and down as well as a list of  //
// pickup requests.                                                   //
////////////////////////////////////////////////////////////////////////

public class Cage
{
    public readonly int id;
    public string status;
    public string doors;
    public List<Request> pickupRequests = new List<Request>();
    public List<Request> destinationRequests = new List<Request>();
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

    public void CleanUpRequests()
    {
        for (int x = this.pickupRequests.Count -1; x >= 0; x--)
        {
            if(this.curFloor == this.pickupRequests[x].pickup)
            {
                this.pickupRequests[x].status = "Destination";
            }
            if(this.pickupRequests[x].status == "Destination")
            {
                this.destinationRequests.Add(this.pickupRequests[x]);
                this.pickupRequests.Remove(this.pickupRequests[x]);
                Console.WriteLine("Destination is now " + this.destinationRequests[x].destination);
            }
        }
        for (int x = this.destinationRequests.Count -1; x >= 0; x--)
        {
            if(this.curFloor == this.destinationRequests[x].destination)
            {
                this.destinationRequests[x].status = "Completed";
            }
            if(this.destinationRequests[x].status == "Completed")
            {
                this.destinationRequests.Remove(this.destinationRequests[x]);
            }
        }
    }
    // Door Methods //
    public void OpenDoors()
    {
        if (this.status != "In-Service")
        {
            this.doors = "Open";
            Console.WriteLine("Cage " + this.id + " doors are open for 8 seconds");
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
            if(this.pickupRequests.Count == 0 && this.destinationRequests.Count == 0)
            {
                this.status = "Idle";
            }
            else 
            {
                this.status = "Loading";
            }
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
        if(this.curFloor -1 == 0)
        {
            this.curFloor -= 2;
        }
        else
        {
            this.curFloor -= 1;
        }
        Console.WriteLine("Cage " + this.id + " at " + this.curFloor);
        this.status = "Loading";
    }

    public void MoveUp()
    {
        while (this.doors != "Closed")
        {
            this.CloseDoors();
        }
        this.status = "In-Service";
        this.direction = "Up";
        Console.WriteLine("Cage " + this.id + " going up at " + this.curFloor);
        if(this.curFloor +1 == 0)
        {
            this.curFloor += 2;
        }
        else
        {
            this.curFloor += 1;
        }
        Console.WriteLine("Cage " + this.id + " at " + this.curFloor);
        this.status = "Loading";
    }
}


/////////////
// Buttons //
/////////////

////////////////////////////////////////////////////////////////////////
// Buttons are used to generate pickup requests from either the lobby //
// panel or from a given floor.                                       //
////////////////////////////////////////////////////////////////////////

public class CallButton
{
    public int id;
    public string status;

    public CallButton(int id, string status)
    {
        this.id = id;
        this.status = status;
    }

    public void CallButtonPressed()
    {
        this.status = "Active";
    }
}

public class FloorButton
{
    public int id;
    public string status;

    public FloorButton(int id, string status)
    {
        this.id = id;
        this.status = status;
    }
}


///////////
// Panel //
///////////

////////////////////////////////////////////////////////////////////////
// This object simulates a panel in the reception of the building.    //
// This panel directs the user to the right column for their request- //
// ed floor and sends the appropriate pickup request.                 //
// Only one panel should be instantiated AFTER Config() has been run  //
////////////////////////////////////////////////////////////////////////

public class Panel
{
    public readonly List<FloorButton> floorButtons = new List<FloorButton>();

    public Panel()
    {
        for (int x = 0 - Configuration.totalBasements; x < 0; x++)
        {
            floorButtons.Add(new FloorButton(x, "Inactive"));
        }
        for (int x = 1; x <= Configuration.totalFloors; x++)
        {
            floorButtons.Add(new FloorButton(x, "Inactive"));
        }
    }

    // Methods //
    public void RequestElevator(int floorNumber, CageManager cageManager)
    {
        foreach(FloorButton button in floorButtons)
        {
            if (button.id == floorNumber)
            {
                button.status = "Active";
            }
        }

        Column myColumn = cageManager.GetColumn(1, floorNumber);
        Console.WriteLine("Floor requested. Please proceed to column " + myColumn.id);

    }

    // Reports //
    public void GetFloorButtonsStatus()
    {
        for (int x = 0; x < this.floorButtons.Count; x++)
        {
            Console.WriteLine("Floor " + this.floorButtons[x].id + " button is " + this.floorButtons[x].status);
        }
    }
}


////////////
// Floors //
////////////

////////////////////////////////////////////////////////////////////////
// The floor object is generated by the Configuration object as a     //
// list of floors each with a call button equal to the number of      //
// total floors set by the user.                                      //
////////////////////////////////////////////////////////////////////////

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

////////////////////////////////////////////////////////////////////////
// A request object is generated each time a Floor or Call button is  //
// pressed. The request is queued by the main program before being    //
// assigned to a cage for treatment.                                  //
////////////////////////////////////////////////////////////////////////

public class Request
{
    public string status;
    public string assignment = "Unassigned";
    public int pickup;
    public int destination;
    public string direction;

    public Request(string status, int pickup, int destination, string direction)
    {
        this.status = status;
        this.pickup = pickup;
        this.destination = destination;
        this.direction = direction;
    }
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
    }

    // Methods //

    // This method loops through all cages in a given column and returns //
    // the cage which can most efficiently fulfill the given request.    // 
    public Cage GetCage(string direction, int column, int reqFloor )
    {
        Cage curCage = this.colList[column].cages[0];
        Cage bestCage = this.colList[column].cages[0];
        int x = 0;
        while ( x < this.colList[column].cages.Count )
        {
            curCage = this.colList[column].cages[x];
            if (curCage.direction == direction && direction == "Up" && curCage.curFloor < reqFloor && (curCage.status == "In-Service" || curCage.status == "Loading"))
            {
                Console.WriteLine("Same direction UP was selected"); // **For debugging**
                return curCage; // Returns the cage already going the same direction (UP) that has not yet passed the requested floor
            } 
            else if (curCage.direction == direction && direction == "Down" && curCage.curFloor > reqFloor && (curCage.status == "In-Service" || curCage.status == "Loading"))
            {
                Console.WriteLine("Same direction DOWN was selected"); // **For debugging**
                return curCage; // Returns the cage already going the same direction (DOWN) that has not yet passed the requested floor
            } 
            else if (curCage.status == "Idle")
            {
                bool allCagesAreIdle = true;
                for(int r = 0; r < this.colList[column].cages.Count; r++)
                {
                    if(this.colList[column].cages[r].status != "Idle")
                    {
                        allCagesAreIdle = false;
                    }
                }
                if(allCagesAreIdle)
                {
                    for (int i = x+1; i < this.colList[column].cages.Count; i++)
                    {
                        Cage compareCage = this.colList[column].cages[i];
                        if(compareCage.status == "Idle")
                        {
                            Console.WriteLine("Cage " + bestCage.id + " to be compared to " + compareCage.id); // **For debugging**
                            int gapA = Math.Abs(bestCage.curFloor - reqFloor);
                            int gapB = Math.Abs(compareCage.curFloor - reqFloor);
                            if (gapB < gapA)
                            {
                                bestCage = compareCage; // Closest idle cage
                            }
                        }
                        Console.WriteLine("Cage " + curCage.id + " is selected."); // **For debugging**
                    }
                    return bestCage;
                }
            } 
            else 
            {
                for (int i = 0; i < this.colList[column].cages.Count; i++)
                {
                    if (direction == "Up" && this.colList[column].cages[i].destinationRequests.Count < curCage.destinationRequests.Count)
                    {
                        curCage = this.colList[column].cages[i];
                    }
                    else if (direction == "Down" && this.colList[column].cages[i].pickupRequests.Count < curCage.pickupRequests.Count)
                    {
                        curCage = this.colList[column].cages[i];
                    }
                }
                Console.WriteLine("Least occupied cage is selected"); // **For debugging**
            }
            x++;
        }
        return curCage; // Returns the idle cage closest to the requested pickup
    }

    // Returns a column where the requested floor is served //
    public Column GetColumn(int pickup, int destination)
    {
        bool pickupServed = false;
        bool destServed = false;
        foreach (Column column in this.colList)
        {
            foreach(int id in column.floorsServed)
            {
                if (id == pickup)
                {
                    pickupServed = true;
                }
                if (id == destination)
                {
                    destServed = true;
                }
                if (pickupServed && destServed)
                {
                    return column;
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

////////////////////////////////////////////////////////////////////////
// This static object generates a hardware configuration from user    //
// input and the corresponding floor list.                            //
////////////////////////////////////////////////////////////////////////

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
                floorList.Add(new Floor(x, new CallButton(x, "Inactive")));
            }
        }

        // Adds remaining floors to the floor list //
        for (int x = 1; x < 1 + totalFloors; x++)
        {
            floorList.Add(new Floor(x, new CallButton(x, "Inactive")));
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
    // Checks all buttons and adds requests to the queue //
    static List<Request> requestQueue = new List<Request>();
    static void RequestGenerator(Panel myPanel)
    {
        // Checks call buttons //
        foreach(Floor floor in Configuration.floorList)
        {
            if (floor.button.status == "Active")
            {
                Console.WriteLine("Floor button " + floor.button.id + " is active.");
                if(floor.id > 0)
                {
                    Request myRequest = new Request("Pickup", floor.button.id, 1, "Down");
                    foreach(Request request in requestQueue)
                    {
                        if(floor.button.id == request.pickup && request.status == "Pickup")
                        {
                            Console.WriteLine("My request for floor " + floor.button.id + " was not sent.");
                            return;
                        }
                    }
                    requestQueue.Add(myRequest);
                    Console.WriteLine("My request for floor " + myRequest.pickup + " was added to the request list");
                }
                else
                {
                    Request myRequest = new Request("Pickup", floor.id, 1,  "Up");
                    foreach(Request request in requestQueue)
                    {
                        if(floor.id == request.pickup && request.status == "Pickup")
                        {
                            Console.WriteLine("My request for floor " + floor.button.id + " was not sent.");
                            return;
                        }
                    }
                    Console.WriteLine("My request for floor " + floor.button.id + " was added to the request list");
                    requestQueue.Add(myRequest);
                }
                floor.button.status = "Inactive";
                Console.WriteLine("Floor " + floor.button.id + " is " + floor.button.status);
            }
        }

        // Checks floor buttons //
        foreach(FloorButton button in myPanel.floorButtons)
        {
            if(button.status == "Active")
            {
                Console.WriteLine("Panel button " + button.id + " is " + button.status);
                if(button.id > 0)
                {
                    Request myRequest = new Request("Pickup", 1, button.id, "Up");
                    foreach(Request request in requestQueue)
                    {
                        if(myRequest.destination == request.destination && request.status == "Pickup")
                        {
                            Console.WriteLine("My request for floor " + button.id + " was not sent.");
                            return;
                        }
                    }
                    Console.WriteLine("My request for floor " + button.id + " was added to the request list");
                    requestQueue.Add(myRequest);
                }
                else
                {
                    Request myRequest = new Request("Pickup", 1, button.id, "Down");
                    foreach(Request request in requestQueue)
                    {
                        if(myRequest.destination == request.destination && request.status == "Pickup")
                        {
                            Console.WriteLine("My request for floor " + button.id + " was not sent.");
                            return;
                        }
                    }
                    Console.WriteLine("My request for floor " + myRequest.pickup + " was added to the request list");
                    requestQueue.Add(myRequest);
                }
                button.status = "Inactive";
                Console.WriteLine("Floor " + button.id + " is " + button.status);
            }
        }
    }

    // Assign each request to an elevator //
    static void AssignElevator(CageManager myCageManager)
    {
        foreach(Request request in requestQueue)
        {
            if(request.assignment == "Unassigned")
            {
                    Column myColumn = myCageManager.GetColumn(request.pickup, request.destination);
                    Console.WriteLine("Column " + myColumn.id + " is selected.");
                    Cage myCage = myCageManager.GetCage(request.direction, myColumn.id -1, request.pickup);
                    request.assignment = "Assigned";
                    myCage.pickupRequests.Add(request);
                    Console.WriteLine("Cage " + myCage.id + " receives request for floor " + myCage.pickupRequests[0].pickup);
                    myCage.pickupRequests.OrderBy(o=>o.pickup);
            }
        }
    }

    static void MoveElevators(CageManager myCageManager)
    {
        if (Configuration.totalBasements > 0)
        {
            foreach (Cage cage in myCageManager.colList[0].cages)
            {
               if(cage.pickupRequests.Count != 0)
               {
                   if(cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor > cage.pickupRequests[0].pickup)
                   {
                       cage.MoveDown();
                   }
                   else if (cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor < cage.pickupRequests[0].pickup)
                   {
                       cage.MoveUp();
                   }
                   else if (cage.curFloor == cage.pickupRequests[0].pickup)
                   {
                       cage.OpenDoors();
                       cage.pickupRequests[0].status = "Destination";
                       cage.CleanUpRequests();
                   }
               }
               if(cage.pickupRequests.Count == 0 && cage.destinationRequests.Count != 0)
               {
                   if(cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor > cage.destinationRequests[0].destination)
                   {
                       cage.MoveDown();
                   }
                   if(cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor < cage.destinationRequests[0].destination)
                   {
                       cage.MoveUp();
                   }
                   else if (cage.curFloor == cage.destinationRequests[0].destination)
                   {
                       cage.OpenDoors();
                       cage.destinationRequests[0].status = "Completed";
                       cage.CleanUpRequests();
                   }
               }
            }
            for (int x = 1; x < myCageManager.colList.Count; x++)
            {
                foreach(Cage cage in myCageManager.colList[x].cages)
                {
                    if(cage.pickupRequests.Count != 0)
                    {
                        if(cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor > cage.pickupRequests[0].pickup)
                        {
                            cage.MoveDown();
                        }
                        else if (cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor < cage.pickupRequests[0].pickup)
                        {
                            cage.MoveUp();
                        }
                        else if (cage.curFloor == cage.pickupRequests[0].pickup)
                        {
                            cage.OpenDoors();
                            cage.pickupRequests[0].status = "Destination";
                            cage.CleanUpRequests();
                        }
                    }
                    if(cage.pickupRequests.Count == 0 && cage.destinationRequests.Count != 0)
                    {
                        if(cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor > cage.destinationRequests[0].destination)
                        {
                            cage.MoveDown();
                        }
                        if(cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor < cage.destinationRequests[0].destination)
                        {
                            cage.MoveUp();
                        }
                        else if (cage.curFloor == cage.destinationRequests[0].destination)
                        {
                            Console.WriteLine(cage.curFloor + " is cage " + cage.id + " current floor and current destination is " + cage.destinationRequests[0].destination);
                            cage.OpenDoors();
                            cage.destinationRequests[0].status = "Completed";
                            cage.CleanUpRequests();
                        }
                    }
                }
            }
        }
        else
        {
            foreach(Column column in myCageManager.colList)
            {
                foreach(Cage cage in column.cages)
                {
                    if(cage.pickupRequests.Count != 0)
                    {
                        if(cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor > cage.pickupRequests[0].pickup)
                        {
                            cage.MoveDown();
                        }
                        else if (cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor < cage.pickupRequests[0].pickup)
                        {
                            cage.MoveUp();
                        }
                        else if (cage.curFloor == cage.pickupRequests[0].pickup)
                        {
                            cage.OpenDoors();
                            cage.pickupRequests[0].status = "Destination";
                            cage.CleanUpRequests();
                        }
                    }
                    if(cage.pickupRequests.Count == 0 && cage.destinationRequests.Count != 0)
                    {
                        if(cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor > cage.destinationRequests[0].destination)
                        {
                            cage.MoveDown();
                        }
                        if(cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor < cage.destinationRequests[0].destination)
                        {
                            cage.MoveUp();
                        }
                        else if (cage.curFloor == cage.destinationRequests[0].destination)
                        {
                            cage.OpenDoors();
                            cage.destinationRequests[0].status = "Completed";
                            cage.CleanUpRequests();
                        }
                    }
                }
            }
        }
    }

    static void CleanUpQueue()
    {
        for (int x = requestQueue.Count -1; x >= 0; x--)
        {
            if(requestQueue[x].status == "Completed")
            {
                requestQueue.Remove(requestQueue[x]);
            }
        }
    }

    static void LoopTest(Panel testPanel, CageManager testManager)
    {
        RequestGenerator(testPanel);
        AssignElevator(testManager);
        MoveElevators(testManager);
        CleanUpQueue();
    }

    static void Scenario1(Panel myPanel, CageManager myCageManager)
    {
        myCageManager.colList[1].cages[0].curFloor = 20;
        myCageManager.colList[1].cages[1].curFloor = 3;
        myCageManager.colList[1].cages[2].curFloor = 13;
        myCageManager.colList[1].cages[3].curFloor = 15;
        myCageManager.colList[1].cages[4].curFloor = 6;
        requestQueue.Add(new Request("Destination", 0, 5, "Down"));
        requestQueue[0].assignment = "Assigned";
        myCageManager.colList[1].cages[0].destinationRequests.Add(requestQueue[0]);
        requestQueue.Add(new Request("Destination", 0, 15, "Up"));
        requestQueue[1].assignment = "Assigned";
        myCageManager.colList[1].cages[1].destinationRequests.Add(requestQueue[1]);
        requestQueue.Add(new Request("Destination", 0, 1, "Down"));
        requestQueue[2].assignment = "Assigned";
        myCageManager.colList[1].cages[2].destinationRequests.Add(requestQueue[2]);
        requestQueue.Add(new Request("Destination", 0, 2, "Down"));
        requestQueue[3].assignment = "Assigned";
        myCageManager.colList[1].cages[3].destinationRequests.Add(requestQueue[3]);
        requestQueue.Add(new Request("Destination", 0, 1, "Down"));
        requestQueue[4].assignment = "Assigned";
        myCageManager.colList[1].cages[4].destinationRequests.Add(requestQueue[4]);
        LoopTest(myPanel, myCageManager);
        requestQueue.Add(new Request("Pickup", 1, 20, "Up"));
        while(requestQueue.Count > 0)
        {
            LoopTest(myPanel, myCageManager);
        }
        myCageManager.GetCageStatus();
    }

    static void Scenario2(Panel myPanel, CageManager myCageManager)
    {
        myCageManager.colList[2].cages[0].curFloor = 1;
        myCageManager.colList[2].cages[1].curFloor = 23;
        myCageManager.colList[2].cages[2].curFloor = 33;
        myCageManager.colList[2].cages[3].curFloor = 40;
        myCageManager.colList[2].cages[4].curFloor = 39;
        requestQueue.Add(new Request("Destination", 0, 21, "Up"));
        requestQueue[0].assignment = "Assigned";
        myCageManager.colList[2].cages[0].destinationRequests.Add(requestQueue[0]);
        requestQueue.Add(new Request("Destination", 0, 28, "Up"));
        requestQueue[1].assignment = "Assigned";
        myCageManager.colList[2].cages[1].destinationRequests.Add(requestQueue[1]);
        requestQueue.Add(new Request("Destination", 0, 1, "Down"));
        requestQueue[2].assignment = "Assigned";
        myCageManager.colList[2].cages[2].destinationRequests.Add(requestQueue[2]);
        requestQueue.Add(new Request("Destination", 0, 24, "Down"));
        requestQueue[3].assignment = "Assigned";
        myCageManager.colList[2].cages[3].destinationRequests.Add(requestQueue[3]);
        requestQueue.Add(new Request("Destination", 0, 1, "Down"));
        requestQueue[4].assignment = "Assigned";
        myCageManager.colList[2].cages[4].destinationRequests.Add(requestQueue[4]);
        requestQueue.Add(new Request("Pickup", 1, 36, "Up"));
        while(requestQueue.Count > 0)
        {
            LoopTest(myPanel, myCageManager);
        }
        myCageManager.GetCageStatus();
    }

    static void Scenario3(Panel myPanel, CageManager myCageManager)
    {
        myCageManager.colList[3].cages[0].curFloor = 58;
        myCageManager.colList[3].cages[1].curFloor = 50;
        myCageManager.colList[3].cages[2].curFloor = 46;
        myCageManager.colList[3].cages[3].curFloor = 1;
        myCageManager.colList[3].cages[4].curFloor = 60;
        requestQueue.Add(new Request("Destination", 0, 1, "Down"));
        requestQueue[0].assignment = "Assigned";
        myCageManager.colList[3].cages[0].destinationRequests.Add(requestQueue[0]);
        requestQueue.Add(new Request("Destination", 0, 60, "Up"));
        requestQueue[1].assignment = "Assigned";
        myCageManager.colList[3].cages[1].destinationRequests.Add(requestQueue[1]);
        requestQueue.Add(new Request("Destination", 0, 58, "Up"));
        requestQueue[2].assignment = "Assigned";
        myCageManager.colList[3].cages[2].destinationRequests.Add(requestQueue[2]);
        requestQueue.Add(new Request("Destination", 0, 54, "Up"));
        requestQueue[3].assignment = "Assigned";
        myCageManager.colList[3].cages[3].destinationRequests.Add(requestQueue[3]);
        requestQueue.Add(new Request("Destination", 0, 1, "Down"));
        requestQueue[4].assignment = "Assigned";
        myCageManager.colList[3].cages[4].destinationRequests.Add(requestQueue[4]);
        LoopTest(myPanel, myCageManager);
        requestQueue.Add(new Request("Pickup", 54, 1, "Down"));
        while(requestQueue.Count > 0)
        {
            LoopTest(myPanel, myCageManager);
        }
        myCageManager.GetCageStatus();
    }

    static void Scenario4(Panel myPanel, CageManager myCageManager)
    {
        myCageManager.colList[0].cages[0].curFloor = -4;
        myCageManager.colList[0].cages[1].curFloor = 1;
        myCageManager.colList[0].cages[2].curFloor = -3;
        myCageManager.colList[0].cages[3].curFloor = -6;
        myCageManager.colList[0].cages[4].curFloor = -1;
        myCageManager.colList[0].cages[2].status = "Loading";
        myCageManager.colList[0].cages[3].status = "Loading";
        myCageManager.colList[0].cages[4].status = "Loading";
        myCageManager.colList[0].cages[2].direction = "Down";
        myCageManager.colList[0].cages[3].direction = "Up";
        myCageManager.colList[0].cages[4].direction = "Down";
        requestQueue.Add(new Request("Destination", 0, -5, "Down"));
        requestQueue[0].assignment = "Assigned";
        myCageManager.colList[0].cages[2].destinationRequests.Add(requestQueue[0]);
        requestQueue.Add(new Request("Destination", 0, 1, "Up"));
        requestQueue[1].assignment = "Assigned";
        myCageManager.colList[0].cages[3].destinationRequests.Add(requestQueue[1]);
        requestQueue.Add(new Request("Destination", 0, -6, "Down"));
        requestQueue[2].assignment = "Assigned";
        myCageManager.colList[0].cages[4].destinationRequests.Add(requestQueue[2]);
        LoopTest(myPanel, myCageManager);
        requestQueue.Add(new Request("Pickup", -3, 1, "Up"));
        while(requestQueue.Count > 0)
        {
            LoopTest(myPanel, myCageManager);
        }
        myCageManager.GetCageStatus();
    }

    static void Main(string[] args)
    {
        bool useDemoConfig = true;
        ConsoleKeyInfo cki;
        do {
            Console.WriteLine("Use demo configuration? [y/n]");
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
                Configuration.Config();
                Configuration.GenerateFloors();
                CageManager userCageManager = new CageManager();
                Panel userPanel = new Panel();
                userCageManager.GetCageStatus();
                foreach(Column column in userCageManager.colList)
                {
                    string report = userCageManager.GetFloorsServed(column);
                    Console.WriteLine(report);
                }
                useDemoConfig = false;
                return;
            }
        } while(cki.Key != ConsoleKey.Y);

        // DEMO CONFIGURATION //
        if(useDemoConfig)
        {
            Configuration.batteryOn = true;
            Configuration.totalColumns = 4;
            Configuration.cagesPerColumn = 5;
            Configuration.totalFloors = 60;
            Configuration.totalBasements = 6;

            // CONFIRM DEMO SETUP //
            Console.WriteLine("\n-------HARDWARE SIMULATION-------");
            Console.WriteLine($"\n{"Hardware",-17} {"Value",15}\n");
            Console.WriteLine($"{"Battery", -17} {"On", 15}");
            Console.WriteLine($"{"Total Columns", -17} {Configuration.totalColumns, 15}");
            Console.WriteLine($"{"Cages Per Column", -17} {Configuration.cagesPerColumn, 15}");
            Console.WriteLine($"{"Total Floors", -17} {Configuration.totalFloors, 15}");
            Console.WriteLine($"{"Total Basements", -17} {Configuration.totalBasements, 15}");
        }
        
        // INSTANTIATE FLOORS //
        Configuration.GenerateFloors();

        // INSTANTIATE CAGEMANAGER //
        CageManager myCageManager = new CageManager();

        // INSTANTIATE PANEL //
        Panel myPanel = new Panel();

        while(Configuration.batteryOn)
        {
            int selection = Configuration.GetIntInput("\nPlease select a scenario\n(1,2,3,4 or 5 to EXIT)\n", 0);
            if(selection == 1)
            {
                Scenario1(myPanel, myCageManager);
            }
            else if (selection == 2)
            {
                Scenario2(myPanel, myCageManager);
            }
            else if (selection == 3)
            {
                Scenario3(myPanel, myCageManager);
            }
            else if (selection == 4)
            {
                Scenario4(myPanel, myCageManager);
            }
            else if (selection == 5)
            {
                Configuration.batteryOn = false;
            }
            else
            {
                Console.WriteLine(selection + " is not a valid selection. Please make a valid selection.");
            }
        }
    }
}

