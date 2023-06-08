using Godot;
using System;

public partial class DoorLink : Resource {
    [Export]
    public Door firstDoor;
    [Export]
    public Door secondDoor;

    // public Door GetFirstDoor(Node node) => node.GetTree().Root.GetNode<Door>(secondDoor);
    // public Door GetSecondDoor(Node node) => node.GetTree().Root.GetNode<Door>(secondDoor);

    // Door selectedDoor;
    // public Door otherDoor;

    // public void OnEnteredDoor(Door enteredDoor) {

    //     if (enteredDoor == GetFirstDoor(enteredDoor)) {
    //         selectedDoor = GetFirstDoor(enteredDoor);
    //         otherDoor = GetSecondDoor(enteredDoor);
    //     } else {
    //         selectedDoor = GetSecondDoor(enteredDoor);
    //         otherDoor = GetFirstDoor(enteredDoor);
    //     }

    // }

    [Export] 
    public Vector2 direction1;
    [Export] 
    public Vector2 direction2;
}
