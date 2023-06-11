using Godot;
using System.Threading.Tasks;

public partial class DamageTextManager : Node {
    public static DamageTextManager instance;
    public DamageTextManager() {
        instance = this;
    }

    public async void AddDamageText(DamageText damageText) {
        AddChild(damageText);
        await Task.Delay(700);

        damageText.QueueFree();
    }
}
