using Godot;


public partial class ReloadVisual : ProgressBar {
    [Export]
    Node2D hand;

    Weapon weapon => hand.GetChild<Weapon>(0);
    
    double barProgress = 0;

    private void UnsubToEvents(Weapon oldWeapon) {
        oldWeapon.WeaponSwitched -= OnWeaponSwitched;

        InputController inputController = hand.GetNode<InputController>("../Input Controller");
        inputController.UseWeapon -= UpdateBar;
        inputController.OnWeaponLetGo -= ResetBar;
    }

    public void OnWeaponSwitched(Weapon oldWeapon, Weapon newWeapon) {
        SetUp(newWeapon);
        UnsubToEvents(oldWeapon);
    }

    public void SetUp(Weapon newWeapon) {
        newWeapon.WeaponSwitched += OnWeaponSwitched;

        if (!newWeapon.UsesReloadVisuals) {
            Visible = false;
            return;
        }

        Visible = true;

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

    public override void _Ready() => SetUp(weapon);

}
