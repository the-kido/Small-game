using System.Collections.Generic;
using Godot;
using LootTables;
using Game.Actors;
using Game.Actors.AI;
using Game.Animation;

namespace Game.SealedContent;

public sealed partial class WeirdEnemy : Enemy {
    [Export] 
    public Pathfinder pathfinderComponent;
    [Export]
    private int HoverAtSpawnPointDistance = 0;
    [Export]
    private PackedScene spamedBullet;
    [Export]
    private float attackDelay;

	protected override List<ItemDrop> DeathDrops {get; init;} = EnemyLootTable.GENERIC_ENEMY_DROPS;

	protected sealed override void Init(AnimationController animationController, AIStateMachine stateMachine) {
        DefaultAttackState attackState = new(pathfinderComponent, spamedBullet, attackDelay);
        PatrolState patrolState = new(pathfinderComponent, HoverAtSpawnPointDistance);
        
        animationController.AddAnimation(new("shoot", 2), ref attackState.OnShoot);

        animationController.AddAnimation(new("idle", 1), ref patrolState.IsIdle);
        animationController.AddAnimation(new("flying", 1), ref patrolState.IsMoving);

        stateMachine.AddState(attackState, patrolState);
        stateMachine.AddState(patrolState, attackState);

        //Set default state.
        stateMachine.ChangeState(patrolState);
    }
}

