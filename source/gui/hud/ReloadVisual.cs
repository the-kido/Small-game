using Godot;
using System;

public partial class ReloadVisual : ProgressBar {
    [Export]
    Node2D hand;
    
    Weapon weapon => hand.GetChild<Weapon>(0);

    public override void _Ready() {
        // init if there's already a held weapon.
        SubcribeToEvents(weapon);
    }

    double barProgress = 0;

    private void UnsubToEvents(Weapon oldWeapon) {
        InputController inputController = hand.GetNode<InputController>("../Input Controller");
        inputController.UseWeapon -= (_) => barProgress += oldWeapon.GetProcessDeltaTime();
    }

    public void SubcribeToEvents(Weapon newWeapon) {
        // the other one gets destroyed, so it automagically unsubs. Sub the new weapon now.
        newWeapon.WeaponAdded += SubcribeToEvents;
        newWeapon.WeaponRemoved += UnsubToEvents;

        InputController inputController = hand.GetNode<InputController>("../Input Controller");

        inputController.UseWeapon += UpdateBar;
        inputController.OnWeaponLetGo += ResetBar;
    }

    private void UpdateBar(double delta) {
        barProgress += delta;
        ApplyProgress(barProgress / weapon.ReloadSpeed);
    }

    private void ResetBar() {
        barProgress = 0;
        ApplyProgress(0);
    }

    public void ApplyProgress(double value) {
        Value = value;
    }
}
