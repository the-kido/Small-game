using Godot;
using LootTables;
using System;
using System.Collections.Generic;

public sealed partial class RedBlobEnemy : Enemy
{
	protected override List<ItemDrop> DeathDrops {get; init;} = EnemyLootTable.NONE;

	protected sealed override void Init(AnimationController animationController, AIStateMachine aIStateMachine) {
    
    }
}
