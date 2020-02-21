///////////////////////////////////////////////////////
// {Residential Controller}                          //
///////////////////////////////////////////////////////
// Author: {Joshua Knutson}                          //
// License: {GNUGPLv3}                               //
// Link: {https://www.gnu.org/licenses/gpl-3.0.html} //
// Version: {0.01}                                   //
// Contact: {github.com/alberbecois}                 //
///////////////////////////////////////////////////////


//////////////////////
// Global Variables //
//////////////////////
let battery_on = false;
let total_columns = null;
let cages_per_column = null;
let total_floors = null;
let cageManager = null;
let floorList = [];
function Sleep(milliseconds) {
    const date = Date.now();
    let currentDate = null;
    do {
    currentDate = Date.now();
    } while (currentDate - date < milliseconds);
};


/////////////
// Columns //
/////////////
class Column {
    constructor(status, cages){
        this.status = status;
        this.cages = cages;
    }
}


///////////
// Cages //
///////////
class Cage{
    constructor(id, status, doors){
        this.id = id;
        this.status = status;
        this.doors = doors;
        this.floorButtons = [];
        this.requests = [];
        this.curFloor = 0;
        this.direction = "Up";
        this.door_sensor_status = "Clear";
        this.closeButtonCheck = false;
    }

    openDoors(){
        if(this.status === "Loading"){
            this.doors = "Open";
            console.log("Cage doors are open for 8 seconds");
            for(var x = 0; x < floorList[this.curFloor].buttons; x++){
                x.status = "Inactive";
            }
            for(var x = 0; x < this.floorButtons; x++){
                x.status = "Inactive";
            }
            console.log("Doors closing in 8 seconds");
            Sleep(1000);
            console.log("Doors closing in 7 seconds");
            Sleep(1000);
            console.log("Doors closing in 6 seconds");
            Sleep(1000);
            console.log("Doors closing in 5 seconds");
            Sleep(1000);
            if(!this.closeButtonCheck){
                console.log("Doors closing in 4 seconds");
                Sleep(1000);
            }else {
                this.closeDoors();
                this.closeButtonCheck = false;
                return;
            }
            if(!this.closeButtonCheck){
                console.log("Doors closing in 3 seconds");
                Sleep(1000);
            }else {
                this.closeDoors();
                this.closeButtonCheck = false;
                return;
            }
            if(!this.closeButtonCheck){
                console.log("Doors closing in 2 seconds");
                Sleep(1000);
            }else {
                this.closeDoors();
                this.closeButtonCheck = false;
                return;
            }
            if(!this.closeButtonCheck){
                console.log("Doors closing in 1 seconds");
                Sleep(1000);
            }else {
                this.closeDoors();
                this.closeButtonCheck = false;
                return;
            }
            this.closeDoors();
        }
    }

    openButtonPressed(){
        if(this.status != "In-Service"){
            this.openDoors();
        }
    }

    closeDoors(){
        if(this.door_sensor_status === "Clear"){
            this.doors = "Closed";
            console.log("Cage doors are closed");
            this.status = "Loading";
        }
    }

    closeButtonPressed(){
        this.closeButtonCheck = true;
        console.log("Close button pressed");
    }

    // Movement //
    moveDown(requestedFloor){
        while(this.doors != "Closed"){
            this.closeDoors();
        }
        this.status = "In-Service";
        this.direction = "Down";
        while(this.curFloor != requestedFloor){
            console.log("Cage " + this.id + " going down at " + this.curFloor);
            this.curFloor -= 1;
        }
        console.log("Cage " + this.id + " at " + this.curFloor);
        this.status = "Loading";
        this.openDoors();
    }

    moveUp(requestedFloor){
        while(this.doors != "Closed"){
            this.closeDoors();
        }
        this.status = "In-Service";
        this.direction = "Up";
        while(this.curFloor != requestedFloor){
            console.log("Cage " + this.id + " going up at " + this.curFloor);
            this.curFloor += 1;
        }
        console.log("Cage " + this.id + " at " + this.curFloor);
        this.status = "Loading";
        this.openDoors();
    }

    // Reports //
    getFloorButtonStatus(){
        for(var x = 0; x < this.floorButtons; x++){
            console.log("Floor " + this.floorButtons[x].floorNum + " Button: Ready -- Status: " + this.floorButtons[x].status);
        }
    }
}


