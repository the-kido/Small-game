using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;

public partial class Enemy : Actor, IInteractable
{
    [Export]
    private AnimationPlayer animationPlayer = new();
    
    private AIStateMachine stateMachine;

    public override void _Ready() {
        base._Ready();

        stateMachine = new(this);
        DamageableComponent.OnDeath += DeathAnimation;

        Init(new(animationPlayer), stateMachine);
    }

    public override void _Process(double delta) {
        base._Process(delta);
        stateMachine.UpdateState(delta);
    }

    public void DeathAnimation(DamageInstance damageInstance) {

        DeathState noState = new(damageInstance);
        stateMachine.AddState(noState, null);
        stateMachine.ChangeState(noState);
        
        foreach (string name in animationPlayer.GetAnimationList()) {
            if (name is not "death") continue;
            animationPlayer.Play("death");
        }

        //Stop all collisions from happening
        CollisionLayer = 0;
        //Except for the enviornment, because the dead body can still interact with that.
        CollisionMask = (int) Layers.Enviornment;
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
