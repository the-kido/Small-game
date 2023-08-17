using Godot;

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
            
            case PreloadedScene.DamageTextManager:
                return node.GetNode<T>("/root/DamageTextMultiplier");

            case PreloadedScene.PickupablesManager:
                return node.GetNode<T>("/root/PickupablesManager");
        }

        return default;
    }
}

public enum PreloadedScene {
    BulletFactory,
    ParticleFactory,
    SceneSwitcher,
    GlobalCursor,
    DamageTextManager,
    PickupablesManager,
}
