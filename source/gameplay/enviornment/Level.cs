using Godot;
using System;
using System.Collections.Generic;

public partial class Level : Node{

	
	public new static event Action<Level> Ready;


    [Export]
    public Godot.Collections.Array<NodePath> doors = new();

    public Door GetLinkedDoor(string name) {
        foreach (NodePath door in doors) {
            Door _door = GetNode<Door>(door);

            GD.Print(_door.Name, name);

            if (_door.Name == name) 
                return _door;
        }
        return null;
    }
    public override void _Ready() {
        ChangeLevel();
    }

    private void ChangeLevel() {
        currentLevel = this;
        
        Ready?.Invoke(this);
    }

    public Action LevelCompleted; 
    public static Level currentLevel = new();

    //public Dictionary<string, Door> doors2 = new();

}