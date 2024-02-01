using Godot;
using Game.Data;
using Game.Players;

namespace Game.LevelContent.Pickupables;

public partial class Coin : Pickupable {
    private void UpdateCoins() {
        if (Level.IsCurrentLevelCompleted())
            GameDataService.Save();
    }

    public static PackedScene PackedScene {get; private set;} = ResourceLoader.Load<PackedScene>("res://assets/content/coin.tscn"); 

	protected override void AbsorbPickupable(Player player) {
        RunData.Coins.Add(1);
        UpdateCoins();
	}

    KidoUtils.Timer timer = new(0.75);

	protected override void Update(double delta) {
        timer.Update(delta);
    }

	public override void _Ready() {

        IsPickupable = false;
        Speed = 0;

        // make the orb take time for it to eventually be absorbable (this is when the orb is just spawned)
        timer.TimeOver += () => {
            IsPickupable = true;
            timer = new(time: 0.01, cycles: 100);
            timer.TimeOver += () => {
                Speed += 0.01f;
            };
        };
	}
}

