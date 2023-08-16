using System;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Godot;

public static class DungeonRunData {
    public class Coins : ISaveable {
        public Coins() {
            (this as ISaveable).InitSaveable();
            _count = (int) (this as ISaveable).LoadData();
        }
        
        static Coins() {
            Coins instance = new();
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
        static FreezeOrbs() {
            FreezeOrbs instance = new();
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
            count = (int) (this as ISaveable).LoadData();
        }

        static EnemiesKilled() {
            EnemiesKilled instance = new();
        }

        public SaveData saveData => new("EnemiesKilled", count);
        public static event Action<Enemy> EnemyKilled;
        
        private static int count = 0;

        public void AddDeath(Enemy enemy) {
            EnemyKilled?.Invoke(enemy);
            count += 1;
        }
    }    

}