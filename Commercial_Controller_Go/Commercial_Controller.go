///////////////////////////////////////////////////////
// {Commercial Controller}                           //
///////////////////////////////////////////////////////
// Author: {Joshua Knutson}                          //
// License: {GNUGPLv3}                               //
// Link: {https://www.gnu.org/licenses/gpl-3.0.html} //
// Version: {0.01}                                   //
// Contact: {github.com/alberbecois}                 //
///////////////////////////////////////////////////////

package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"sort"
	"strconv"
	"strings"
	"time"
)

/////////////
// Columns //
/////////////

////////////////////////////////////////////////////////////////////////
// The column object has a list of cages as well as list of served    //
// floors.                                                            //
////////////////////////////////////////////////////////////////////////

// Column objects are generated by the CageManager with an even amount of floors per column whenever possible.
type Column struct {
	id           int
	status       string
	cages        []Cage
	floorsServed []int
}

// NewColumn is a Column factory function (constructor)
func NewColumn(id int, status string, cages []Cage, floorsServed []int) Column {
	c := Column{}
	c.id = id
	c.status = status
	c.cages = cages
	c.floorsServed = floorsServed
	return c
}

///////////
// Cages //
///////////

////////////////////////////////////////////////////////////////////////
// Each Column object has a list of Cage objects which contain all    //
// the necessary methods for moving up and down as well as a list of  //
// pickup requests.                                                   //
////////////////////////////////////////////////////////////////////////

// Cage objects are generated as a list in the Column object.
type Cage struct {
	id                  int
	status              string
	doors               string
	pickupRequests      []Request
	destinationRequests []Request
	curFloor            int
	direction           string
	timer               int
	doorSensorStatus    string
}

// NewCage is a Cage factory function (constructor)
func NewCage(id int, status string, doors string) Cage {
	c := Cage{}
	c.id = id
	c.status = status
	c.doors = doors
	c.curFloor = 1
	c.direction = "Up"
	c.timer = 0
	c.doorSensorStatus = "Clear"
	return c
}

// CleanUpRequests loops through the cages request queue updating and removing items as necessary
func (c *Cage) CleanUpRequests() {
	for i := (len(c.pickupRequests) - 1); i >= 0; i-- {
		if c.curFloor == c.pickupRequests[i].pickup {
			c.pickupRequests[i].status = "Destination"
		}
		if c.pickupRequests[i].status == "Destination" {
			c.destinationRequests = append(c.destinationRequests, c.pickupRequests[i])
			c.pickupRequests = append(c.pickupRequests[:i], c.pickupRequests[i+1:]...)
			readout := "Destination is now " + strconv.Itoa(c.destinationRequests[i].destination)
			fmt.Println(readout)
		}
	}
	for i := (len(c.destinationRequests) - 1); i >= 0; i-- {
		if c.curFloor == c.destinationRequests[i].destination {
			c.destinationRequests[i].status = "Completed"
		}
		if c.destinationRequests[i].status == "Completed" {
			c.destinationRequests = append(c.destinationRequests[:i], c.destinationRequests[i+1:]...)
		}
	}
}

// DOOR METHODS //

// OpenDoors opens the cage doors for 8 seconds
func (c *Cage) OpenDoors() {
	if c.status != "In-Service" {
		c.doors = "Open"
		message := "Cage " + strconv.Itoa(c.id) + " doors are open for 8 seconds"
		fmt.Println(message)
		c.timer = 8
		for c.timer > 0 {
			alert := "Closing in " + strconv.Itoa(c.timer) + " seconds."
			fmt.Println(alert)
			time.Sleep(1 * time.Second)
			c.timer--
		}
		c.CloseDoors()
	}
}

// OpenButtonPressed opens doors if cage is not between floors
func (c *Cage) OpenButtonPressed() {
	if c.status != "In-Service" {
		c.OpenDoors()
	}
}

