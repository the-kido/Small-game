using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    private Damageable damageableComponent;

    public override void _Ready() {
        
        damageableComponent.Damage(new DamageInstance() {damage = 10});

        //damageableComponent.Ondamaged += help;
    } 
    public void help(DamageInstance a) {
        GD.Print("This got invoked!");
    }
    
}
