namespace KidoUtils;

enum Layers: uint {
    Environment = 1,
    Player = 2,
    Enemies = 4,
    PlayerProjectile = 8,
    EnemyProjectile = 16,
}
/// <summary>
/// Defined actors can summon bullets. Therefor, each enum will hold the layers/mask that the 
/// Summoner would hit. For example, a player would hit enemies. Enemies would hit players.
///  
/// </summary>
public enum BulletFrom {
    Player,
    Enemy,
}
