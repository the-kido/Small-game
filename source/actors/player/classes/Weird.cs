using Game.Players;
using Game.Actors;
using Godot;

namespace Game.SealedContent;

public sealed partial class Weird : Player {
    
    ActorStats classStats = new() {
        damageTaken = new(2f, 0),
        damageDealt = new(4f, 0.2f),
        reloadSpeed = new(0.1f, 0),
        speed = new(2, 0),
    };
    public override void ClassInit() {
        StatsManager.AddStats(classStats);
    }
}
