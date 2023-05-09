using Godot;
using System.Collections.Generic;
using System.Linq;
using KidoUtils;
using System.Threading.Tasks;

public partial class Gun : Weapon {
    [Export]
    public PackedScene bulletAsset; 
    private Node2D nuzzle;
    public override void _Ready() {
        base._Ready();
        nuzzle = (Node2D) GetNode("Nuzzle");
    }
    
    public override void UpdateWeapon(Vector2 shootTo) {
        hand.LookAt(shootTo);
    }

    public override void Attack() {
 
        KidoUtils.Utils.GetPreloadedScene<BulletFactory>(this, PreloadedScene.BulletFactory)
            .SpawnBullet(bulletAsset)
            .init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletFrom.Player);
        Camera.currentCamera.StartShake((float) DebugHUD.instance.anySlider.Value, 300, 1);
 
    }
}
