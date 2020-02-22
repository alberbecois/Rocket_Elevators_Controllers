# Residential Controller v1.00

[CodeBoxx Technology School - Odyssey Week 2 Assignment.]

## Getting Started
### Requirements

* [Python 3](https://www.python.org/) - To run the Python version
* [REPL.it](https://repl.it/) - To run the JavaScript version

## Running the tests

When the program is run, you will be asked to activate the battery. To begin, you must enter 'y' as the response. Anything else will abort the setup. Setup takes three inputs: The number of columns, the number of cages per column, and the number of floors. The appropriate number of each corresponding object will then be generated.

To create the conditions specified by the requirements please enter 1 for columns, 2 for cages per column, and 10 for floors. There are two hard coded scenarios which are provided for demonstration purposes, it is worth noting that they will run as long as there are a minimum of 1 column, 1 cage, and 10 floors.

After setup is complete you will be prompted to select a scenario to run. It is possible to modify the scenarios using the following methods.

## Methods

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