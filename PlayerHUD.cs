using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerHUD : CanvasLayer
{
    [Export]
    Player player;
    [Export]
    Label healthLable;
    
    string healthLableText = "♥: ";
    public override void _Ready()
    {
        player.DamageableComponent.OnDamaged += UpdateHealth;
        player.DamageableComponent.OnDamaged += DamageFlash;

        UpdateHealth(new DamageInstance(){damage = 0});
    }

    private void UpdateHealth(DamageInstance damage) {
        healthLable.Text = healthLableText + player.DamageableComponent.Health.ToString();
    }

    volatile int percentRed = 0;    
    //Set the flashing to true.
    //If another damage comes in, stop the other flashing and start a new flashing.    
    private async void DamageFlash(DamageInstance _) {
        if (percentRed != 0) {
            percentRed = 100;
            return;
        }
        percentRed = 100;

        Color color = new(1,1,1);
        while (percentRed > 0) {

            color.G = 1 - percentRed / 100f;
            color.B = 1 - percentRed / 100f;
            healthLable.Modulate = color;
            await Task.Delay(3);
            percentRed-=5;
        }
    }
}