/////////////
// Buttons //
/////////////
class CallButton{
    constructor(direction, column, floor){
        this.direction = direction;
        this.column = column;
        this.floor = floor;
        this.status = "Inactive";
    }

    // Methods //
    requestPickup(direction, column, floor){
        var cage = cageManager.getAvailableCage(direction, column, floor);
        cageManager.requestElevator(cage, floor);
    }

    callButtonPressed(){
        this.status = "Active";
        console.log(this.direction + " button pressed at " + this.column + " column " + this.floor + " floor.");
        this.requestPickup(this.direction, this.column, this.floor);
    }
}

class FloorButton{
    constructor(cage, floor){
        this.cage = cage;
        this.floorNum = floor;
        this.status = "Inactive";
    }

    // Methods //
    floorButtonPressed(){
        this.status = "Active";
        console.log(this.floorNum + " button pressed from inside the cage");
        cageManager.requestFloor(this.cage, this.floorNum);
    }
}


////////////
// Floors //
////////////
class Floor{
    constructor(number, buttons){
        this.number = number;
        this.buttons = buttons;
    }

    // Reports //
    getCallButtonStatus(){
        for(var x = 0; x < this.buttons.length; x++){
            console.log(this.buttons[x].direction + " button is " + this.buttons[x].status);
        }
    }
}


//////////////
// Requests //
//////////////
class Request{
    constructor(status, floor){
        this.status = status;
        this.floor = floor;
    }
}


//////////////////
// Cage Manager //
//////////////////
class CageManager{
    constructor(){
        this.col_list = [];
    }

    // Methods //
    getAvailableCage(direction, column, reqFloor){
        for(var x = 0; x < this.col_list[column].cages.length; x++){
            var cage = this.col_list[column].cages[x];
            if(cage.direction === direction && direction === "Up" && cage.curFloor < reqFloor){
                return cage; // Going same direction (UP) before requested floor
            }else if(cage.direction === direction && direction === "Down" && cage.curFloor > reqFloor){
                return cage; // Going same direction (DOWN) before requested floor
            }else if(cage.status === "Idle"){
                return cage; // Return an unoccupied cage
            }else {
                for(var i = 0; i < this.col_list[column].cages; i++){
                    if(this.col_list[column].cages[i].requests.length < cage.requests.length){ //Return least busy cage
                        cage = this.col_list[column].cages[i];
                    }
                    return cage;
                }
            }
        }
    }

    requestElevator(cage, floor){
        cage.requests.push(new Request("Pending", floor));
        console.log("Floor " + cage.requests[cage.requests.length-1].floor + " added to request list.");
        if(cage.direction === "Up"){
            // Sort ascending
        }else {
            // Sort descending
        }
    }

    requestFloor(cage, floor){
        cage.requests.push(new Request("Pending", floor));
        console.log("Floor " + cage.requests[cage.requests.length-1].floor + " added to request list.");
        if(cage.direction === "Up"){
            // Sort ascending
        }else {
            // Sort descending
        }
    }

    dispatchElevators(){
        for(var x = 0; x < this.col_list.length; x++){
            for(var i = 0; i < this.col_list[x].cages.length; i++){
                var curCage = this.col_list[x].cages[i];
                while(curCage.requests.length != 0){
                    for(var j = 0; j < curCage.requests.length; j++){
                        if(curCage.requests[j].status === "Pending"){
                            if(curCage.requests[j].floor > curCage.curFloor){
                                curCage.moveUp(curCage.requests[j].floor);
                            }
                            else {
                                curCage.moveDown(curCage.requests[j].floor);
                            }
                            curCage.requests[j].status = "Completed";
                        }
                        curCage.requests = [];
                        curCage.status = "Idle";
                    }
                }
            }
        }
    }

    // Reports //
    getCageStatus(){
        for(var x = 0; x < this.col_list.length; x++){
            for(var i = 0; i < this.col_list[x].cages.length; i++){
                console.log("Column " + x + ": Cage " + i + " is " + this.col_list[x].cages[i].status);
                console.log("Current floor: " + this.col_list[x].cages[i].curFloor + " Door status: " + this.col_list[x].cages[i].doors);
            }
        }
    }
}


