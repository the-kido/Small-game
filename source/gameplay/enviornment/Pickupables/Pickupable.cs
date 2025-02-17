using Godot;
using System;
using Game.Players;
using KidoUtils;
using Game.Autoload;

namespace Game.LevelContent.Pickupables;

public abstract partial class Pickupable : Node2D {

    [Export]
    protected float Speed {get; set;} = 1;
    
    protected virtual int AttractionDistance {get;} = 400;
    protected virtual int MinimumAttractionDistance {get;} = 50;

    public sealed override void _Process(double delta) {
        Player.Players.ForEach(player => MoveTowardsPlayer(player, delta));
        Player.Players.ForEach(player => Update(delta));
        SplashUpdate(delta);
	}

    protected bool IsPickupable {get; set;} = true;

    private float Multiplier(float distance) {
        
        if (distance > AttractionDistance) return 0;
        if (distance < MinimumAttractionDistance) distance = 50;
        
        float multiplier = -3 * MathF.Log10(MathF.Pow(distance - MinimumAttractionDistance, 1/4.6f)) + 1.67f;
        
        //multiplier cannot go past 1.5
        return MathF.Min(multiplier, 1.5f);
    }

    protected abstract void AbsorbPickupable(Player player);
    protected virtual void Update(double delta) {}

    private void Absorb(Player player) {
        AbsorbPickupable(player);
        QueueFree();
        Utils.GetPreloadedScene<PickupablesManager>(this, PreloadedScene.PickupablesManager).RemoveChild(this);
    }
   
    private void MoveTowardsPlayer(Player player, double delta) {
        // Don't continue to move the orb if it's not pickupable
        if (!IsPickupable) 
            return;

        float distance = GlobalPosition.DistanceTo(player.GlobalPosition);
        
        if (distance < MinimumAttractionDistance / 2) 
            Absorb(player);

        float flatMultipler = (float) delta * 1000f * Speed;
        float magnitude = Multiplier(distance) * flatMultipler;

        Vector2 followDirection = GlobalPosition.DirectionTo(player.GlobalPosition);
        GlobalPosition += followDirection * magnitude; 
    }
}

// Implement all of the "splashing" stuff (yes, i really couldn't think of a better name) seperately from the main stuff.
public abstract partial class Pickupable : Node2D {

    public void SplashOut() {
        splashIndex = 0;
        direction = direction.Random();
    }

    // default to 1 to avoid the splash when instanced.
    float splashIndex = 1;
    const int SPLASH_SPEED = 200;
    Vector2 direction = Vector2.Zero;

    private void SplashUpdate(double delta) {
        if (splashIndex > 1) 
            return;

        splashIndex += (float) delta;

        Position += direction * (float) delta * SPLASH_SPEED * MathF.Log10(splashIndex);
    }
}
