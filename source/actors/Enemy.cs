using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;

public partial class Enemy : Actor,  IInteractable
{

    //Used for fade in-out.
    [Export]
    private Sprite2D sprite;

    public AIStateMachine StateMachine {get; init;}
    public Enemy() {
        StateMachine = new(this);
    }

    public override void _Ready() {
        base._Ready();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void OnDeath(DamageInstance damageInstance)
    {
        DeathState noState = new(damageInstance);
        
        StateMachine.AddState(noState, null);
        StateMachine.ChangeState(noState);
    }
    
    public override void OnDamaged(DamageInstance damageInstance) {
        
    }
}
