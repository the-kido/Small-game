using Game.Players;
using System;
using Game.Actors;
using Game.SealedContent;

namespace Game.Data;

public class PlayerClassManager : ISaveable {
    public SaveData SaveData => 
        new("PlayerClass", playerClass?.GetType().ToString());
    
    readonly Player player;

    public IPlayerClass playerClass {get; private set;} = PlayerClasses.normal;

    public PlayerClassManager(Player player) {
        this.player = player;
        (this as ISaveable).InitSaveable();
        ClassSwitched = null;
    }

    public void SwitchClassFromSave() {
        string className = (string) (this as ISaveable).LoadData();
        
        if (string.IsNullOrEmpty(className)) {
            SetClass(new Normal());
            return;
        }

        Type playerClassType = Type.GetType(className);
        
        if (playerClassType is null)  {
            SetClass(new Normal());
            return;
        }

        IPlayerClass playerClassInstance = (IPlayerClass) Activator.CreateInstance(playerClassType);
        
        SetClass(playerClassInstance);
    }

    /// <summary>
    /// Invoked after the player class is switched. It's invocation list is cleared when the game scene changes.
    /// </summary>
    public event Action<IPlayerClass> ClassSwitched;
    
    public void SwitchClass(IPlayerClass newClass) {
        // Remove old player class
        playerClass?.ClassRemoved(player);
        
        SetClass(newClass);

        GameDataService.Save(); // Save the change in class
        ClassSwitched?.Invoke(newClass);
    }

    private void SetClass(IPlayerClass newClass) {
        // Set new class
        playerClass = newClass;
        playerClass.classResource.DoSafetyChecks(); // Do safety checks first because I can't do this any better.
        
        playerClass.ClassInit(player);
        
        UpdateSpritesFromResource(playerClass.classResource);
    }

    private void UpdateSpritesFromResource(PlayerClassResource playerClassResource) =>
        player.sprite.SpriteFrames = playerClassResource.playerSprites;

}