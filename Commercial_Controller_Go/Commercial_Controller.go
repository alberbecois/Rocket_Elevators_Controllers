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
	"fmt"
	"log"
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

func askForConfirmation() bool {
	var response string
	_, err := fmt.Scanln(&response)
	if err != nil {
		log.Fatal(err)
	}
	okayResponses := []string{"y", "Y", "yes", "Yes", "YES"}
	nokayResponses := []string{"n", "N", "no", "No", "NO"}
	if containsString(okayResponses, response) {
		return true
	} else if containsString(nokayResponses, response) {
		return false
	} else {
		fmt.Println("Please type yes or no and then press enter:")
		return askForConfirmation()
	}
}

//////////
// Main //
//////////

func main() {
	fmt.Println("Hello, 世界 - お名前は？")
	var input string
	fmt.Scanln(&input)
	fmt.Println("Hello, " + input)

	fmt.Println("Activate battery? [y/n]")

	if askForConfirmation() == true {
		fmt.Println("Initializing...")
	} else {
		fmt.Println("Startup aborted!")
	}

}
