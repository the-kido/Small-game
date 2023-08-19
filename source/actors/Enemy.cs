using Godot;
using System.Collections.Generic;
using KidoUtils;
using LootTables;
using Game.Actors.AI;
using Game.Damage;
using Game.Data;
using Game.Animation;
using Game.Players;

namespace Game.Actors;

public abstract partial class Enemy : Actor, IPlayerAttackable {
    
    [Export]
    private AnimationPlayer animationPlayer = new();
    private AIStateMachine stateMachine;

    public override void _Ready() {
        base._Ready();

        stateMachine = new(this);
        
        DamageableComponent.OnDeath += OnDeath;

        Init(new(animationPlayer), stateMachine);
    }

    ActorStatsManager actorStatsManager;
    public override void ApplyStats(ActorStats newStats) {
        EffectiveSpeed = (int) (newStats.speedMultiplier * moveSpeed);
    }

    private void OnDeath(DamageInstance damageInstance) {
        DeathAnimation(damageInstance);
        DropLootTable();
        DungeonRunData.EnemiesKilled.AddDeath(this);
    }

    protected abstract void Init(AnimationController animationController, AIStateMachine aIStateMachine);
    protected abstract List<ItemDrop> DeathDrops {get; init;}


    public override void _Process(double delta) {
        base._Process(delta);
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


    ///<Summary>
    ///Define the states and animations that you want this enemy to have. That is all.
    ///</Summary>

    #endregion
}
