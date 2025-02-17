using Godot;
using Game.Players;
using Game.Players.Mechanics;

namespace Game.UI;

public partial class ShieldInfo : Control {
    [Export]
    private TextureRect icon;
    [Export]
    private Label healthLabel;
    [Export]
    private Color shieldDisabledModulation;
    [Export]
    private NinePatchRect usingShildIndicator;

    Shield HeldShield => player.ShieldManager.HeldShield;

    Player player;

    public void Init(Player player) {
        this.player = player;

        if (player.InputController.ShieldInput is null) 
            return;

        player.InputController.ShieldInput.PlayerShieldsDamage += EnableShieledUsageIndicator;
        
        if (player.ShieldManager is null) 
            return;

        player.ShieldManager.ShieldAdded += UpdateNewShield;
        player.ShieldManager.ShieldRemoved += RemoveOldShield;

        // In case of race condition
        if (player.ShieldManager.HeldShield is not null) 
            UpdateNewShield(player.ShieldManager.HeldShield);
    }

    private void EnableShieledUsageIndicator(bool @bool) => usingShildIndicator.Visible = @bool;
    
    private void UpdateNewShield(Shield newShield) {
        
        if (newShield is null) { 
            Disable();
            return;
        }

        Visible = true;
        icon.Texture = newShield.Icon;
        healthLabel.Text = newShield.Health.ToString();
        
        newShield.Updated += UpdateShield;
    }

    private void Disable() {
        Visible = false;
    }

    private void RemoveOldShield(Shield oldShield) =>
        oldShield.Updated -= UpdateShield;

    private readonly Color white = new(1, 1, 1);
    
    private void UpdateShield() {
        healthLabel.Text = HeldShield.Health.ToString();
        Modulate = HeldShield.Alive ? white : shieldDisabledModulation;
    }
}