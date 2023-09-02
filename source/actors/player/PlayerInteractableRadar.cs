using System.Collections.Generic;
using Godot;

namespace Game.Players.Inputs;

public partial class PlayerInteractableRadar : Area2D{
    public List<IPlayerAttackable> NearbyEnemies {get; private set;} = new();

    public override void _Ready() {
        BodyEntered += OnNearbyEnemyAreaEntered;
        BodyExited += OnNearbyEnemyAreaExited;
    }
    
    private void OnNearbyEnemyAreaEntered(Node2D body) =>
        NearbyEnemies.Add((IPlayerAttackable) body);
    private void OnNearbyEnemyAreaExited(Node2D body) =>
        NearbyEnemies.Remove((IPlayerAttackable) body);

}