using Godot;

public sealed partial class BadShield : Shield {
    public override string Description => "It really sucks dude";

    public static PackedScene PackedSceneResource => ResourceLoader.Load<PackedScene>("res://source/content/shields/bad_shield.tscn");
}