using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Game.Players;
using Godot;

public partial class Shop : Node2D {

	[Export]    
	Godot.Collections.Array<ShopItem> shopItems;
	[Export]
	Godot.Collections.Array<int> chanceWeight;

	[ExportGroup("Don't edit")]
	[Export]
	Node2D shopItemAnchorsParent;
	[Export]
	AnimationPlayer animationPlayer;

    Node2D GetItemAnchor (int index) => shopItemAnchorsParent.GetChild<Node2D>(index);
    Label GetItemPriceLabel (int index) => GetItemAnchor(index).GetChild<Label>(0);


	// Drop then Stop the Shop
	public void Drop() {
		Visible = true;
		animationPlayer.Play("Land");
	}

	public void Stop() {
		animationPlayer.Play("Pull Up");
		
		foreach(ShopItem shopItem in shopItems) {
			shopItem.DisconnectEvents(Player.Players[0]);
		}
	}

	public override void _Ready() {
		Visible = false;

		foreach (var item in shopItems) {
			item.ProcessMode = ProcessModeEnum.Disabled;
			item.Visible = false;     
		}

		if (shopItems.Count < 3 || chanceWeight.Count < 3) 
			throw new Exception("This 'Shop' doesn't have 3 or more shopItems attached to it");

		foreach(ShopItem shopItem in shopItems) {
			shopItem.ProcessMode = ProcessModeEnum.Disabled;
		}

        RollShopItems();
	}

    private void RollShopItems() {

		List<int> skipped = new();

		
		// iterate over all items REQUIRED
		for (int shopItemIndex = 0; shopItemIndex < 3; shopItemIndex++) { 
			
			float chance = 0;
			float random = new Random().NextSingle();
			int shopItemCount = shopItems.Count - skipped.Count;

			for (int i = 0; i < 3; i++) {
				
				if (skipped.Contains(i)) continue;

				chance += (float) chanceWeight[i] / shopItemCount;
				
				if (chance >= random) {
					InitItem(shopItems[i], shopItemIndex);
					// When an item is selected, remove from pool:
					skipped.Add(i);
					// // Move to next shopItemIndex
					break; 
				}
			}
		}
	}

    private void InitItem(ShopItem item, int index) {
		Node2D anchor = GetItemAnchor(index);

		item.GlobalPosition = anchor.GlobalPosition;
		item.Reparent(anchor, true);

		item.ProcessMode = ProcessModeEnum.Inherit;
		item.Visible = true;
		GetItemPriceLabel(index).Text = $"${item.Price}"; 
	}

	// find a way to init 3 items.
	// also have a method that the "shopdropevent" can access to show this shop. 
}
