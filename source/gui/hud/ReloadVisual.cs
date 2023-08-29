using Godot;
using Game.Players.Inputs;
using Game.Players.Mechanics;

namespace Game.UI;

public partial class ReloadVisual : ProgressBar {
    WeaponManager hand;

    double barProgress = 0;

    private void OnWeaponSwitched(Weapon newWeapon) {

        if (!newWeapon.UsesReloadVisuals) {
            Visible = false;
            return;
        }

        Visible = true;
    }

    private void UpdateBar(double delta) {
        barProgress += delta;
        ApplyProgress(barProgress / hand.HeldWeapon.EffectiveReloadSpeed);
    }

    private void ResetBar() {
        barProgress = 0;
        ApplyProgress(0);
    }

    private void ApplyProgress(double value) {
        Value = value;
    }

    public void Init(WeaponManager hand) {
        this.hand = hand;
        
        hand.WeaponController.UseWeapon += UpdateBar;
        hand.WeaponController.OnWeaponLetGo += ResetBar;

        hand.WeaponSwitched += OnWeaponSwitched;

        // Call it initially
        OnWeaponSwitched(hand.HeldWeapon);
    }
}
