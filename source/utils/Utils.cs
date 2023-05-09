using Godot;
using System.Threading.Tasks;

namespace KidoUtils;


public class Utils {

    public static T GetPreloadedScene<T>(Node node, PreloadedScene preloadedScene) where T : GodotObject {

        switch(preloadedScene){
            case PreloadedScene.BulletFactory:
                return node.GetNode<T>("/root/BulletFactory");

            case PreloadedScene.ParticleFactory:
                return node.GetNode<T>("/root/ParticleFactory");

            case PreloadedScene.SceneSwitcher:
                return node.GetNode<T>("/root/SceneSwitcher");

            case PreloadedScene.GlobalCursor:
                return node.GetNode<T>("/root/GlobalCursor");
        }
        return default(T);
    }


    public static bool NodeSeesPoint(Node2D from, Vector2 toPosition, uint blockingMask) {
        PhysicsDirectSpaceState2D spaceState = from.GetWorld2D().DirectSpaceState;
        var ray = PhysicsRayQueryParameters2D.Create(from.GlobalPosition, toPosition, blockingMask);
        var result = spaceState.IntersectRay(ray);

        //if nothing hit the ray, they we good.
        if (result.Count <= 0) {
            return true;
        }
        return false;
    }
}

public enum PreloadedScene {
    BulletFactory,
    ParticleFactory,
    SceneSwitcher,
    GlobalCursor,

}
