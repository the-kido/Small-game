using Godot;
using System;

public abstract partial class Pickupable : Node2D {

    public static PackedScene PackedScene {get; private set;} 

	public sealed override void _Process(double delta) {
        Player.players.ForEach(player => Update(player, delta));
        SplashUpdate(delta);
	}

    [Export]
    protected float Speed {get; set;} = 1;
    [Export]
    const int ATTRACTION_DISTANCE = 400;
    const int MINIMUM_ATTACTION_DISTANCE = 50;

    protected bool IsPickupable {get; set;} = true;

    private float Multiplier(float distance) {
        
        if (distance > ATTRACTION_DISTANCE) return 0;
        if (distance < MINIMUM_ATTACTION_DISTANCE) distance = 50;
        
        float multiplier = -3 * MathF.Log10(MathF.Pow(distance - MINIMUM_ATTACTION_DISTANCE, 1/4.6f)) + 1.67f;
        
        //multiplier cannot go past 1.5
        return MathF.Min(multiplier, 1.5f);
    }

    protected abstract void AbsorbPickupable(Player player);

    private void Absorb(Player player) {
        AbsorbPickupable(player);
        QueueFree();
    }
   
    private void Update(Player player, double delta) {
        // Don't continue to move the orb if it's not pickupable
        if (!IsPickupable) return;

        float distance = GlobalPosition.DistanceTo(player.GlobalPosition);
        if (distance < MINIMUM_ATTACTION_DISTANCE / 2) Absorb(player);

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
        if (splashIndex > 1) return;

        splashIndex += (float) delta;

        Position += direction * (float) delta * SPLASH_SPEED * MathF.Log10(splashIndex);
    }

}
