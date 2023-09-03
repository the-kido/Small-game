using System.Collections.Generic;
using Game.LevelContent.Pickupables;
using Godot;

namespace Game.Autoload;

// It just makes autoloading more convinient to have this class for reference
public partial class PickupablesManager : Node {

    public readonly List<Pickupable> pickupables = new();

    public override void _Ready() {
        SceneSwitcher.SceneSwitched += ClearAllPickupables;
    }

    private void ClearAllPickupables() {
        for (int i = 0; i < pickupables.Count; i++)
            pickupables[i].QueueFree();
        
        pickupables.Clear();
    }

    public void AddChild(Pickupable node) {
        base.AddChild(node);

        pickupables.Add(node);

    }
    public void RemoveChild(Pickupable node) {
        pickupables.Remove(node);
    }
}
