using Godot;
using Game.Players.Mechanics;

namespace Game.UI;

public partial class ReloadVisual : ProgressBar {
    WeaponManager hand;

    double barProgress = 0;

    readonly Color glow = new(1.3f, 1.3f, 1.3f);

    private void ApplyProgress(double value) {
        Value = value;
        SelfModulate = Value >= 1 ? glow : Colors.White;
    }       
    
    private void UpdateBar(double delta) {
        barProgress += delta;
        ApplyProgress(barProgress / hand.HeldWeapon.EffectiveReloadSpeed);
    }

    private void ResetBar() {
        barProgress = 0;
        ApplyProgress(0);
    }

    private void OnWeaponSwitched(Weapon newWeapon) {

        AttachEvents();

        if (!newWeapon.UsesReloadVisuals) {
            Visible = false;
            return;
        }

        Visible = true;
    }

    private void AttachEvents() {
        hand.WeaponController.UseWeapon += UpdateBar;
        hand.WeaponController.OnWeaponLetGo += ResetBar;
    }

    public void Init(WeaponManager hand) {
        this.hand = hand;
        
        // Call these initially
        hand.WeaponSwitched += OnWeaponSwitched;
        AttachEvents();
    }
}
