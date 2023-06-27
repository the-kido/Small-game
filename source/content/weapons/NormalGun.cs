using Godot;
using System.Collections.Generic;
using System.Linq;
using KidoUtils;
using System.Threading.Tasks;

public partial class NormalGun : Gun {
   
    protected override DamageInstance damage => new(Player) {
        statusEffect = new FireEffect(),
        damage = 5, 
    }; 

    public override Type WeaponType {get; protected set;} = Type.InstantShot;

	protected override BulletInstance BulletInstance() => new(BulletFrom.Player, damage, BulletSpeed.VeryFast);

    public override void Attack() {
        SpawnBulletInstance();
    }
}
