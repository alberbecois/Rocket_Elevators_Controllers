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
	"strconv"
	"strings"
)

/////////////
// Columns //
/////////////
type Column struct {
	id 				int
	status			string
	cages			[]Cage
	floorsServed	[]int
}

///////////
// Cages //
///////////
type Cage struct {
	id						int
	status					string
	doors					string
	pickupRequests			[]Request
	destinationRequests		[]Request
	curFloor				int
	direction				string
	timer					int
	doorSensorStatus		string
}

/////////////
// Buttons //
/////////////
type CallButton struct {
	id		int
	status	string
}

type FloorButton struct {
	id		int
	status	string
}

///////////
// Panel //
///////////
type Panel struct {
	floorButtons	[]FloorButton
}

////////////
// Floors //
////////////
type Floor struct {
	id		int
	button	CallButton
}

//////////////
// Requests //
//////////////
type Request struct {
	status		string
	assignment	string
	pickup 		int
	destination int
	direction	string
}

//////////////////
// Cage Manager //
//////////////////

// CageManager should be instantiated once after Configuration is completed.
type CageManager struct {
	colList		Column
}

//////////////////////////
// System Configuration //
//////////////////////////

// Configuration should be run once on startup to generate a hardware simulation.
type Configuration struct {
	batteryOn      bool
	totalColumns   int
	cagesPerColumn int
	totalFloors    int
	totalBasements int
}

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
}

//////////
// Main //
//////////

func main() {
	/*fmt.Println("Hello, 世界 - お名前は？")
	var input string
	fmt.Scanln(&input)
	fmt.Println("Hello, " + input)*/

	if askForConfirmation("Activate battery?") == true {
		fmt.Println("Initializing...")
		initialize()
	} else {
		fmt.Println("Startup aborted!")
	}

}
