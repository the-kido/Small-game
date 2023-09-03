using Godot;
using System.Threading.Tasks;
using Game.Data;
using Game.Players;

namespace Game.LevelContent.Pickupables;

public partial class Coin : Pickupable {
    int _currentCoins = 0;
    int CurrentCoins {
        get => _currentCoins;
        set {
            if (Level.CurrentLevelCompleted())  
                GameDataService.Save();
            
            _currentCoins = value;
        }
    }

    public static PackedScene PackedScene {get; private set;} = ResourceLoader.Load<PackedScene>("res://assets/content/coin.tscn"); 

	protected override void AbsorbPickupable(Player player) {
        // somehow add "money" via HUD
        CurrentCoins -= 1;

        RunData.AllData[RunDataEnum.Coins].Add(1);
        RunData.AllData[RunDataEnum.FreezeOrbs].Add(1);
	}

	protected override void Update(double delta) {
        
    }

	public override async void _Ready() {
        CurrentCoins += 1;

        // make the orb take time for it to eventually be absorbable (this is when the orb is just spawned)
        IsPickupable = false;
        await Task.Delay(750);
        IsPickupable = true;

        Speed = 0;
        for (int i = 0; i < 100; i++) {
            Speed += 0.01f;
            await Task.Delay(10);
        }
	}
}

