using Godot;
using KidoUtils;

public abstract partial class Interactable : AnimatedSprite2D {

    [Export]
    protected Area2D range;
    

    protected abstract void OnInteracted(Player player);


    private void SetIndicatorVisibility(bool isVisible) {
        Visible = isVisible;
    }

    // NOTE: This will 100% break when there are several players
    private void OnBodyEntered(Node2D body) {
        if (body is Player) SetIndicatorVisibility(true);
    }

    private void OnBodyExited(Node2D body) {
        if (body is Player) SetIndicatorVisibility(false);
    }

	public override void _Ready() {
        // Enforce the proper layers
        range.CollisionLayer = (uint) Layers.Environment;
        range.CollisionMask = (uint) Layers.Player;

        range.BodyEntered += OnBodyEntered;
	}
}

