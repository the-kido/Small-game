using System;
using Game.Data;
using Game.Players;
using Godot;

namespace Game.LevelContent.Pickupables;

public partial class FreezeCharge : Pickupable {

    protected override int AttractionDistance => 100;

    protected override void AbsorbPickupable(Player player) {
        RunData.FreezeOrbs.Add(1);
    }

    public override void _Ready() {
        
        if (Level.IsCurrentLevelCompleted()) 
            QueueFree();
        
        Random random = new();
        float single = random.NextSingle();
        
        if (single > (1f/3f)) 
            QueueFree();
    }
}