////////////////////
// Initialization //
////////////////////
function initialize(){
    // Turn on the battery
    var startup = prompt("Activate battery? (y/n): ");
    if(startup === "y"){
        console.log("Initializing...");
        battery_on = true;
        
        // Set total columns
        var total_columns_check = false;
        while(!total_columns_check){
            if(total_columns === null || isNaN(total_columns)){
                total_columns = prompt("Enter the total number of columns: ");
            }
            else {
                total_columns_check = true;
            }
        }
        // Set cages per column
        var cages_per_column_check = false;
        while(!cages_per_column_check){
            if(cages_per_column === null || isNaN(cages_per_column)){
                cages_per_column = prompt("How many cages are installed per column?: ");
            }
            else {
                cages_per_column_check = true;
            }
        }
        // Set number of floors
        var total_floors_check = false;
        while(!total_floors_check){
            if(total_floors === null || isNaN(total_floors)){
                total_floors = prompt("How many floors are there in the building?: ");
            }
            else {
                total_floors_check = true;
            }
        }

        // Confirm setup conditions
        console.log("\n---HARDWARE SIMULATION---");
        console.log("battery_on = " + battery_on);
        console.log("total_columns = " + total_columns);
        console.log("cages_per_column = " + cages_per_column);
        console.log("total_floors = " + total_floors);
    }
    else {
        console.log("Startup aborted!");
        return;
    }

    // Instantiate the CageManager
    cageManager = new CageManager();
    console.log("\nBeginning CageManager setup...\n \n---COLUMNS AND CAGES---");

    // Instantiate FloorButtons
    function instantiateFloorButtons(cage){
        var buttonList = [];
        for(var x = 0; x < total_floors; x++){
            buttonList.push(new FloorButton(cage, x));
        }
        return buttonList;
    }

    // Insert Cages into Columns
    function instantiateCages(){
        var listCages = [];
        for(var x = 0; x < cages_per_column; x++){
            listCages.push(new Cage(x, "Idle", "Closed"));
        }
        return listCages
    }

    // Insert columns into CageManager
    for(var x = 0; x < total_columns; x++){
        cageManager.col_list.push(new Column("Active", instantiateCages()));
        console.log("Column " + x + " is " + cageManager.col_list[x].status);
    }

    // Insert FloorButtons into Cages
    for(var x = 0; x < cageManager.col_list.length; x++){
        for(var i = 0; i < cageManager.col_list[x].cages.length; i++){
            var curCage = cageManager.col_list[x].cages[i];
            curCage.floorButtons = instantiateFloorButtons(curCage);
        }
    }
        
    // Confirm Cage status
    cageManager.getCageStatus();

    // Insert CallButtons into Floor
    function instantiateCallButtons(floor, column){
        var listButtons = [];
        listButtons.push(new CallButton("Up", column, floor));
        listButtons.push(new CallButton("Down", column, floor));
        return listButtons;
    }
    
    // Generate Floors and Call Buttons
    console.log("\n---FLOORS---");
    for(var x = 0; x < total_columns; x++){
        for(var i = 0; i < total_floors; i++){
            floorList.push(new Floor(i, instantiateCallButtons(i, x)));
            console.log("Floor " + floorList[i].number + " is initialized");
        }
    }
    
    // Confirm Button status
    console.log("\n---CALL BUTTONS---");
    for(var x = 0; x < floorList.length; x++){
        for(var i = 0; i < floorList[x].buttons.length; i++){
            console.log(floorList[x].buttons[i].column + "-" + floorList[x].buttons[i].floor + " " + floorList[x].buttons[i].direction + " button is ready and " + floorList[x].buttons[i].status);
        }
    }
}

///////////////
// Scenarios //
///////////////
function demo(){
    console.log("\nFor demonstration purposes only...\n");
    floorList[5].buttons[0].callButtonPressed();
    cageManager.dispatchElevators();
    cageManager.col_list[0].cages[0].floorButtons[1].floorButtonPressed();
    cageManager.dispatchElevators();
    cageManager.getCageStatus();
}


//////////
// Main //
//////////
function main(){
    initialize();
    
    if(battery_on){
        demo();
    }
    else {
        console.log("Exiting program");
    }
}

main();

