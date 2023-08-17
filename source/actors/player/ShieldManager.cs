using Godot;
using System;

namespace Game.Players.Mechanics;

// TODO:
// this is a VERY small bug but if there is a chest
// then the shield will regen its heatlh!
public partial class ShieldManager : Node2D { 
    
    public Shield HeldShield {get; private set;}

    public event Action<Shield> ShieldAdded;
    public event Action<Shield> ShieldRemoved;

    Player player;
    public void Init(Player player) {
        this.player = player;
    }

    public override void _Process(double delta) => HeldShield?.Update(delta);

    public void ChangeShield(Shield newShield) {
        
        if (HeldShield is not null) {
            HeldShield.QueueFree();
            player.DamageableComponent.DamagedBlocked -= HeldShield.Use;
            ShieldRemoved?.Invoke(HeldShield);
        }

        HeldShield = newShield;
        HeldShield.Init();
        ShieldAdded?.Invoke(HeldShield);

        player.DamageableComponent.DamagedBlocked += HeldShield.Use;
    }
}
