using System;
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
            _count = (int) MathF.Max(0, value);
            ValueChanged?.Invoke(_count);
        }
    }
    
    public abstract string ValueName {get;}

    protected int _count = 0;

    public Action<int> ValueChanged {get; set;}

    public void Add(int value) => Count += value;
    public void Set(int value) => Count = value;

    readonly DataSaver saveable;
    public RunData() {
        saveable = new(() => new(ValueName, _count));
        _count = (int) saveable.LoadValue();
    }
}

public static class DungeonRunData {
    public class Coins : RunData { 
        public override string ValueName => "Coins";
    }

    public class FreezeOrbs : RunData {
        public override string ValueName => "FreezeOrbs";

        static ViewedResource viewedResource;
        public FreezeOrbs() : base() {
            FreezeWave += FreezeOrbMechanic.Freeze;
            viewedResource = new(
                RunDataEnum.FreezeOrbs,
                () => $"{_count}",
                ResourceLoader.Load<Texture2D>("res://assets/enviornment/pickupables/freeze-charge.png")
            );
        }

        public static event Action FreezeWave;
        public override int Count {
            get => _count;
            set {
                if (value >= 3) {
                    FreezeWave?.Invoke();
                    _count = 0;
                } else {
                    ValueChanged?.Invoke(value);
                    _count = value;
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
                () => $"{_count}", 
                ResourceLoader.Load<Texture2D>("res://assets/enviornment/gameplay/shop/respawn_token.png")
            );
        }
    }

    public class Deaths : RunData {
        public override string ValueName => "PlayerDeaths";
    }
}