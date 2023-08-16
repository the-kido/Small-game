using System;

public static class DungeonRunData {

    public static event Action<int> CoinValueChanged;
    private static int _coins = 0;
    public static int Coins {
        get => _coins;
        set {
            CoinValueChanged?.Invoke(value);
            _coins = value;
        }
    }

    public static event Action FreezeWave;
    private static int _freezeOrbs = 0;
    public static int FreezeOrbs {
        get => _freezeOrbs;
        set {
            if (value >= 3) {
                FreezeWave?.Invoke();
                _freezeOrbs = 0;
            } else {
                _freezeOrbs = value;
            }
        }
    }

    public static int _enemiesKilled;
    public static int EnemiesKilled {
        get => _enemiesKilled;
        set => _enemiesKilled = value;
    }
    

    
}