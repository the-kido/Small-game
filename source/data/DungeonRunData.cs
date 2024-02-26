using System;
using System.Threading.Tasks;
using Game.Actors;
using Game.Mechanics;
using Godot;

namespace Game.Data;

public enum RunDataEnum {
    Coins,
    FreezeOrbs,
    EnemiesKilled,
    DamageTaken,
    RespawnTokens,
    PlayerDeaths,
}

public abstract class RunData {

    public readonly static RunData 
    Coins = new DungeonRunData.Coins(),
    FreezeOrbs = new DungeonRunData.FreezeOrbs(),
    EnemiesKilled = new DungeonRunData.EnemiesKilled(),
    DamageTaken = new DungeonRunData.DamageTaken(), 
    RespawnTokens = new DungeonRunData.RespawnTokens(),
    Deaths = new DungeonRunData.Deaths();

    public static RunData GetRunDataFromEnum(RunDataEnum runDataEnum) => runDataEnum switch {
        RunDataEnum.Coins => Coins,
        RunDataEnum.FreezeOrbs => FreezeOrbs,
        RunDataEnum.EnemiesKilled => EnemiesKilled,
        RunDataEnum.DamageTaken => DamageTaken,
        RunDataEnum.RespawnTokens => RespawnTokens,
        RunDataEnum.PlayerDeaths => Deaths,
        _ => default
    };

    public virtual int Count { 
        get => _count;
        set {
            int old = _count;
            _count = (int) MathF.Max(0, value);
            ValueChanged?.Invoke(old, value);
        }
    }
    
    public abstract string ValueName {get;}

    protected int _count = 0;

    // Before and after values
    public Action<int, int> ValueChanged {get; set;}

    public void Add(int value) => Count += value;
    public void Set(int value) => Count = value;

    readonly DataSaver saver;
    public RunData() {
        saver = new(ValueName, () => _count, () => _count = 0);
        _count = (int) saver.LoadValue();
    }
}

public static class DungeonRunData {
    public class Coins : RunData { 
        public override string ValueName => "Coins";
    }

    public class FreezeOrbs : RunData {
        public override string ValueName => "FreezeOrbs";

        static ViewedResource viewedResource;
        bool temporarilyThree = false;

        static string NormalText(int value) => $"x{value}/3";
        static string FancyText(int value) => $"[wave][color=85ffff]{NormalText(value)}";
        string GetText => !temporarilyThree ? NormalText(_count) : FancyText(3);

        async void ShowViewerAsThreeForABit() {
            temporarilyThree = true;
            viewedResource.UpdateText(valueGained: true);
            await Task.Delay(5000);
            temporarilyThree = false;
            viewedResource.UpdateText(valueGained: false);
        }
        
        public FreezeOrbs() : base() {
            FreezeWave += FreezeOrbMechanic.Freeze;
            FreezeWave += ShowViewerAsThreeForABit;

            viewedResource = new (
                RunDataEnum.FreezeOrbs,
                () => GetText,
                ResourceLoader.Load<Texture2D>("res://assets/enviornment/pickupables/freeze-charge.png")
            );
        }

        public static event Action FreezeWave;
        public override int Count {
            get => _count;
            set {
                int old = _count;
                if (value >= 3) {
                    FreezeWave?.Invoke();
                    _count = 0;
                    ValueChanged?.Invoke(2, 3);
                } else {
                    _count = value;
                    ValueChanged?.Invoke(old, value);
                }
            }
        }
    }

    public class EnemiesKilled : RunData {
        public override string ValueName => "EnemiesKilled";
        public EnemiesKilled() : base() => Enemy.EnemyKilled += (_,_) => Add(1);
    }

    public class DamageTaken : RunData {
        public override string ValueName => "DamageTaken";
    }

    public class RespawnTokens : RunData {
        public override string ValueName => "RespawnTokens";

        public RespawnTokens () : base() {
            ViewedResource resourcesViewer = new(
                RunDataEnum.RespawnTokens,
                () => $"x{_count}", 
                ResourceLoader.Load<Texture2D>("res://assets/enviornment/gameplay/shop/respawn_token.png")
            );
        }
    }

    public class Deaths : RunData {
        public override string ValueName => "PlayerDeaths";
    }
}