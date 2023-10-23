using System;
using System.Collections.Generic;
using Game.Actors;
using Game.LevelContent;
using Game.Mechanics;

namespace Game.Data;

public abstract class RunData : ISaveable {
    public readonly static Dictionary<RunDataEnum, RunData> AllData = new() {
        {RunDataEnum.Coins, new DungeonRunData.Coins()},
        {RunDataEnum.FreezeOrbs, new DungeonRunData.FreezeOrbs()},
        {RunDataEnum.EnemiesKilled, new DungeonRunData.EnemiesKilled()},
        {RunDataEnum.DamageTaken, new DungeonRunData.DamageTaken()},
        {RunDataEnum.RespawnTokens, new DungeonRunData.RespawnTokens()},
    };

    public abstract int Count {get; set;}
    public abstract string ValueName {get;}

    protected int _count = 0;

    public Action<int> ValueChanged {get; set;}

    public SaveData SaveData => new(ValueName, _count);    
    public void Add(int value) => Count += value;
    
    public RunData() {
        (this as ISaveable).InitSaveable();
        _count = (int) (this as ISaveable).LoadData();
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