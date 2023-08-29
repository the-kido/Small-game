using Game.Actors;
using Game.Data;
using Godot;
using System;

namespace Game.Players.Mechanics;

// TODO:
// this is a VERY small bug but if there is a chest
// then the shield will regen its heatlh!
public partial class ShieldManager : Node2D, ISaveable { 
    
    public Shield HeldShield {get; private set;}

    public SaveData SaveData => new("Shield", HeldShield?.Resource.ResourcePath);

    public event Action<Shield> ShieldAdded;
    public event Action<Shield> ShieldRemoved;

    Player player;
    
    private void SwitchToDefaultShield(PlayerClass playerClass) {
        PackedScene defaultShield = playerClass.PlayerClassResource.defaultShield;
        
        if (defaultShield is null) 
            RemoveShield();
        else
            ChangeShield(defaultShield.Instantiate<Shield>());
    }

    private Shield LoadShieldFromSave() {
        string resourcePath = (string)(this as ISaveable).LoadData();

        return string.IsNullOrEmpty(resourcePath) ? null : ResourceLoader.Load<PackedScene>(resourcePath).Instantiate<Shield>();
    }

    public void Init(Player player, PlayerClassResource playerClassResource) {
        this.player = player;
        player.InputController.ShieldInput = new(player);

        PlayerManager.ClassSwitched += SwitchToDefaultShield;

        (this as ISaveable).InitSaveable();
        HeldShield = LoadShieldFromSave();
        
        if (HeldShield is null) {
            if (playerClassResource.defaultShield is not null) {
                Shield shield = playerClassResource.defaultShield.Instantiate<Shield>();
                ChangeShield(shield);
            }
        } else {
            ChangeShield(HeldShield);
        }
    }

    public override void _Process(double delta) => HeldShield?.Update(delta);

    public void RemoveShield() {
        if (HeldShield is not null)
            ShieldRemoved?.Invoke(HeldShield);
        
        HeldShield = null;
        ShieldAdded?.Invoke(HeldShield);
    }

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
