using Godot;
using System;

public partial class Player : Actor
{
    public override void _Ready() {
        
        GD.Print(DamageableComponent.Health);
        DamageableComponent.Damage(new DamageInstance() {damage = 10});
        GD.Print(DamageableComponent.Health);
    }   
    
    public override void OnDeath(DamageInstance damageInstance)
    {
        GD.Print("NOOOO THE PLAYER IS DEADDD");
    }

    public override void OnDamaged(DamageInstance damageInstance)
    {
        throw new NotImplementedException();
    }
}
