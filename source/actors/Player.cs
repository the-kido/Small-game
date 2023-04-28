using Godot;
using System;

public partial class Player : Actor
{
    //Hide speed value for player.
    private new int MoveSpeed;

    public override void _Ready() {
        base._Ready();
        DamageableComponent.Damage(new DamageInstance() {damage = 10});
    }   
    
    public override void OnDeath(DamageInstance damageInstance)
    {
        GD.Print("NOOOO THE PLAYER IS DEADDD");
    }

    public override void OnDamaged(DamageInstance damageInstance)
    {
        GD.Print("player says OW");
    }
}
