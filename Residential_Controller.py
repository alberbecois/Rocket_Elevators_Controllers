#######################################################
## {Residential Controller}                          ##
#######################################################
## Author: {Joshua Knutson}                          ##
## License: {GNUGPLv3}                               ##
## Link: {https://www.gnu.org/licenses/gpl-3.0.html} ##
## Version: {0.01}                                   ##
## Contact: {github.com/alberbecois}                 ##
#######################################################


######################
## Global Variables ##
######################
battery_on = False
total_columns = None
cages_per_column = None
total_floors = None
cageManager = None
floorList = []
import time

#############
## Columns ##
#############
class Column:
    def __init__(self, status, cages):
        self.status = status
        self.cages = cages


###########
## Cages ##
###########
class Cage:
    def __init__(self, status, doors, floorButtons):
        self.status = status
        self.doors = doors
        self.floorButtons = floorButtons
        self.requests = []
        self.curFloor = 0
        self.direction = "Up"
        self.timer = 0
        self.door_sensor_status = "Clear"

        ## Door Methods ##
        def openDoors(self):
            if self.status == "Loading":
                self.doors = "Open"
                self.timer = 8
                for x in floorList[self.curFloor].buttons:
                    x.status = "Inactive"
                while self.timer > 0:
                    self.timer -= 1
                    time.sleep(1)
                self.closeDoors()
        
        def openButtonPressed(self):
            if self.status != "In-Service":
                self.openDoors()

        def closeDoors(self):
            if self.door_sensor_status == "Clear":
                self.doors = "Closed"
                self.status = "Loading"
        
        def closeButtonPressed(self):
            if self.timer < 5:
                self.closeDoors()

        ## Movement ##
        def moveDown(self, requestedFloor):
            if self.doors != "Closed":
                self.closeDoors()
            else:
                self.status = "In-Service"
                self.direction = "Down"
                while self.curFloor != requestedFloor:
                    self.curFloor -= 1
                self.status = "Loading"
                self.openDoors()
        
        def moveUp(self, requestedFloor):
            if self.doors != "Closed":
                self.closeDoors()
            else:
                self.status = "In-Service"
                self.direction = "Up"
                while self.curFloor != requestedFloor:
                    self.curFloor += 1
                self.status = "Loading"
                self.openDoors()
        
        
#############
## Buttons ##
#############
class CallButton:
    def __init__(self, direction, column, floor):
        self.direction = direction
        self.column = column
        self.floor = floor
        self.status = "Inactive"

    def requestPickup(self, direction, column, floor):
        cage = cageManager.getAvailableCage(direction, column, floor)
        cageManager.requestElevator(cage, floor)
    
    def callButtonPressed(self):
        self.status = "Active"
        self.requestPickup(self.direction, self.column, self.floor)

class FloorButton:
    def __init__(self, floor):
        self.floorNum = floor
        self.status = "Inactive"


############
## Floors ##
############
class Floor:
    def __init__(self, number, buttons):
        self.number = number
        self.buttons = buttons

    ## Reports ##
    def getCallButtonStatus(self):
        for i in range(0, len(self.buttons)):
            print(self.buttons[i].direction + " button is " + self.buttons[i].status)

##################
## Cage Manager ##
##################
class CageManager:
    def __init__(self):
        self.col_list = []
     
    ## Methods ##
    def getAvailableCage(self, direction, column, reqFloor):
        for i in range(0, len(self.col_list[column].cages)):
            if self.col_list[column].cages[i].direction == direction and direction == "Up" and self.col_list[column].cages[i].curFloor < reqFloor:
                return self.col_list[column].cages[i] # Going same direction (UP) before requested floor
            elif self.col_list[column].cages[i].direction == direction and direction == "Down" and self.col_list[column].cages[i].curFloor > reqFloor:
                return self.col_list[column].cages[i] # Going same direction (DOWN) before requested floor
            elif self.col_list[column].cages[i].status == "Idle":
                return self.col_list[column].cages[i] # Return an unoccupied cage
            else:
                cage = self.col_list[column].cages[i] # Return the least busy cage
                for j in range(0, len(self.col_list[column].cages)):
                    if len(self.col_list[column].cages[j].requests) < len(cage.requests):
                        cage = self.col_list[column].cages[j]
                return cage
    
    def requestElevator(self, cage, floor):
        cage.requests.append(floor)

    ## Reports ##
    def getCageStatus(self):
        for i in range(0, len(self.col_list)):
            for j in range(0, len(self.col_list[i].cages)):
                print("Column " + str(i) + ": Cage " + str(j) + " is " + self.col_list[i].cages[j].status)
                print("Current floor: " + str(self.col_list[i].cages[j].curFloor) + " Door status: " + self.col_list[i].cages[j].doors)

#############
## Startup ##
#############
def initialize():
    # Turn on the battery
    startup = input("Activate battery? (y/n): ")
    if startup == "y":
        print("Initializing...")
        global battery_on
        battery_on = True
        # Set total columns
        while True:
            try:
                global total_columns
                total_columns = int(input("Enter the total number of columns: "))
                break
            except ValueError:
                print("Please enter a valid number")
        while True:
            try:
                global cages_per_column
                cages_per_column = int(input("How many cages are installed per column?: "))
                break
            except ValueError:
                print("Please enter a valid number")
        while True:
            try:
                global total_floors
                total_floors = int(input("How many floors are there in the building?: "))
                break
            except ValueError:
                print("Please enter a valid number")
        print("battery_on = ", battery_on)
        print("total_columns = ", total_columns)
        print("cages_per_column = ", cages_per_column)
        print("total_floors = ", total_floors)
    else:
        print("Startup aborted!")
        return
    
    # Instantiate the CageManager
    global cageManager
    global floorList
    cageManager = CageManager()
    print("\nBeginning CageManager setup...\n")

    # Instantiate FloorButtons
    def instantiateFloorButtons():
        buttonList = []
        for i in range(0, total_floors):
            buttonList.append(FloorButton(i))
        return buttonList
    
    # Insert Cages into Columns
    def instantiateCages():
        listCages = []
        for i in range(0, cages_per_column):
            listCages.append(Cage("Idle", "Closed", instantiateFloorButtons()))
        return listCages

    # Insert columns into CageManager
    for i in range(0, total_columns):
        cageManager.col_list.append(Column("Active", instantiateCages()))
        print("Column " + str(i) + " is " + cageManager.col_list[i].status)

    # Confirm Cage status
    cageManager.getCageStatus()

    # Insert CallButtons into Floor
    def instantiateCallButtons(floor, column):
        listButtons = []
        listButtons.append(CallButton("Up", column, floor))
        listButtons.append(CallButton("Down", column, floor))
        return listButtons

    # Generate Floors and Call Buttons
    for x in range(0, total_columns):
        for i in range(0, total_floors):
            floorList.append(Floor(i, instantiateCallButtons(i, x)))
            print("Floor " + str(floorList[i].number) + " is initialized")
    
    # Confirm Button status
    for i in range(0, len(floorList)):
        for j in range(0, len(floorList[i].buttons)):
            print(str(floorList[i].buttons[j].column) + str(floorList[i].buttons[j].floor) + floorList[i].buttons[j].direction + " button is ready and " + floorList[i].buttons[j].status)


##########
## Main ##
##########
def main():
    initialize()
    
    # Test Function #
    floorList[3].buttons[0].callButtonPressed()
    floorList[3].getCallButtonStatus()
    print(str(cageManager.col_list[0].cages[0].requests))

main()