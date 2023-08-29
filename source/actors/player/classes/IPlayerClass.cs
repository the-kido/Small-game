using Game.Actors;

namespace Game.Players;

public interface IPlayerClass {
    public void ClassInit(Player player);
    public void ClassRemoved(Player player);
    public PlayerClassResource classResource {get;}
}