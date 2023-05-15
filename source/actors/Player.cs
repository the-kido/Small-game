using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed partial class Player : Actor
{
    //Hide speed value for player.

    [Export]
    public PlayerHUD HUD {get; private set;}

    private new int MoveSpeed;

    public List<Actor> NearbyEnemies {get; private set;} = new();
    public static Player[] players {get; private set;} = new Player[4];

    public override void _Ready () {
        base._Ready();

        //Default some values
        HUD.ConnectedPlayer = this;
        players[0] = this;
    }   
    
    public override void OnDeath (DamageInstance damageInstance) {
        PlayFreezeFrame(1000);
    }

    public override void OnDamaged (DamageInstance damageInstance) {
        PlayFreezeFrame(300);
    }

    private async void PlayFreezeFrame (int milliseconds) {
        GetTree().Paused = true;
        await Task.Delay(milliseconds);
        GetTree().Paused = false;
    }

    #region signals
    private void OnNearbyEnemyAreaEntered(Node2D body) =>
        NearbyEnemies.Add((Actor) body);
    private void OnNearbyEnemyAreaExited(Node2D body) =>
        NearbyEnemies.Remove((Actor) body);

    #endregion
}
