# Rocket Elevator Controllers v1.00

[CodeBoxx Technology School - Odyssey Week 3 Assignment.]

## Commercial Controllers
## Getting Started
### Requirements

* [.NET Framework](https://dotnet.microsoft.com/) - Needed to run the C# version
* [GO Lang](https://golang.org/) - For the current alpha of the Go version

## Running the tests

C# ONLY - On startup you will be asked if you want to run the demo setup or not. If you select no, you will be prompted to make your own hardware setup which will then generate the appropriate amount of floors columns etc based on your input before exiting the program. It is required to use the demo setup to run the test scenarios. When you select yes, the test environment of 4 columns, 5 cages per column, 60 floors and 6 basements will be generated. You will then be prompted to select a test scenario for simulation. 

GO ONLY - The scenarios are not presently functional in the Go version but you can still use the feature of generating your own hardware environment. The menu runs almost entirely the same. 

## Scenarios
### C#
```csharp
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
```

### Go
```go
func Scenario1(myPanel Panel, myCageManager CageManager) {
	myCageManager.colList[1].cages[0].curFloor = 20
	myCageManager.colList[1].cages[1].curFloor = 3
	myCageManager.colList[1].cages[2].curFloor = 13
	myCageManager.colList[1].cages[3].curFloor = 15
	myCageManager.colList[1].cages[4].curFloor = 6
	requestQueue = append(requestQueue, NewRequest("Destination", 0, 5, "Down"))
	requestQueue[0].assignment = "Assigned"
	myCageManager.colList[1].cages[0].destinationRequests = append(myCageManager.colList[1].cages[0].destinationRequests, requestQueue[0])
	requestQueue = append(requestQueue, NewRequest("Destination", 0, 15, "Up"))
	requestQueue[1].assignment = "Assigned"
	myCageManager.colList[1].cages[1].destinationRequests = append(myCageManager.colList[1].cages[1].destinationRequests, requestQueue[1])
	requestQueue = append(requestQueue, NewRequest("Destination", 0, 1, "Down"))
	requestQueue[2].assignment = "Assigned"
	myCageManager.colList[1].cages[2].destinationRequests = append(myCageManager.colList[1].cages[2].destinationRequests, requestQueue[2])
	requestQueue = append(requestQueue, NewRequest("Destination", 0, 2, "Down"))
	requestQueue[3].assignment = "Assigned"
	myCageManager.colList[1].cages[3].destinationRequests = append(myCageManager.colList[1].cages[3].destinationRequests, requestQueue[3])
	requestQueue = append(requestQueue, NewRequest("Destination", 0, 1, "Down"))
	requestQueue[4].assignment = "Assigned"
	myCageManager.colList[1].cages[4].destinationRequests = append(myCageManager.colList[1].cages[4].destinationRequests, requestQueue[4])
	LoopTest(myPanel, myCageManager)
	requestQueue = append(requestQueue, NewRequest("Pickup", 1, 20, "Up"))
	for len(requestQueue) > 0 {
		LoopTest(myPanel, myCageManager)
	}
	myCageManager.GetCageStatus()
}
```

The structure of the test scenarios differs a bit from the Residential version with the addition of a new LoopTest function which simulates the program running in a real environment and supports new requests made during the execution loop. The Go version has pointer errors making the Scenario loop infinitely at the moment but the structure is mostly the same. The same reporting functions from the Residential Controllers are present in these new Controllers, please review below for more information.

## Residential Controllers
## Getting Started
### Requirements

* [Python 3](https://www.python.org/) - To run the Python version
* [REPL.it](https://repl.it/) - To run the JavaScript version

## Running the tests

When the program is run, you will be asked to activate the battery. To begin, you must enter 'y' as the response. Anything else will abort the setup. Setup takes three inputs: The number of columns, the number of cages per column, and the number of floors. The appropriate number of each corresponding object will then be generated.

To create the conditions specified by the requirements please enter 1 for columns, 2 for cages per column, and 10 for floors. There are two hard coded scenarios which are provided for demonstration purposes, it is worth noting that they will run as long as there are a minimum of 1 column, 1 cage, and 10 floors.

After setup is complete you will be prompted to select a scenario to run. It is possible to modify the scenarios using the following methods:

## Scenarios
### Python
```python
def scenario1():
    cageManager.col_list[0].cages[0].curFloor = 1
    cageManager.col_list[0].cages[1].curFloor = 5
    cageManager.getCageStatus()
    floorList[2].buttons[0].callButtonPressed()
    cageManager.dispatchElevators()
    cageManager.getCageStatus()
    cageManager.col_list[0].cages[0].floorButtons[6].floorButtonPressed()
    cageManager.dispatchElevators()
    cageManager.getCageStatus()
```
### JavaScript
```javascript
function scenario1(){
    cageManager.col_list[0].cages[0].curFloor = 1;
    cageManager.col_list[0].cages[1].curFloor = 5;
    cageManager.getCageStatus();
    floorList[2].buttons[0].callButtonPressed();
    cageManager.dispatchElevators();
    cageManager.getCageStatus();
    cageManager.col_list[0].cages[0].floorButtons[6].floorButtonPressed();
    cageManager.dispatchElevators();
    cageManager.getCageStatus();
}
```

The first line sets the starting position of the 1st elevator in the 1st cage to floor 2 (counting from zero). The second line sets the 2nd elevator in the 1st cage to begin on floor 5. You can modify the values as long as your setup includes enough columns and elevators to accomodate your desired simulation.

The 4th line calls the UP button on the 3rd floor. The button in position 0 is always UP, position 1 is always DOWN. Whenever a call button is pressed, the request will be added to the closest available elevator request list based on elevator positions when the button is called. The dispatchElevators() function executes the current request list of all elevators in all columns.

## Reports
```python
getCageStatus()
```
Can be called at anytime from the cageManager object to get the position and status of all elevators in all cages.
```python
getCallButtonStatus()
```
Can be called at anytime from a floor object to get the status of all call buttons on that floor.
```python
getFloorButtonStatus()
```
Can be called at anytime from a cage object to get the status of all floor buttons in that cage.

## Notes
### Version Notes

1.00 - Launches support for Commercial Controllers on C# and Go (alpha). Residential Controllers in Javascript and Python

### JavaScript

It should be noted that as the prompt() function is used to get input for this version, it must be used in a REPL environment. Regular Node environments require a readline method.