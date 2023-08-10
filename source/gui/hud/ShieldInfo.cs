using Godot;

public partial class ShieldInfo : Control{
    [Export]
    private TextureRect icon;
    [Export]
    private Label healthLabel;

    [Export]
    private NinePatchRect usingShildIndicator;


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
        icon.Texture = newShield.Icon;
        healthLabel.Text = newShield.Health.ToString();
        
        newShield.Updated += UpdateHealth;
    }
    
    private void RemoveOldShield(Shield oldShield) {
        oldShield.Updated -= UpdateHealth;
    }

    private void UpdateHealth() {
        healthLabel.Text = player.ShieldManager.HeldShield.Health.ToString();
    }
}