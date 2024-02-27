using Godot;
using System.Threading.Tasks;
using Game.Players;
using Game.Damage;

namespace Game.UI;

public partial class HealthHud : Control {
    
    #region move to seperate "health lable" class
    [Export]
    RichTextLabel healthLable;
    string HealthLableText => $"♥: {player.DamageableComponent.Health}[color=gray] / {player.DamageableComponent.EffectiveHealth}";

    private Player player;
    
    public void UpdateHealth(DamageInstance damage) {
        healthLable.Text = HealthLableText;
        DamageFlash();
    }

    public void Init(Player player) {
        this.player = player;
        healthLable.Text = HealthLableText;

        player.DamageableComponent.OnDamaged += UpdateHealth;
    } 

    volatile int percentRed = 0;    
    //Set the flashing to true.
    //If another damage comes in, stop the other flashing and start a new flashing.    
    private async void DamageFlash() {
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
    #endregion
}