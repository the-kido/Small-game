using Godot;

namespace KidoUtils;

public class Utils {

    public static T GetPreloadedScene<T>(Node node, PreloadedScene preloadedScene) where T : Node => preloadedScene switch {
        PreloadedScene.BulletFactory => node.GetNode<T>("/root/BulletFactory"),
        PreloadedScene.ParticleFactory => node.GetNode<T>("/root/ParticleFactory"),
        PreloadedScene.SceneSwitcher => node.GetNode<T>("/root/SceneSwitcher"),
        PreloadedScene.GlobalCursor => node.GetNode<T>("/root/GlobalCursor"),
        PreloadedScene.DamageTextManager => node.GetNode<T>("/root/DamageTextMultiplier"),
        PreloadedScene.PickupablesManager => node.GetNode<T>("/root/PickupablesManager"),
        PreloadedScene.SettingsPage => node.GetNode<T>("/root/SettingsPage"),
        _ => default,
    };
}

public enum PreloadedScene {
    BulletFactory,
    ParticleFactory,
    SceneSwitcher,
    GlobalCursor,
    DamageTextManager,
    PickupablesManager,
    SettingsPage,
}
