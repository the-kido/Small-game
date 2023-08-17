using LootTables;
using System.Collections.Generic;
using Game.Actors;
using Game.Animation;
using Game.Actors.AI;

namespace Game.SealedContent;

public sealed partial class RedBlobEnemy : Enemy {
	protected override List<ItemDrop> DeathDrops {get; init;} = EnemyLootTable.NONE;

	protected sealed override void Init(AnimationController animationController, AIStateMachine aIStateMachine) {
    
    }
}
