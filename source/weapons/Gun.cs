using Godot;
using System.Collections.Generic;
using System.Linq;
using KidoUtils;
using System.Threading.Tasks;

public partial class Gun : Weapon {
    [Export]
    public PackedScene bulletAsset; 
    private Node2D nuzzle => (Node2D) GetNode("Nuzzle");

    public override Type WeaponType {get; protected set;} = Type.InstantShot;

    public override void UpdateWeapon(Vector2 shootTo) {
        hand.LookAt(shootTo);
    }

    DamageInstance damage = new() {
        damage = 5,
        statusEffect = new FireEffect(),
    };
    
    public override void Attack() {
 
        KidoUtils.Utils.GetPreloadedScene<BulletFactory>(this, PreloadedScene.BulletFactory)
            .SpawnBullet(bulletAsset)
            .Init(nuzzle.GlobalPosition, nuzzle.GlobalRotation, BulletFrom.Player, damage);
        Camera.currentCamera.StartShake((float) DebugHUD.instance.anySlider.Value, 300, 1);
    }
    public override void OnWeaponLetGo() {}
}
