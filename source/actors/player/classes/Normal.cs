using Game.Actors;
using Game.Players;
using Godot;

namespace Game.SealedContent;

public sealed class Normal : IPlayerClass {
    
    ActorStats classStats = new() {
        damageTaken = new(0.8f, 0),
        damageDealt = new(1, 0.2f),
    };

    public PlayerClassResource classResource => ResourceLoader.Load<PlayerClassResource>("res://assets/content/classes/default.tres");

    public void ClassInit(Player player) {
        player.StatsManager.AddStats(classStats);
    }

    public void ClassRemoved(Player player) {
        player.StatsManager.RemoveStats(classStats);
    }
}
