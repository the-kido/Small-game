using Godot;
using System.Collections.Generic;
using Game.Players.Mechanics;
using Game.Actors;
using Game.Players.Inputs;
using Game.UI;
using Game.Data;
using KidoUtils;
using Game.LevelContent;

namespace Game.Players;

public sealed partial class Player : Actor {

	[ExportCategory("Required")]
	[Export]
	public PlayerInteractableRadar InteractableRadar {get; private set;}
	[Export]
	public InputController InputController {get; private set;}
	[Export]
	public WeaponManager WeaponManager;
	[Export]
	public ShieldManager ShieldManager;

	[ExportCategory("Optional")]
	[Export]
	private AudioStreamPlayer2D epicSoundEffectPlayer;

	public GUI GUI {get; private set;}
	public static List<Player> Players {get; private set;}
	
	protected override void SetStats(ActorStats newStats) {
		base.SetStats(newStats);

		// Additional things for player
		WeaponManager.reloadSpeed = newStats.reloadSpeed;
	}
	
	public PlayerClassManager classManager;

	public void Init() {
		_Ready();
		Players = new() { this };
		GUI = Utils.GetPreloadedScene<GUI>(this, PreloadedScene.GUI);
		
		classManager = new(this);

		Camera.CurrentCamera.Init(this);
		
		InputController.Init(this);
		
		WeaponManager.Init(this);
		ShieldManager.Init(this);

		GUI.Init(this);

		PlayerDeathHandler playerDeathHandler = new(this);

		DamageableComponent.OnDamaged += playerDeathHandler.DamageFramePause;
		DamageableComponent.TotalDamageTaken += RunData.DamageTaken.Add;
		DamageableComponent.OnDeath += playerDeathHandler.OnDeath;

		// Make player immune to damage while in dialogue.
		GUI.DialoguePlayer.Started += (_) => DamageableComponent.ChangeImmunity(true);
		GUI.DialoguePlayer.Ended += () => DamageableComponent.ChangeImmunity(false);

		Level.CurrentLevel.LevelCompleted += Effect.ClearAllEffects;

		classManager.SwitchClassFromSave();
	}

	private void SetProcessMode(bool enable) =>
		ProcessMode = enable ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
}
