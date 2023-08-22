using Game.Players;
using Game.Actors;
using Godot;

namespace Game.SealedContent;

public sealed partial class Weird : Player {
    
    ActorStats classStats = new() {
        damageTaken = new(2f, 0),
        damageDealt = new(0.5f, 0.2f),
        reloadSpeed = new(5f, 0),
    };
    public override void ClassInit() {
        actorStatsManager.AddStats(classStats);
    }
}
