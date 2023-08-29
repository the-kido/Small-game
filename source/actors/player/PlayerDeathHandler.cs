using Godot;
using System.Threading.Tasks;
using Game.UI;
using Game.Damage;

namespace Game.Players;

public class PlayerDeathHandler {

    readonly Player player;
    
    public PlayerDeathHandler(Player player) =>
        this.player = player;

    public void OnDeath(DamageInstance _) {

        player.GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Playing = true;
        
        PlayImpactFrames(1000);
        Camera.currentCamera.StartShake(300, 300, 2);
        
        //Make player immune to anything and everything all of the time
        FreezePlayer();
    }

    private void FreezePlayer() {
        player.CollisionLayer = 0;
        player.CollisionMask = 0;

        player.CallDeferred("SetProcessMode", false);
    }
    
    private void SetProcessMode(bool enable) =>
        player.ProcessMode = enable ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
    
    public void DamageFramePause(DamageInstance damageInstance) {
        if (!damageInstance.suppressImpactFrames) PlayImpactFrames(300);
    }

    private async void PlayImpactFrames(int milliseconds) {
        player.GetTree().Paused = true;
        await Task.Delay(milliseconds);
        player.GetTree().Paused = false;
    }
}