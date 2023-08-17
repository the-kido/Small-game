using Godot;
using Game.Players;
using Game.Players.Mechanics;

namespace Game.UI;

public partial class ShieldInfo : Control{
    [Export]
    private TextureRect icon;
    [Export]
    private Label healthLabel;

    [Export]
    private Color shieldDisabledModulation;

    [Export]
    private NinePatchRect usingShildIndicator;

    Shield heldShield => player.ShieldManager.HeldShield;

    Player player;
    public void Init(Player player) {
        this.player = player;
        player.InputController.ShieldInput.PlayerShieldsDamage += EnableShieledUsageIndicator;
        player.ShieldManager.ShieldAdded += UpdateNewShield;
        player.ShieldManager.ShieldRemoved += RemoveOldShield;
        // Well i'd perfer to get it directly from the shield itself
        
    }

    private void EnableShieledUsageIndicator(bool @bool) => usingShildIndicator.Visible = @bool;
    private void UpdateNewShield(Shield newShield) {
        Visible = true;
        icon.Texture = newShield.Icon;
        healthLabel.Text = newShield.Health.ToString();
        
        newShield.Updated += UpdateShield;
    }

    private void RemoveOldShield(Shield oldShield) {
        oldShield.Updated -= UpdateShield;
    }

    private Color white = new(1, 1, 1);
    private void UpdateShield() {
        healthLabel.Text = heldShield.Health.ToString();
        
        if (!heldShield.Alive) {
            Modulate = shieldDisabledModulation; 
        }
        else {
            Modulate = new Color(1, 1, 1);
        }
    }
}