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
    def __init__(self, direction, floor):
        self.direction = direction
        self.floor = floor
        self.status = "Inactive"

    def requestPickup(self, direction, floor):
        cage = cageManager.getAvailableCage(direction, floor)
        cageManager.sendRequest(cage)
    
    def callButtonPressed(self):
        self.status = "Active"
        self.requestPickup(self.direction, self.floor)

class FloorButton:
    def __init__(self, floor):
        self.floorNum = floor


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
        battery_on = True
        # Set total columns
        while True:
            try:
                total_columns = int(input("Enter the total number of columns: "))
                break
            except ValueError:
                print("Please enter a valid number")
        while True:
            try:
                cages_per_column = int(input("How many cages are installed per column?: "))
                break
            except ValueError:
                print("Please enter a valid number")
        while True:
            try:
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
    def instantiateCallButtons(floor):
        listButtons = []
        listButtons.append(CallButton("Up", floor))
        listButtons.append(CallButton("Down", floor))
        return listButtons

    # Generate Floors and Call Buttons
    for i in range(0, total_floors):
        floorList.append(Floor(i, instantiateCallButtons(i)))
        print("Floor " + str(floorList[i].number) + " is initialized")
    
    # Confirm Button status
    for i in range(0, len(floorList)):
        for j in range(0, len(floorList[i].buttons)):
            print(floorList[i].buttons[j].direction + " button is ready and " + floorList[i].buttons[j].status)

initialize()
