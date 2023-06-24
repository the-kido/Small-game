using Godot;
using System.Collections.Generic;
using KidoUtils;
using LootTables;

public abstract partial class Enemy : Actor, IPlayerAttackable {
    
    [Export]
    private AnimationPlayer animationPlayer = new();
    private AIStateMachine stateMachine;

    public override void _Ready() {
        base._Ready();

        stateMachine = new(this);
        
        DamageableComponent.OnDeath += DeathAnimation;
        DamageableComponent.OnDeath += DropLootTable;

        Init(new(animationPlayer), stateMachine);
    }

    protected abstract void Init(AnimationController animationController, AIStateMachine aIStateMachine);
    protected abstract List<Loot> DeathDrops {get; init;}


    public override void _Process(double delta) {
        base._Process(delta);
        stateMachine.UpdateState(delta);
    }

    public void DropLootTable(DamageInstance _) => DeathDrops.ForEach(loot => loot.Init(this));

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
        CollisionMask = (int) Layers.Environment;
    }

    #region IInteractable

    bool IPlayerAttackable.IsInteractable() => DamageableComponent.IsAlive;

    Vector2 IPlayerAttackable.GetPosition() => GlobalPosition;


    ///<Summary>
    ///Define the states and animations that you want this enemy to have. That is all.
    ///</Summary>

    #endregion
}
