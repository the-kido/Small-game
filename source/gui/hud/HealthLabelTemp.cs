using Godot;
using System.Threading.Tasks;
using Game.Players;
using Game.Damage;

namespace Game.UI;

public partial class HealthLabelTemp : Label {
    
    #region move to seperate "health lable" class
    [Export]
    Label healthLable;
    string healthLableText => "♥: " + player.DamageableComponent.Health.ToString();

    private Player player;
    
    public void UpdateHealth(DamageInstance damage) {
        healthLable.Text = healthLableText;
        DamageFlash();
    }

    public void Init(Player player) {
        this.player = player;
        healthLable.Text = healthLableText;

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