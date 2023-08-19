using System.Collections.Generic;
using Game.Actors;
using Godot;

namespace Game.Players.Inputs;

public partial class PlayerInteractableRadar : Area2D{
    public List<Actor> NearbyEnemies {get; private set;} = new();

    public override void _Ready() {
        BodyEntered += OnNearbyEnemyAreaEntered;
        BodyExited += OnNearbyEnemyAreaEntered;
    }
    
    private void OnNearbyEnemyAreaEntered(Node2D body) =>
        NearbyEnemies.Add((Actor) body);
    private void OnNearbyEnemyAreaExited(Node2D body) =>
        NearbyEnemies.Remove((Actor) body);

}