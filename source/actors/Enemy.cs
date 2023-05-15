using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;

public partial class Enemy : Actor, IInteractable
{
    [Export]
    private AnimationPlayer animationPlayer = new();

    private AIStateMachine StateMachine {get; init;}
    private AnimationController AnimationController {get; set;}
    
    public Enemy() {
        StateMachine = new(this);
    }
    
    public override void _Ready() {
        base._Ready();
        AnimationController = new(animationPlayer);
        Init(AnimationController, StateMachine);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        StateMachine.UpdateState(delta);
    }

    public override void OnDeath(DamageInstance damageInstance) {
        DeathState noState = new(damageInstance);
        StateMachine.AddState(noState, null);
        StateMachine.ChangeState(noState);

        //No longer recieve damage.
        DamageableComponent.QueueFree();
        
        //Stop all collisions from happening
        CollisionLayer = 0;
        //Except for the enviornment, because the dead body can still interact with that.
        CollisionMask = (int) Layers.Enviornment;
    }
    

    public override void OnDamaged(DamageInstance damageInstance) {
        
    }

    
    public virtual void Init(AnimationController animationController, AIStateMachine aIStateMachine) {
        throw new NotImplementedException();
    }

    #region IInteractable

    bool IInteractable.IsInteractable() {
        return DamageableComponent.IsAlive;
    }

    Vector2 IInteractable.GetPosition() {
        return GlobalPosition;
    }

    CollisionShape2D IInteractable.GetCollisionShape()
    {
        return CollisionShape;
    }

    ///<Summary>
    ///Define the states and animations that you want this enemy to have. That is all.
    ///</Summary>

    #endregion
}
