using Godot;
using System.Threading.Tasks;

public partial class HealthLabelTemp : Label {
    
    #region move to seperate "health lable" class
    [Export]
    Label healthLable;

    string healthLableText = "♥: ";
    
    public void UpdateHealth(DamageInstance damage) {
        healthLable.Text = healthLableText + damage.damage.ToString();
        DamageFlash();
    }

    public void Init(int health) => healthLable.Text = healthLableText + health.ToString();

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