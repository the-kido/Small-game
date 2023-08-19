
namespace Game.Players;
public sealed partial class Normal : Player {
    public override void _Ready() {
        base._Ready();
        DamageableComponent.DamageTakenMulitplier = 0.8f;
    }
}