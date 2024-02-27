using Godot;
using Game.Players.Mechanics;
using System;

namespace Game.UI;

public partial class ReloadVisual : ProgressBar {
    WeaponManager hand;
    [Export]
    Control bar;

    double barProgress = 0;
    const int SEGMENTS = 5;

    readonly Color glow = new(1.3f, 1.3f, 1.3f);

    // Value is from 0 to 1.
    private void ApplyProgress(double value) {
        double flooredValue = MathF.Floor((float) value * SEGMENTS) / SEGMENTS; // 0 to 5

        Value = flooredValue;
        SelfModulate = flooredValue >= 1 ? glow : Colors.White;
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
            bar.Visible = false;
            return;
        }

        bar.Visible = true;
    }

    private void AttachEvents() {
        hand.WeaponController.UseWeapon += UpdateBar;
        hand.WeaponController.OnWeaponLetGo += ResetBar;
    }

    public void Init(WeaponManager hand) {
        this.hand = hand;

        if (bar.Visible) KidoUtils.ErrorUtils.AvoidIncorrectVisibility(bar, false); 
        
        // Call these initially
        hand.WeaponSwitched += OnWeaponSwitched;
        AttachEvents();
    }
}
