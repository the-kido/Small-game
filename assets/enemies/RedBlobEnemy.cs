using Godot;
using LootTables;
using System;
using System.Collections.Generic;

public sealed partial class RedBlobEnemy : Enemy
{
	protected override List<Loot> DeathDrops {get; init;} = LootTable.NONE;

	protected sealed override void Init(AnimationController animationController, AIStateMachine aIStateMachine) {
    
    }
}
