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
	reader := bufio.NewReader(os.Stdin)
	var totalColumns int
	// Set total number of columns
	fmt.Printf("Enter the total number of columns")

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
