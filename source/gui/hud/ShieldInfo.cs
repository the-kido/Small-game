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
        player.ShieldManager.ShieldChanged += UpdateShield;
        // Well i'd perfer to get it directly from the shield itself
        player.DamageableComponent.DamagedBlocked += UpdateHealth;
    }

    private void EnableShieledUsageIndicator(bool @bool) => usingShildIndicator.Visible = @bool;
    private void UpdateShield(Shield shield) {
        icon.Texture = shield.Icon;
        healthLabel.Text = shield.Health.ToString();
    }
    private void UpdateHealth(DamageInstance damageInstance) {
        healthLabel.Text = player.ShieldManager.HeldShield.Health.ToString();
    }
}