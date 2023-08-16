using System;
using System.Runtime.CompilerServices;
using Godot;

public static class DungeonRunData {

    public class Coins : ISaveable {
        public Coins() {
            (this as ISaveable).InitSaveable();
            _count = (int) (this as ISaveable).LoadData();
        }
        public SaveData saveData => new("Coins", _count);

        public static event Action<int> ValueChanged;
        
        private static int _count = 0;

        public static int Count {
            get => _count;
            set {
                ValueChanged?.Invoke(value);
                _count = value;
            }
        }
    }
    public class FreezeOrbs : ISaveable {
        public FreezeOrbs() {
            _count = (int) (this as ISaveable).LoadData();
        }

        public SaveData saveData => new("FreezeOrbs", _count);
        
        public static event Action FreezeWave;
        private static int _count = 0;
        public static int Count {
            get => _count;
            set {
                if (value >= 3) {
                    FreezeWave?.Invoke();
                    _count = 0;
                } else {
                    _count = value;
                }
            }
        }

    }
    public class EnemiesKilled : ISaveable {
        public EnemiesKilled() {
            _count = (int) (this as ISaveable).LoadData();
        }
        public SaveData saveData => new("EnemiesKilled", _count);

        
        public static int _count = 0;
        public static int Count {
            get => _count;
            set => _count = value;
        }
    }

    static Coins CoinCount {get; set;} = new();
    static FreezeOrbs FreezeOrbCount {get; set;} = new();
    static EnemiesKilled EnemyKillCount {get; set;} = new();
    
}