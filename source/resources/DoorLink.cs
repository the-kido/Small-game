using Godot;
using System;

public partial class DoorLink : Resource {

    // When the scene switches, invoke the events attached to the two doors for "open other door"
    // whichever door is still in scene, it will open for it. 
    // right?
    public event Action OnSceneSwitched;

}
