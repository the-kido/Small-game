using Godot;
using System.Threading.Tasks;
using Game.UI;
using Game.Damage;
using Game.Data;
using Game.LevelContent;

namespace Game.Players;

public class PlayerDeathHandler {

    readonly Player player;
    
    public PlayerDeathHandler(Player player) {
        this.player = player;
        ReviveMenu.DeathAccepted += GiveDeathPentaly;
    }

    // hehe
    private void GiveDeathPentaly() {
        RunData.Coins.Set(0);
        RunData.FreezeOrbs.Set(0);

        RegionManager.ResetCurrentRegionData();

		GameDataService.Save();
    }

    public void OnDeath(DamageInstance _) {

        player.GetNode<AudioStreamPlayer2D>("Death Sound").Playing = true;
        
        PlayImpactFrames(1000);
        Camera.CurrentCamera.StartShake(300, 300, 2);
        
        //Make player immune to anything and everything all of the time
        FreezePlayer();
    }

    private void FreezePlayer() {
        player.CollisionLayer = 0;
        player.CollisionMask = 0;

        player.CallDeferred("SetProcessMode", false);
    }
    
    public void DamageFramePause(DamageInstance damageInstance) {
        if (!damageInstance.suppressImpactFrames) PlayImpactFrames(300);
    }

    private async void PlayImpactFrames(int milliseconds) {
        player.GetTree().Paused = true;
        await Task.Delay(milliseconds);
        player.GetTree().Paused = false;
    }
}