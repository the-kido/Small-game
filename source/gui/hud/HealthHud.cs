using Godot;
using Game.Players;
using Game.Damage;

namespace Game.UI;

public partial class HealthHud : Control {
    [Export]
    RichTextLabel healthLable;
    [Export]
    AnimationPlayer animationPlayer;

    [Export]
    ProgressBar actualHealth, healthDifference;

    [Export]
    LowHealthShader lowHealthShader;

    string HealthLableText => $"♥: {player.DamageableComponent.Health}[color=gray] / {player.DamageableComponent.EffectiveMaxHealth}";

    private Player player;
    const float SPEED = 0.5f;

    // While negative, it is interpolating until it stops at -1
    double showDifferenceDelay = -2;
    double difference;
    
    public void DecreaseHealth(DamageInstance damage) {
        healthLable.Text = HealthLableText;
        animationPlayer.Play("red_flash");

        if (showDifferenceDelay <= -1) healthDifference.Value = actualHealth.Value;
        healthDifferenceValue = healthDifference.Value;

        actualHealth.Value = (float) player.DamageableComponent.Health / player.DamageableComponent.EffectiveMaxHealth;
        difference = (float) healthDifference.Value - actualHealth.Value; 

        showDifferenceDelay = SPEED;
    }
    public void IncreaseHealth(int increase) {
        healthLable.Text = HealthLableText;
        animationPlayer.Play("red_flash");
        // temporary fix to heatlh nto increasing on bar
        healthDifference.Value = player.DamageableComponent.Health;
    }

    public void Init(Player player) {
        this.player = player;
        healthLable.Text = HealthLableText;

        lowHealthShader.Init(player);

        player.DamageableComponent.OnDamaged += DecreaseHealth;
        player.DamageableComponent.OnHealed += IncreaseHealth;

        healthDifference.Value = player.DamageableComponent.Health;
        actualHealth.Value = player.DamageableComponent.Health / player.DamageableComponent.EffectiveMaxHealth;
    }

    // Required because of innaccuracies in how Godot handles ProgressBars
    double healthDifferenceValue = 0;
    public override void _Process(double delta) {
        showDifferenceDelay -= delta;
        if (showDifferenceDelay <= 0 && showDifferenceDelay >= -SPEED) {
            healthDifferenceValue -= difference * delta * (1 / SPEED);
            healthDifference.Value = healthDifferenceValue;
        }
    }
}