using System;
using System.Collections.Generic;
using Game.Actors;

namespace Game.Data;

public abstract class RunData : ISaveable {
    public static Dictionary<RunDataEnum, RunData> AllData = new() {
        {RunDataEnum.Coins, new DungeonRunData.Coins()},
        {RunDataEnum.FreezeOrbs, new DungeonRunData.FreezeOrbs()},
        {RunDataEnum.EnemiesKilled, new DungeonRunData.EnemiesKilled()},
        {RunDataEnum.DamageTaken, new DungeonRunData.DamageTaken()},
    };

    protected int _count = 0;

    public Action<int> ValueChanged {get; set;}

    public SaveData SaveData => new("Coins", _count);
    
    public RunData() {
        (this as ISaveable).InitSaveable();
        _count = (int) (this as ISaveable).LoadData();
    }
    public void Add(int value) {
        _count += value;
    }

    public abstract int Count {get; set;}
}

public enum RunDataEnum {
    Coins,
    FreezeOrbs,
    EnemiesKilled,
    DamageTaken,
}

public static class DungeonRunData {
    public class Coins : RunData, ISaveable { 
        public override int Count {
            get => _count;
            set {
                ValueChanged?.Invoke(value);
                _count = value;
            }
        }
    }
    public class FreezeOrbs : RunData, ISaveable {
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

    public class EnemiesKilled : RunData, ISaveable {
        public EnemiesKilled() : base() => Enemy.EnemyKilled += (_,_) => Add(1);
        public override int Count { 
            get => _count;
            set {
                _count = (int) MathF.Max(0, value);
                ValueChanged?.Invoke(_count);
            }
        }
    }

    public class DamageTaken : RunData, ISaveable {
        public override int Count { 
            get => _count;
            set {
                _count = (int) MathF.Max(0, value);
                ValueChanged?.Invoke(_count);
            }
        }
    }
}