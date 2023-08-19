using Godot;
using System;
using Game.Players.Mechanics;
using Game.UI;
using Game.Damage;

namespace Game.Players.Inputs;

public struct UIInputFilter {
	public UIInputFilter(Player player) =>
		player.DamageableComponent.OnDeath += SetFilterModeOnDeath;

	private void SetFilterModeOnDeath(DamageInstance _) => SetFilterMode(true);
	
	public void SetFilterMode(bool @bool) {
		OnFilterModeChanged?.Invoke(@bool);
		_filterNonUiInput = @bool;
	}

	bool _filterNonUiInput = false;
	public bool FilterNonUiInput => _filterNonUiInput;

    public event Action<bool> OnFilterModeChanged = delegate{};
}

public partial class InputController : Node {
	private Player attachedPlayer;
	private WeaponManager hand => attachedPlayer.WeaponManager;
	
	private GUI GUI => attachedPlayer.GUI;

	#region GUI
	public event Action LeftClicked; 
	private void InvokeGUILeftClick() {
		if (Input.IsActionJustPressed("default_attack")) LeftClicked?.Invoke();
	}
	#endregion

	public UIInputFilter UIInputFilter {get; private set;}
	// Exposed components
	public WeaponController WeaponController {get; set;}
	public ShieldInput ShieldInput {get; set;}

	[Export]
	public MovementController MovementController {get; private set;}
	public DialogueController DialogueController {get; private set;}
	public InteractablesButtonController InteractablesButtonController {get; private set;} // TODO: Rename

	public void Init(Player player) {
		attachedPlayer = player;
		UIInputFilter = new();
		
		// Init everything required
		DialogueController = new(this, GUI);
		InteractablesButtonController = new(player, GUI);
		MovementController.Init(player, this);

	}
	public override void _Process(double delta) {
		if (attachedPlayer is null) return; // Enforce that this node is initialized
		// Allow player to interact with UI even if input is filtered.
		UpdateUIInput(delta);

		if (UIInputFilter.FilterNonUiInput) return;
		
		UpdateNonUIInput(delta);
	}

	private void UpdateUIInput(double _) {
		DialogueController.Continue();
		InvokeGUILeftClick();
	}

	private void UpdateNonUIInput(double delta) {
		MovementController.UpdateMovement();
		
		WeaponController?.Update(delta);
		ShieldInput?.Update();
	}
}