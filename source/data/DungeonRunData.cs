using System;
using System.Collections.Generic;
using Game.Actors;
using Game.Mechanics;

namespace Game.Data;

public abstract class RunData {

    public  readonly static RunData 
    Coins = new DungeonRunData.Coins(),
    FreezeOrbs = new DungeonRunData.FreezeOrbs(),
    EnemiesKilled = new DungeonRunData.EnemiesKilled(),
    DamageTaken = new DungeonRunData.DamageTaken(), 
    RespawnTokens = new DungeonRunData.RespawnTokens();

    public readonly static Dictionary<RunDataEnum, RunData> AllData = new() {
        {RunDataEnum.Coins, Coins},
        {RunDataEnum.FreezeOrbs, FreezeOrbs},
        {RunDataEnum.EnemiesKilled, EnemiesKilled},
        {RunDataEnum.DamageTaken, DamageTaken},
        {RunDataEnum.RespawnTokens, RespawnTokens},
    };

    public abstract int Count {get; set;}
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

public enum RunDataEnum {
    Coins,
    FreezeOrbs,
    EnemiesKilled,
    DamageTaken,
    RespawnTokens,
}

public static class DungeonRunData {
    public class Coins : RunData { 
        
        public override string ValueName => "Coins";

        public override int Count {
            get => _count;
            set {
                ValueChanged?.Invoke(value);
                _count = value;
            }
        }
    }

    public class FreezeOrbs : RunData {
        public override string ValueName => "FreezeOrbs";

        public FreezeOrbs() : base() => FreezeWave += FreezeOrbMechanic.Freeze;

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
        
        public override int Count { 
            get => _count;
            set {
                _count = (int) MathF.Max(0, value);
                ValueChanged?.Invoke(_count);
            }
        }
    }

    public class DamageTaken : RunData {

        public override string ValueName => "DamageTaken";

        public override int Count { 
            get => _count;
            set {
                _count = (int) MathF.Max(0, value);
                ValueChanged?.Invoke(_count);
            }
        }
    }

     public class RespawnTokens : RunData {

        public override string ValueName => "RespawnTokens";

        public override int Count { 
            get => _count;
            set {
                _count = (int) MathF.Max(0, value);
                ValueChanged?.Invoke(_count);
            }
        }
    }
}