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
        this.timer = 0;
        this.door_sensor_status = "Clear";
    }

    // Door Methods //
    openDoors(){
        if(this.status === "Loading"){
            this.doors = "Open";
            console.log("Cage doors are open for 8 seconds");
            this.timer = 8;
            for(var x = 0; x < floorList[this.curFloor].buttons; x++){
                x.status = "Inactive";
            }
            for(var x = 0; x < this.floorButtons; x++){
                x.status = "Inactive";
            }
            var doorTimer = function(){
                if(this.timer === 0){
                    return;
                }else {
                    console.log("Closing in " + this.timer + " seconds");
                    this.timer -= 1;
                    setTimeout(doorTimer, 1000);
                }
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
        if(this.door_sensor_status === "Clear" && this.timer < 5){
            this.doors = "Closed";
            console.log("Cage doors are closed");
            this.status = "Loading";
        }
    }

    closeButtonPressed(){
        if(this.timer < 5){
            this.closeDoors();
        }
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
        cage = cageManager.getAvailableCage(direction, column, floor);
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
        cage.requests.push(request = new Request("Pending", floor));
        console.log("Floor " + cage.requests[cage.requests.length-1].floor + " added to request list.");
        if(cage.direction === "Up"){
            // Sort ascending
        }else {
            // Sort descending
        }
    }

    requestFloor(cage, floor){
        cage.requests.push(request = new Request("Pending", floor));
        console.log("Floor " + cage.requests[cage.requests.length-1].floor + " added to request list.");
        if(cage.direction === "Up"){
            // Sort ascending
        }else {
            // Sort descending
        }
    }

    dispatchElevators(){
        for(var x = 0; x < this.col_list.length; x++){
            for(var i = 0; i < this.col_list[x].cages[i]; i++){
                var curCage = this.col_list[x].cages[j];
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
            for(var i = 0; i < this.col_list[x].cages; i++){
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
    
}