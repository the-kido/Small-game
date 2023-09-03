using Game.Data;
using Game.Players;

namespace Game.LevelContent.Pickupables;

public partial class FreezeCharge : Pickupable {

    protected override int AttractionDistance => 100;

    protected override void AbsorbPickupable(Player player) {
        RunData.AllData[RunDataEnum.FreezeOrbs].Add(1);
    }
}
