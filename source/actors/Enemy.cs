using Godot;
using System.Collections.Generic;
using KidoUtils;
using LootTables;
using Game.Actors.AI;
using Game.Damage;
using Game.Animation;
using Game.Players;
using System;

namespace Game.Actors;

public abstract partial class Enemy : Actor, IPlayerAttackable {
    
    public static event Action<Enemy, DamageInstance> EnemyKilled; 

    [Export]
    private AnimationPlayer animationPlayer = new();
    private AIStateMachine stateMachine;

    public bool PauseAI {get; set;} = false;

    public override void _Ready() {
        base._Ready();

        stateMachine = new(this);
        
        DamageableComponent.OnDeath += OnDeath;

        Init(new(animationPlayer), stateMachine);
    }

    private void OnDeath(DamageInstance damageInstance) {
        DeathAnimation(damageInstance);
        DropLootTable();

        EnemyKilled?.Invoke(this, damageInstance);
    }

    protected abstract void Init(AnimationController animationController, AIStateMachine aIStateMachine);
    protected abstract List<ItemDrop> DeathDrops {get; init;}


    public override void _Process(double delta) {
        base._Process(delta);
        
        if (!PauseAI)
            stateMachine.UpdateState(delta);
    }

    public void DropLootTable() => DeathDrops.ForEach(loot => loot.Init(this));

    public void DeathAnimation(DamageInstance damageInstance) {
        AIState stateAtDeath;
        
        if (damageInstance.type is DamageInstance.Type.Void)
            stateAtDeath = new FallDeathState();
        else
            stateAtDeath = new DeathState(damageInstance);
        
        stateMachine.AddState(stateAtDeath, null);
        stateMachine.ChangeState(stateAtDeath);
        
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

    Node2D IPlayerAttackable.GetNode() => this;


    ///<Summary>
    ///Define the states and animations that you want this enemy to have. That is all.
    ///</Summary>

    #endregion
}
