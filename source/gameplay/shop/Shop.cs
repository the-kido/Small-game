
using System;
using Godot;

public partial class Shop : Sprite2D {

	[Export]    
	Godot.Collections.Array<ShopItem> shopItems;
	[Export]
	Godot.Collections.Array<float> chances;

	[ExportGroup("Don't edit")]
	[Export]
	Node2D shopItemAnchorsParent;
    Node2D GetItemAnchor (int index) => shopItemAnchorsParent.GetChild<Node2D>(index);

	public override void _Ready() {
		foreach (var item in shopItems) {
			item.ProcessMode = ProcessModeEnum.Disabled;
			item.Visible = false;     
		}

		if (shopItems.Count < 3 || chances.Count < 3) 
			throw new Exception("This 'Shop' doesn't have 3 or more shopItems attached to it");

        // temporary implementaiton
        for (int i = 0; i < 3; i++) {
            ShopItem item = shopItems[i];

            item.GlobalPosition = GetItemAnchor(i).GlobalPosition;
            item.ProcessMode = ProcessModeEnum.Inherit;
			item.Visible = true;    
        }
	}

	// find a way to init 3 items.
	// also have a method that the "shopdropevent" can access to show this shop. 
}