// CloseDoors closes the cage doors
func (c *Cage) CloseDoors() {
	if c.doorSensorStatus == "Clear" && c.timer < 5 {
		c.doors = "Closed"
		fmt.Println("Cage doors are closed")
		if len(c.pickupRequests) == 0 && len(c.destinationRequests) == 0 {
			c.status = "Idle"
		} else {
			c.status = "Loading"
		}
	}
}

// CloseButtonPressed closes the doors if they have been open at least a few seconds
func (c *Cage) CloseButtonPressed() {
	if c.timer < 5 {
		c.CloseDoors()
	}
}

// MOVEMENT //

// MoveDown moves the cage down one floor
func (c *Cage) MoveDown() {
	for c.doors != "Closed" {
		c.CloseDoors()
	}
	c.status = "In-Service"
	c.direction = "Down"
	alert := "Cage " + strconv.Itoa(c.id) + " going down at " + strconv.Itoa(c.curFloor)
	fmt.Println(alert)
	if c.curFloor-1 == 0 {
		c.curFloor -= 2
	} else {
		c.curFloor--
	}
	alert2 := "Cage " + strconv.Itoa(c.id) + " at " + strconv.Itoa(c.curFloor)
	fmt.Println(alert2)
	c.status = "Loading"
}

// MoveUp moves the cage up one floor
func (c *Cage) MoveUp() {
	for c.doors != "Closed" {
		c.CloseDoors()
	}
	c.status = "In-Service"
	c.direction = "Up"
	alert := "Cage " + strconv.Itoa(c.id) + " going up at " + strconv.Itoa(c.curFloor)
	fmt.Println(alert)
	if c.curFloor+1 == 0 {
		c.curFloor += 2
	} else {
		c.curFloor++
	}
	alert2 := "Cage " + strconv.Itoa(c.id) + " at " + strconv.Itoa(c.curFloor)
	fmt.Println(alert2)
	c.status = "Loading"
}

/////////////
// Buttons //
/////////////

////////////////////////////////////////////////////////////////////////
// Buttons are used to generate pickup requests from either the lobby //
// panel or from a given floor.                                       //
////////////////////////////////////////////////////////////////////////

// A CallButton is generated per Floor object
type CallButton struct {
	id     int
	status string
}

// NewCallButton is a CallButton factory function (constructor)
func NewCallButton(id int, status string) CallButton {
	b := CallButton{}
	b.id = id
	b.status = status
	return b
}

// CallButtonPressed sets the status of a CallButton to "Active"
func (b *CallButton) CallButtonPressed() {
	b.status = "Active"
}

// A FloorButton per Floor object is instantiated by the Panel.
type FloorButton struct {
	id     int
	status string
}

