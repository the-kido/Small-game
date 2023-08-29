using Game.Actors;
using Godot;

namespace Game.Players;

// What I want:
//  I want a player scene which is static / never changing
//      This is in order to keep consistency with other classes
//  I want to be able to change the mechanics of a player without changing the required functionality
//      I can do this via abstract classes, but that would mean having seperate scenes for every player, and I can't export things
//  I want to be able to instance a player in any scene with the appropriate class
//      I can do this via:
//          Creating a placeholder node for the player, which must call "spawn player" at some point
//          Having the player have a "PlayerClass" field which is a class 

public sealed partial class Normal : PlayerClass {
    
    ActorStats classStats = new() {
        damageTaken = new(0.8f, 0),
        damageDealt = new(2, 0.2f),
        reloadSpeed = new(0.1f, 0),
    };

    public PlayerClassResource PlayerClassResource => ResourceLoader.Load<PlayerClassResource>("res://assets/content/classes/default.tres");

    public void ClassInit(Player player) {
        player.StatsManager.AddStats(classStats);
    }

    public void ClassRemoved(Player player) {
        player.StatsManager.RemoveStats(classStats);
    }
}
