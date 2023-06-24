using Godot;
using System.Collections.Generic;
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
    
}