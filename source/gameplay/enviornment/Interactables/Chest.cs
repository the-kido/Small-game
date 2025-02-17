using Godot;
using System;
using LootTables;
using Game.Players;
using Game.Players.Mechanics;
using Game.Data;

namespace Game.LevelContent;

public partial class Chest : AnimatedSprite2D {

	[Export]
	ChestTables chestLootTable;

	[Export]
	Interactable interactable;

	[Export]
	Sprite2D itemShowcase;

	IChestItem containedWeapon;

	private static Godot.Collections.Dictionary<string, bool> ChestOpened = new();
	
	static readonly RegionDataSaver chestStatesSaver = 
		new("ChestStates", () => ChestOpened, () => ChestOpened = new());

	private bool LoadChestOpened() {
		var loadedStuff = chestStatesSaver.LoadValue();
		ChestOpened = (Godot.Collections.Dictionary<string, bool>) loadedStuff;
		return ChestOpened.ContainsKey(GetPath()) && ChestOpened[GetPath()];
	}

	public override void _Ready() {
		bool isChestOpened = LoadChestOpened();
		
		if (isChestOpened) {
			Disable(null);
			return;
		}

		ChestOpened[GetPath()] = isChestOpened; 

		interactable.Interacted += OnInteracted;

		float chance = 0;
		float random = new Random().NextSingle();

		foreach (ChestItemDrop item in ChestLootTables.All[chestLootTable]) {

			chance += item.Chance;
			
			if (chance >= random)
				containedWeapon = item.ChestItem.Instantiate<IChestItem>();
		}
	}

	private void SwitchItems(int oldWeaponIndex, Player player) {

		if (containedWeapon.Type is ChestItemType.WEAPON) {
			Weapon containedWeaponInstance = ((Weapon) containedWeapon).PackedScene.Instantiate<Weapon>();
			Weapon oldWeapon = player.WeaponManager.GetWeapon(oldWeaponIndex);

			player.WeaponManager.AddAndSwitchWeapon(containedWeaponInstance, oldWeaponIndex);
			containedWeapon = oldWeapon?.PackedScene.Instantiate<IChestItem>();

			if (containedWeapon is null)
				Disable(containedWeaponInstance.Icon);
			
			return;
		}

		if (containedWeapon.Type is ChestItemType.SHIELD) {

		}
	}

	KidoUtils.Timer timer = KidoUtils.Timer.NONE;
	
	

	private void Disable(Texture2D sprite) {
		ChestOpened[GetPath()] = true;

		interactable.QueueFree();
		SelfModulate = new(0.8f, 0.8f, 0.9f);
		
		if (sprite is not null)
			itemShowcase.Texture = sprite;
		
		if (Level.IsCurrentLevelCompleted())
			GameDataService.Save();

		timer = new(5);
		timer.TimeOver += FullyDisable;
	}

	// fantastic
	private void FullyDisable() {
		itemShowcase.QueueFree();
		timer = KidoUtils.Timer.NONE;
	}

	public override void _Process(double delta) => timer.Update(delta);

	private void OnInteracted(Player player) {
		
		Play("open");

		switch (containedWeapon.Type) {
			case ChestItemType.WEAPON:
				for (int i = 0; i < player.WeaponManager.Weapons.Length; i++) {
					Weapon weapon = player.WeaponManager.Weapons[i];
					if (weapon is null) {
						player.WeaponManager.AddAndSwitchWeapon((Weapon) containedWeapon, i);
						Disable(containedWeapon.Icon);
						return;
					}
				}
				break;
			case ChestItemType.SHIELD:
				if (player.ShieldManager.HeldShield is null) {
					player.ShieldManager.ChangeShield((Shield) containedWeapon);
					Disable(containedWeapon.Icon);
					return;
				}
				break;
		}

		player.GUI.OpenChestMenu(containedWeapon);
		
		player.GUI.chestMenu.OnSelectionMade += (oldWeaponIndex) => Callthing(oldWeaponIndex, player);
		player.GUI.chestMenu.Disable += () => PlayBackwards("open");
	}

	private void Callthing(int oldWeaponIndex, Player player) {

		player.GUI.chestMenu.OnSelectionMade -= (oldWeaponIndex) => Callthing(oldWeaponIndex, player);
		SwitchItems(oldWeaponIndex, player);
	}
}
