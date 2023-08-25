using Game.Players;
using Game.Actors;
using Game.Players.Inputs;
using Godot;
using Game.LevelContent;


namespace Game.SealedContent;

public sealed partial class Weird : Player {
    
    ActorStats classStats = new() {
        damageTaken = new(2f, 0),
        damageDealt = new(4f, 0.2f),
        reloadSpeed = new(0.1f, 0),
        speed = new(2, 0),
    };

    // protected override PlayerClassResource playerClassResource => 

    public override void ClassInit() {
        StatsManager.AddStats(classStats);

        WeirdInput weirdInput = new(this);
        InputController.AddInput(weirdInput, true);
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