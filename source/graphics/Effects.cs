using Godot;

namespace Game.Graphics;

public static class Effects {

    private static PackedScene Load(string path) => ResourceLoader.Load<PackedScene>(path);

    public readonly static PackedScene 
    Fire = Load("res://assets/effects/fire.tscn"),
    Wet = Load("res://assets/effects/wet.tscn"),
    Gas = Load("res://assets/effects/gas.tscn"),
    Shield = Load("res://assets/effects/shield.tscn"),
    Ice = Load("res://assets/effects/ice.tscn");

}
