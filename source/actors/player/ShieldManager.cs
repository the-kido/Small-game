using Godot;
using System;

// TODO:
// this is a VERY small bug but if there is a chest
// then the shield will regen its heatlh!

public partial class ShieldManager : Node2D { 
    
    public Shield HeldShield {get; private set;}

    public event Action<Shield> ShieldChanged;

    Player player;
    public void Init(Player player) {
        this.player = player;
    }

    public void ChangeShield(Shield newShield) {
        
        if (HeldShield is not null) {
            HeldShield.QueueFree();
            player.DamageableComponent.DamagedBlocked -= HeldShield.Use;
        }

        HeldShield = newShield;
        HeldShield.Init();
        ShieldChanged?.Invoke(HeldShield);

        player.DamageableComponent.DamagedBlocked += HeldShield.Use;
    }
}
