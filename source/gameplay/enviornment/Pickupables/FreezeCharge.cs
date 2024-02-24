using System;
using Game.Data;
using Godot;

namespace Game.LevelContent.Pickupables;

public partial class FreezeCharge : Sprite2D {

    [Export]
    Interactable interactable;

    public override void _Ready() {
        if (Level.IsCurrentLevelCompleted()) GetParent().QueueFree();

        if (new Random((int)Time.GetTicksMsec()).NextSingle() > 1f / 3f) {
            GetParent().QueueFree();
        } else {
            // Delete them after the level finishes
            Level.CurrentLevel.LevelCompleted += PlayAbsorbAnimation;

            interactable.Interacted += (player) => {
                RunData.FreezeOrbs.Add(1);
                PlayAbsorbAnimation(); // Play death animation in the future or something.
            };
        }
    }
    
    bool absorbed = false;
    // Will add in the future.
    private void PlayAbsorbAnimation() {
        if (absorbed) return;
        absorbed = true;

        GetParent().QueueFree();
    }
}
