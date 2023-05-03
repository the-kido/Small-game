using Godot;
using System.Collections.Generic;

public sealed partial class Player : Actor
{
    //Hide speed value for player.


    private new int MoveSpeed;

    public List<Actor> NearbyEnemies {get; private set;} = new();
    public static Player[] players {get; private set;} = new Player[4];

    public override void _Ready() {
        base._Ready();
        players[0] = this;
    }   
    
    public override void OnDeath(DamageInstance damageInstance)
    {
        //Invisible bc fun
        Modulate = new Color(0,0,0,0);
    }

    public override void OnDamaged(DamageInstance damageInstance)
    {
        //GD.Print("player says OW");
    }

    #region signals
    
    private void OnNearbyEnemyAreaEntered(Node2D body) =>
        NearbyEnemies.Add((Actor) body);
    private void OnNearbyEnemyAreaExited(Node2D body) =>
        NearbyEnemies.Remove((Actor) body);

    #endregion
}
