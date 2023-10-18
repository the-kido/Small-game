using System;
using Godot;

public partial class Shop : Sprite2D {

	[Export]    
	Godot.Collections.Array<ShopItem> shopItems;
	[Export]
	Godot.Collections.Array<int> chanceWeight;

	[ExportGroup("Don't edit")]
	[Export]
	Node2D shopItemAnchorsParent;
    Node2D GetItemAnchor (int index) => shopItemAnchorsParent.GetChild<Node2D>(index);

	public override void _Ready() {
		foreach (var item in shopItems) {
			item.ProcessMode = ProcessModeEnum.Disabled;
			item.Visible = false;     
		}

		if (shopItems.Count < 3 || chanceWeight.Count < 3) 
			throw new Exception("This 'Shop' doesn't have 3 or more shopItems attached to it");

        RollShopItems();
	}
	private void RollShopItems() {
		// iterate over all items REQUIRED
		for (int shopItemIndex = 0; shopItemIndex < 3; shopItemIndex++) { 
			
			float chance = 0;
			float random = new Random().NextSingle();
			int shopItemCount = shopItems.Count;

			for (int i = 0; i < shopItemCount; i++) {
				chance += (float) chanceWeight[i] / shopItemCount;
				
				if (chance >= random) {
					InitItem(shopItems[i], shopItemIndex);
					// When an item is selected, remove from pool:
					shopItems.RemoveAt(i);
					chanceWeight.RemoveAt(i);
					// Move to next shopItemIndex
					break; 
				}
			}
		}
	}

	private void InitItem(ShopItem item, int index) {
		item.GlobalPosition = GetItemAnchor(index).GlobalPosition;
		item.ProcessMode = ProcessModeEnum.Inherit;
		item.Visible = true;    
	}

	// find a way to init 3 items.
	// also have a method that the "shopdropevent" can access to show this shop. 
}
