###################################################
## {Residential Controller}
###################################################
## Author: {Joshua Knutson}
## License: {GNUGPLv3}
## Link: {https://www.gnu.org/licenses/gpl-3.0.html}
## Version: {0.01}
## Contact: {github.com/alberbecois}
###################################################


######################
## Global Variables ##
######################
battery_on = False
total_columns = None
cages_per_column = None
total_floors = None

#############
## Classes ##
#############
class Column:
    def __init__(self, status, cages):
        self.status = status
        self.cages = cages

class Cage:
    def __init__(self, status, buttons, doors, requests):
        self.status = status
        self.buttons = buttons
        self.doors = doors
        self.requests = requests

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
    

initialize()