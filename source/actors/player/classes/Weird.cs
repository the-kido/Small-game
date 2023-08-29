using Game.Players;
using Game.Actors;
using Game.Players.Inputs;
using Godot;
using Game.LevelContent;

namespace Game.SealedContent;

public sealed partial class Weird : PlayerClass {
    ActorStats classStats = new() {
        damageTaken = new(2f, 0),
        damageDealt = new(4f, 0.2f),
        reloadSpeed = new(0.1f, 0),
        speed = new(2, 0),
    };

    public PlayerClassResource PlayerClassResource => 
        PlayerClasses.List["Weird"]; 

    WeirdInput weirdInput;
    public void ClassInit(Player player) {
        player.StatsManager.AddStats(classStats);

        weirdInput = new(player);
        player.InputController.AddInput(weirdInput, true);
    }

    public void ClassRemoved(Player player) {
        player.StatsManager.RemoveStats(classStats);
        player.InputController.RemoveInput(weirdInput, true);
    }
}

public class WeirdInput : IInput {
    readonly Player player;
    public WeirdInput(Player player) {
        this.player = player;
    }

    private readonly CompressedTexture2D temp = ResourceLoader.Load<CompressedTexture2D>("res://assets/effects/shield.png");
    
    public void Update(double delta) {
        if (Input.IsActionJustPressed("select_slot_1")) {
            Sprite2D sprite2D = new() {
                Texture = temp,
                GlobalPosition = player.GlobalPosition,
            };

            Level.CurrentLevel.AddChild(sprite2D);
        }
    }
}