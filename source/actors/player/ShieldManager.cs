using Game.Actors;
using Game.Damage;
using Game.Data;
using Godot;
using System;

namespace Game.Players.Mechanics;

// TODO:
// this is a VERY small bug but if there is a chest
// then the shield will regen its heatlh!
public partial class ShieldManager : Node2D { 
    
    public Shield HeldShield {get; private set;}

    private DataSaver dataSaver;

    public event Action<Shield> ShieldAdded;
    public event Action<Shield> ShieldRemoved;

    Damageable playerDamageableComponent;
    
    private void SwitchToDefaultShield(IPlayerClass playerClass) {
        PackedScene defaultShield = playerClass.classResource.defaultShield;
        
        if (defaultShield is null) 
            RemoveShield();
        else
            ChangeShield(defaultShield.Instantiate<Shield>());
    }

    private void LoadShieldFromSave() {
        string resourcePath = (string) dataSaver.LoadValue();
        Shield loadedShield = string.IsNullOrEmpty(resourcePath) ? null : ResourceLoader.Load<PackedScene>(resourcePath).Instantiate<Shield>();
        
        if (loadedShield is null) 
            return;

        ChangeShield(loadedShield);
    }

    public void Init(Player player) {
        dataSaver = new("Shield", () => HeldShield?.Resource.ResourcePath, () => HeldShield = null);
        playerDamageableComponent = player.DamageableComponent;
        player.InputController.ShieldInput = new(player);

        player.classManager.ClassSwitched += SwitchToDefaultShield;

        LoadShieldFromSave();
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
            playerDamageableComponent.DamagedBlocked -= HeldShield.Use;
            ShieldRemoved?.Invoke(HeldShield);
        }

        HeldShield = newShield;
        HeldShield.Init();
        ShieldAdded?.Invoke(HeldShield);

        playerDamageableComponent.DamagedBlocked += HeldShield.Use;
    }
}