// NewFloorButton is a FloorButton factory function (constructor)
func NewFloorButton(id int, status string) FloorButton {
	b := FloorButton{}
	b.id = id
	b.status = status
	return b
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

// A Panel should be instantiated after the CageManager.
type Panel struct {
	floorButtons []FloorButton
}

// NewPanel is a Panel factory function (constructor)
func NewPanel() Panel {
	p := Panel{}
	for i := 0 - myConfiguration.totalBasements; i < 0; i++ {
		p.floorButtons = append(p.floorButtons, NewFloorButton(i, "Inactive"))
	}
	for i := 1; i <= myConfiguration.totalFloors; i++ {
		p.floorButtons = append(p.floorButtons, NewFloorButton(i, "Inactive"))
	}
	return p
}

// METHODS //

// RequestElevator sets the requested floorbutton to "Active" status and advises which column to use
func (p *Panel) RequestElevator(floorNum int, c CageManager) {
	for _, button := range p.floorButtons {
		if button.id == floorNum {
			button.status = "Active"
		}
	}
	var myColumn = c.GetColumn(1, floorNum)
	pResponse := "Floor requested. Please proceed to column " + strconv.Itoa(myColumn.id)
	fmt.Println(pResponse)
}

// REPORTS //

// GetFloorButtonStatus prints out the status of all FloorButtons in the panel
func (p Panel) GetFloorButtonStatus() {
	for _, fb := range p.floorButtons {
		response := "Floor " + strconv.Itoa(fb.id) + " button is " + fb.status
		fmt.Println(response)
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

// Floor objects are generated by the Configuration object equal to the number of floors.
type Floor struct {
	id     int
	button CallButton
}

// NewFloor is a Floor factory function (constructor)
func NewFloor(id int, button CallButton) Floor {
	f := Floor{}
	f.id = id
	f.button = button
	return f
}

//////////////
// Requests //
//////////////

////////////////////////////////////////////////////////////////////////
// A request object is generated each time a Floor or Call button is  //
// pressed. The request is queued by the main program before being    //
// assigned to a cage for treatment.                                  //
////////////////////////////////////////////////////////////////////////

// Request objects are instantiated whenever a button is pressed.
type Request struct {
	status      string
	assignment  string
	pickup      int
	destination int
	direction   string
}

// NewRequest is a Request factory function (constructor)
func NewRequest(status string, pickup int, destination int, direction string) Request {
	r := Request{}
	r.status = status
	r.assignment = "Unassigned"
	r.pickup = pickup
	r.destination = destination
	r.direction = direction
	return r
}

//////////////////
// Cage Manager //
//////////////////

////////////////////////////////////////////////////////////////////////
// This object contains all the column and cage objects in the system //
// Only one CageManager should instantiated and only after Config has //
// been called during the initial setup.                              //
////////////////////////////////////////////////////////////////////////

// CageManager should be instantiated once after Configuration is completed.
type CageManager struct {
	colList []Column
}

// NewCageManager is a CageManager factory function (constructor)
func NewCageManager() CageManager {
	c := CageManager{}
	var floorRange int
	var extraFloors int
	var floorCounter int
	if myConfiguration.totalBasements > 0 {
		if (myConfiguration.totalFloors-1)%(myConfiguration.totalColumns-1) != 0 {
			floorRange = (myConfiguration.totalFloors - 1) / (myConfiguration.totalColumns - 1)
			extraFloors = (myConfiguration.totalFloors - 1) % (myConfiguration.totalColumns - 1)
		} else {
			floorRange = (myConfiguration.totalFloors - 1) / (myConfiguration.totalColumns - 1)
			extraFloors = 0
		}
		floorCounter = 2
		var bColumnFloors []int
		for i := 0; i < myConfiguration.totalBasements; i++ {
			if myConfiguration.floorList[i].id < 0 {
				bColumnFloors = append(bColumnFloors, myConfiguration.floorList[i].id)
			}
		}
		bColumnFloors = append(bColumnFloors, 1)
		c.colList = append(c.colList, NewColumn(1, "Active", c.GenerateCages(myConfiguration.cagesPerColumn), bColumnFloors))
		for i := 2; i <= myConfiguration.totalColumns; i++ {
			var floorsServed []int
			floorsServed = append(floorsServed, 1)
			if myConfiguration.totalColumns-i < extraFloors {
				for j := 0; j < floorRange+1; j++ {
					floorsServed = append(floorsServed, floorCounter)
					floorCounter++
				}
				c.colList = append(c.colList, NewColumn(i, "Active", c.GenerateCages(myConfiguration.cagesPerColumn), floorsServed))
			} else {
				for j := 0; j < floorRange; j++ {
					floorsServed = append(floorsServed, floorCounter)
					floorCounter++
				}
				c.colList = append(c.colList, NewColumn(i, "Active", c.GenerateCages(myConfiguration.cagesPerColumn), floorsServed))
			}
		}
	} else {
		if myConfiguration.totalFloors%myConfiguration.totalColumns != 0 {
			floorRange = myConfiguration.totalFloors / myConfiguration.totalColumns
			extraFloors = myConfiguration.totalFloors % myConfiguration.totalColumns
		} else {
			floorRange = myConfiguration.totalFloors / myConfiguration.totalColumns
			extraFloors = 0
		}
		floorCounter = 2
		for i := 1; i <= myConfiguration.totalColumns; i++ {
			var floorsServed []int
			floorsServed = append(floorsServed, 1)
			for j := 0; j < floorRange; j++ {
				floorsServed = append(floorsServed, floorCounter)
				floorCounter++
			}
			c.colList = append(c.colList, NewColumn(i, "Active", c.GenerateCages(myConfiguration.cagesPerColumn), floorsServed))
		}
	}
	return c
}

// METHODS //

// GetCage returns an available cage to fulfill a pickup request
func (c *CageManager) GetCage(direction string, column int, reqFloor int) *Cage {
	var curCage = c.colList[column].cages[0]
	var bestCage = c.colList[column].cages[0]
	x := 0
	for x < len(c.colList[column].cages) {
		curCage = c.colList[column].cages[x]
		if curCage.direction == direction && direction == "Up" && curCage.curFloor < reqFloor && curCage.status != "Idle" {
			fmt.Println("Same direction UP was selected")
			return &curCage // Returns the cage already going the same direction (UP) that has not yet passed the requested floor
		} else if curCage.direction == direction && direction == "Down" && curCage.curFloor > reqFloor && curCage.status != "Idle" {
			fmt.Println("Same direction DOWN was selected")
			return &curCage // Returns the cage already going the same direction (DOWN) that has not yet passed the requested floor
		} else if curCage.status == "Idle" {
			allCagesAreIdle := true
			for i := 0; i < len(c.colList[column].cages); i++ {
				if c.colList[column].cages[i].status != "Idle" {
					allCagesAreIdle = false
				}
				if allCagesAreIdle {
					for i := x + 1; i < len(c.colList[column].cages); i++ {
						var compareCage = c.colList[column].cages[i]
						if compareCage.status == "Idle" {
							fmt.Println("Idle cage gap comparison is selected")
							var gapA = Abs(bestCage.curFloor - reqFloor)
							var gapB = Abs(compareCage.curFloor - reqFloor)
							if gapB < gapA {
								bestCage = compareCage // Closest idle cage
							}
						}
						comment := "Cage " + strconv.Itoa(curCage.id) + " is selected."
						fmt.Println(comment)
					}
					return &bestCage
				}
			}
		} else {
			for i := 0; i < len(c.colList[column].cages); i++ {
				if direction == "Up" && len(c.colList[column].cages[i].destinationRequests) < len(curCage.destinationRequests) {
					curCage = c.colList[column].cages[i]
				} else if direction == "Down" && len(c.colList[column].cages[i].pickupRequests) < len(curCage.pickupRequests) {
					curCage = c.colList[column].cages[i]
				}
			}
			fmt.Println("Least occupied cage is selected")
		}
		x++
	}
	return &curCage
}

// GetColumn returns the column serving the requested floor
func (c *CageManager) GetColumn(pickup int, destination int) *Column {
	pickupServed := false
	destServed := false
	for _, column := range c.colList {
		for _, id := range column.floorsServed {
			if id == pickup {
				pickupServed = true
			}
			if id == destination {
				destServed = true
			}
			if pickupServed && destServed {
				return &column
			}
		}
	}
	return nil
}

// GenerateCages makes a list of Cages for other factory functions
func (c CageManager) GenerateCages(numCages int) []Cage {
	var cageList []Cage
	for i := 0; i <= numCages; i++ {
		cageList = append(cageList, NewCage(i, "Idle", "Closed"))
	}
	return cageList
}

// REPORTS //

// GetCageStatus provides a read out of each cage location and status
func (c CageManager) GetCageStatus() {
	for i := 0; i < len(c.colList); i++ {
		for j := 0; j < len(c.colList[i].cages); j++ {
			var curCage Cage = c.colList[i].cages[j]
			fmt.Println("Column ", c.colList[i].id, ": Cage ", curCage.id, " is ", curCage.status)
			fmt.Println("Current floor: ", curCage.curFloor, " Door status: ", curCage.doors)
		}
	}
}

// GetFloorsServed returns a string of the floors served by a given column
func (c CageManager) GetFloorsServed(column Column) string {
	var myFloors []string
	for _, floor := range column.floorsServed {
		myFloors = append(myFloors, strconv.Itoa(floor))
	}
	floorString := strings.Join(myFloors, ",")
	colString := strconv.Itoa(column.id)
	myString := "Column " + colString + ": " + floorString
	return myString
}

//////////////////////////
// System Configuration //
//////////////////////////

////////////////////////////////////////////////////////////////////////
// This static object generates a hardware configuration from user    //
// input and the corresponding floor list.                            //
////////////////////////////////////////////////////////////////////////

// Configuration should be run once on startup to generate a hardware simulation.
type Configuration struct {
	batteryOn      bool
	totalColumns   int
	cagesPerColumn int
	totalFloors    int
	totalBasements int
	floorList      []Floor
}

var myConfiguration = Configuration{}

// GenerateFloors is to be called after Config: Generates Floor structs and adds them to the floorList
func (c *Configuration) GenerateFloors() {
	// Checks if building has basements to add to the floor list
	if c.totalBasements > 0 {
		for i := 0 - c.totalBasements; i < 0; i++ {
			c.floorList = append(c.floorList, NewFloor(i, NewCallButton(i, "Inactive")))
		}
	}
	// Adds remaining floors
	for i := 1; i < 1+c.totalFloors; i++ {
		c.floorList = append(c.floorList, NewFloor(i, NewCallButton(i, "Inactive")))
	}
}

// GetFloorStatus prints out the call status of all active floors
func (c Configuration) GetFloorStatus() {
	fmt.Println("\n-----------------FLOOR STATUS------------------")
	for i := 0; i < len(c.floorList); i++ {
		idstring := strconv.Itoa(c.floorList[i].id)
		s := "Floor " + idstring + ":  Active  //  Call Status: " + c.floorList[i].button.status
		fmt.Println(s)
	}
}

// Global RequestQueue //
var requestQueue []Request

///////////////
// Functions //
///////////////

// Gets a y or n response from the user
func askForConfirmation(s string) bool {
	reader := bufio.NewReader(os.Stdin)

	for {
		fmt.Printf("%s [y/n]: ", s)

		response, err := reader.ReadString('\n')
		if err != nil {
			log.Fatal(err)
		}

		response = strings.ToLower(strings.TrimSpace(response))

		if response == "y" || response == "yes" {
			return true
		} else if response == "n" || response == "no" {
			return false
		} else {
			fmt.Printf(response + " is not a valid selection\n")
		}
	}
}

// Abs returns the absolute value of x.
func Abs(x int) int {
	if x < 0 {
		return -x
	}
	return x
}

// Gets positive integer values from the user
func takeIntInput(s string) int {
	reader := bufio.NewReader(os.Stdin)

	for {
		fmt.Printf("%s: ", s)

		input, err := reader.ReadString('\n')
		if err != nil {
			log.Fatal(err)
		}
		cleanedInput := strings.Replace(input, "\r\n", "", -1)
		myInt, err1 := strconv.Atoi(cleanedInput)
		if err1 != nil {
			fmt.Printf(cleanedInput + " is not a valid number. Please enter a valid number.\n")
		} else if myInt < 0 {
			fmt.Printf("Value cannot be less than zero. Please enter a valid number.\n")
		} else {
			return myInt
		}
	}
}

// Performs initial setup for the hardware simulation
func initialize() {
	// Set total number of columns
	totalColumns := takeIntInput("Enter the total number of columns")

	// Set cages per column
	cagesPerColumn := takeIntInput("How many cages are installed per column?")

	// Set number of floors
	totalFloors := takeIntInput("How many floors (excluding basements) are there in the building?")

	// Set number of basements
	totalBasements := takeIntInput("How many basements are there? ")

	// Confirm setup conditions
	fmt.Printf("\n-------HARDWARE SIMULATION-------\n")
	fmt.Printf("\n%-17v", "Hardware")
	fmt.Printf("%15v\n\n", "Value")
	fmt.Printf("%-17v", "Battery")
	fmt.Printf("%15v\n", "On")
	fmt.Printf("%-17v", "Total Columns")
	fmt.Printf("%15v\n", totalColumns)
	fmt.Printf("%-17v", "Cages Per Column")
	fmt.Printf("%15v\n", cagesPerColumn)
	fmt.Printf("%-17v", "Total Floor")
	fmt.Printf("%15v\n", totalFloors)
	fmt.Printf("%-17v", "Total Basements")
	fmt.Printf("%15v\n", totalBasements)

	// Set configuration values
	myConfiguration.batteryOn = true
	myConfiguration.totalColumns = totalColumns
	myConfiguration.cagesPerColumn = cagesPerColumn
	myConfiguration.totalFloors = totalFloors
	myConfiguration.totalBasements = totalBasements
}

// RequestGenerator checks all buttons and adds requests to the queue
func RequestGenerator(myPanel Panel) {
	for f := range myConfiguration.floorList {
		floor := &myConfiguration.floorList[f]
		if floor.button.status == "Active" {
			alert := "Floor button " + strconv.Itoa(floor.button.id) + " is active."
			fmt.Println(alert)
			if floor.id > 0 {
				var myRequest = NewRequest("Pickup", floor.button.id, 1, "Down")
				for _, request := range requestQueue {
					if myRequest.pickup == request.pickup && request.status == "Pickup" {
						warning := "My request for floor " + strconv.Itoa(floor.button.id) + " was not sent."
						fmt.Println(warning)
						return
					}
				}
				requestQueue = append(requestQueue, myRequest)
				notice := "My request for floor " + strconv.Itoa(myRequest.pickup) + " was added to the request list"
				fmt.Println(notice)
			} else {
				var myRequest = NewRequest("Pickup", floor.id, 1, "Up")
				for _, request := range requestQueue {
					if myRequest.pickup == request.pickup && request.status == "Pickup" {
						warning := "My request for floor " + strconv.Itoa(floor.button.id) + " was not sent."
						fmt.Println(warning)
						return
					}
				}
				requestQueue = append(requestQueue, myRequest)
				notice := "My request for floor " + strconv.Itoa(myRequest.pickup) + " was added to the request list"
				fmt.Println(notice)
			}
			floor.button.status = "Inactive"
			alert2 := "Floor " + strconv.Itoa(floor.button.id) + " is " + floor.button.status
			fmt.Println(alert2)
		}
	}
	for f := range myPanel.floorButtons {
		button := myPanel.floorButtons[f]
		if button.status == "Active" {
			alert := "Panel button " + strconv.Itoa(button.id) + " is " + button.status
			fmt.Println(alert)
			if button.id > 0 {
				var myRequest = NewRequest("Pickup", 1, button.id, "Up")
				for _, request := range requestQueue {
					if myRequest.pickup == request.pickup && request.status == "Pickup" {
						warning := "My request for floor " + strconv.Itoa(button.id) + " was not sent."
						fmt.Println(warning)
						return
					}
				}
				requestQueue = append(requestQueue, myRequest)
				notice := "My request for floor " + strconv.Itoa(myRequest.pickup) + " was added to the request list"
				fmt.Println(notice)
			} else {
				var myRequest = NewRequest("Pickup", 1, button.id, "Down")
				for _, request := range requestQueue {
					if myRequest.pickup == request.pickup && request.status == "Pickup" {
						warning := "My request for floor " + strconv.Itoa(button.id) + " was not sent."
						fmt.Println(warning)
						return
					}
				}
				requestQueue = append(requestQueue, myRequest)
				notice := "My request for floor " + strconv.Itoa(myRequest.pickup) + " was added to the request list"
				fmt.Println(notice)
			}
		}
		button.status = "Inactive"
		alert2 := "Floor " + strconv.Itoa(button.id) + " is " + button.status
		fmt.Println(alert2)
	}
}

// AssignElevator assigns each request to an elevator
func AssignElevator(myCageManager CageManager) {
	for r := range requestQueue {
		request := requestQueue[r]
		if request.assignment == "Unassigned" {
			var myColumn = myCageManager.GetColumn(request.pickup, request.destination)
			m1 := "Column " + strconv.Itoa(myColumn.id) + " is selected."
			fmt.Println(m1)
			var myCage = myCageManager.GetCage(request.direction, myColumn.id-1, request.pickup)
			request.assignment = "Assigned"
			myCage.pickupRequests = append(myCage.pickupRequests, request)
			m2 := "Cage " + strconv.Itoa(myCage.id) + " receives request for floor " + strconv.Itoa(myCage.pickupRequests[0].pickup)
			fmt.Println(m2)
			sort.SliceStable(myCage.pickupRequests, func(i, j int) bool {
				return myCage.pickupRequests[i].pickup < myCage.pickupRequests[j].pickup
			})
		}
	}
}

// MoveElevators moves all elevators towards next destination or pickup
func MoveElevators(myCageManager CageManager) {
	if myConfiguration.totalBasements > 0 {
		for c := range myCageManager.colList[0].cages {
			cage := &myCageManager.colList[0].cages[c]
			if len(cage.pickupRequests) != 0 {
				if cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor > cage.pickupRequests[0].pickup {
					cage.MoveDown()
				} else if cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor < cage.pickupRequests[0].pickup {
					cage.MoveUp()
				} else if cage.curFloor == cage.pickupRequests[0].pickup {
					cage.OpenDoors()
					cage.pickupRequests[0].status = "Destination"
					cage.CleanUpRequests()
				}
			}
			if len(cage.pickupRequests) == 0 && len(cage.destinationRequests) != 0 {
				if cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor > cage.destinationRequests[0].destination {
					cage.MoveDown()
				} else if cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor < cage.destinationRequests[0].destination {
					cage.MoveUp()
				} else if cage.curFloor == cage.destinationRequests[0].destination {
					cage.OpenDoors()
					cage.destinationRequests[0].status = "Completed"
					cage.CleanUpRequests()
				}
			}
		}
		for i := 1; i < len(myCageManager.colList); i++ {
			for c := range myCageManager.colList[i].cages {
				cage := &myCageManager.colList[i].cages[c]
				if len(cage.pickupRequests) != 0 {
					if cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor > cage.pickupRequests[0].pickup {
						cage.MoveDown()
					} else if cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor < cage.pickupRequests[0].pickup {
						cage.MoveUp()
					} else if cage.curFloor == cage.pickupRequests[0].pickup {
						cage.OpenDoors()
						cage.pickupRequests[0].status = "Destination"
						cage.CleanUpRequests()
					}
					if len(cage.pickupRequests) == 0 && len(cage.destinationRequests) != 0 {
						if cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor > cage.destinationRequests[0].destination {
							cage.MoveDown()
						} else if cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor < cage.destinationRequests[0].destination {
							cage.MoveUp()
						} else if cage.curFloor == cage.destinationRequests[0].destination {
							cage.OpenDoors()
							cage.destinationRequests[0].status = "Completed"
							cage.CleanUpRequests()
						}
					}
				}
			}
		}
	} else {
		for col := range myCageManager.colList {
			column := &myCageManager.colList[col]
			for c := range column.cages {
				cage := column.cages[c]
				if len(cage.pickupRequests) != 0 {
					if cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor > cage.pickupRequests[0].pickup {
						cage.MoveDown()
					} else if cage.curFloor != cage.pickupRequests[0].pickup && cage.curFloor < cage.pickupRequests[0].pickup {
						cage.MoveUp()
					} else if cage.curFloor == cage.pickupRequests[0].pickup {
						cage.OpenDoors()
						cage.pickupRequests[0].status = "Destination"
						cage.CleanUpRequests()
					}
					if len(cage.pickupRequests) == 0 && len(cage.destinationRequests) != 0 {
						if cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor > cage.destinationRequests[0].destination {
							cage.MoveDown()
						} else if cage.curFloor != cage.destinationRequests[0].destination && cage.curFloor < cage.destinationRequests[0].destination {
							cage.MoveUp()
						} else if cage.curFloor == cage.destinationRequests[0].destination {
							cage.OpenDoors()
							cage.destinationRequests[0].status = "Completed"
							cage.CleanUpRequests()
						}
					}
				}
			}
		}
	}
}

// CleanUpQueue checks the requestQueue for requests that are Completed and removes them
func CleanUpQueue() {
	for i := len(requestQueue) - 1; i >= 0; i-- {
		if requestQueue[i].status == "Completed" {
			requestQueue = append(requestQueue[:i], requestQueue[i+1:]...)
		}
	}
}

// LoopTest simulates one loop of the main program
func LoopTest(myPanel Panel, myCageManager CageManager) {
	RequestGenerator(myPanel)
	AssignElevator(myCageManager)
	MoveElevators(myCageManager)
	CleanUpQueue()
}

// Scenario1 simulates the first scenario
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

//////////
// Main //
//////////

func main() {

	if askForConfirmation("Use demo configuration? [y/n]") == true {
		myConfiguration.batteryOn = true
		myConfiguration.totalColumns = 4
		myConfiguration.cagesPerColumn = 5
		myConfiguration.totalFloors = 60
		myConfiguration.totalBasements = 6
		// Confirm setup conditions
		fmt.Printf("\n-------HARDWARE SIMULATION-------\n")
		fmt.Printf("\n%-17v", "Hardware")
		fmt.Printf("%15v\n\n", "Value")
		fmt.Printf("%-17v", "Battery")
		fmt.Printf("%15v\n", "On")
		fmt.Printf("%-17v", "Total Columns")
		fmt.Printf("%15v\n", myConfiguration.totalColumns)
		fmt.Printf("%-17v", "Cages Per Column")
		fmt.Printf("%15v\n", myConfiguration.cagesPerColumn)
		fmt.Printf("%-17v", "Total Floor")
		fmt.Printf("%15v\n", myConfiguration.totalFloors)
		fmt.Printf("%-17v", "Total Basements")
		fmt.Printf("%15v\n", myConfiguration.totalBasements)
		// Instantiate floors
		myConfiguration.GenerateFloors()
		var myCageManager = NewCageManager()
		// Instantiate Panel
		var myPanel = NewPanel()
		myConfiguration.GetFloorStatus()
		myCageManager.GetCageStatus()
		for _, column := range myCageManager.colList {
			report := myCageManager.GetFloorsServed(column)
			fmt.Println(report)
		}
		for myConfiguration.batteryOn {
			selection := takeIntInput("\nPlease select a scenario\n(1,2,3,4 or 5 to EXIT)\n")
			if selection == 1 {
				Scenario1(myPanel, myCageManager)
			} else if selection == 2 {
				fmt.Println("Scenario 2")
			} else if selection == 3 {
				fmt.Println("Scenario 3")
			} else if selection == 4 {
				fmt.Println("Scenario 4")
			} else if selection == 5 {
				myConfiguration.batteryOn = false
			} else {
				alert := strconv.Itoa(selection) + " is not a valid selection. Please make a valid selection."
				fmt.Println(alert)
			}
		}
	} else {
		if askForConfirmation("Activate battery?") == true {
			fmt.Println("Initializing...")
			initialize()
			// Instantiate floors
			myConfiguration.GenerateFloors()
			var myCageManager = NewCageManager()
			// var myPanel = NewPanel()
			myConfiguration.GetFloorStatus()
			myCageManager.GetCageStatus()
			for _, column := range myCageManager.colList {
				report := myCageManager.GetFloorsServed(column)
				fmt.Println(report)
			}
		} else {
			fmt.Println("Startup aborted!")
		}
	}

}
