///////////////////////////////////////////////////////
// {Commercial Controller}                          //
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
	"strings"
)

//////////////////
// Cage Manager //
//////////////////

type CageManager struct {
}

//////////////////////////
// System Configuration //
//////////////////////////

type Configuration struct {
	batteryOn      bool
	totalColumns   int
	cagesPerColumn int
	totalFloors    int
	totalBasements int
}

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

func initialize() {
	// Set total number of columns
	fmt.Printf("Enter the total number of columns ")
	var totalColumns int
	totalColumns, err := fmt.Scanln(&totalColumns)
	if err != nil {
		log.Fatal(err)
	}

	// Set cages per column
	fmt.Printf("How many cages are installed per column? ")
	var cagesPerColumn int
	cagesPerColumn, err1 := fmt.Scanln(&cagesPerColumn)
	if err1 != nil {
		log.Fatal(err)
	}

	// Set number of floors
	fmt.Printf("How many floors (excluding basements) are there in the building? ")
	var totalFloors int
	totalFloors, err2 := fmt.Scanln(&totalFloors)
	if err2 != nil {
		log.Fatal(err)
	}

	// Set number of basements
	fmt.Printf("How many basements are there? ")
	var totalBasements int
	totalBasements, err3 := fmt.Scanln(&totalBasements)
	if err3 != nil {
		log.Fatal(err)
	}

	// Confirm setup conditions
	fmt.Printf("\n-------HARDWARE SIMULATION-------\n")
	fmt.Printf("\n%-17v", "Hardware")
	fmt.Printf("%17v\n\n", "Value")
	fmt.Printf("%-17v", "Battery")
	fmt.Printf("%17v\n", "On")
	fmt.Printf("%-17v", "Total Columns")
	fmt.Printf("%17v\n", totalColumns)
	fmt.Printf("%-17v", "Cages Per Column")
	fmt.Printf("%17v\n", cagesPerColumn)
	fmt.Printf("%-17v", "Total Floor")
	fmt.Printf("%17v\n", totalFloors)
	fmt.Printf("%-17v", "Total Basements")
	fmt.Printf("%17v\n", totalBasements)
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
