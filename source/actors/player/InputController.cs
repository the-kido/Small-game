using Godot;
using System;
using Game.UI;
using Game.Damage;
using System.Collections.Generic;

namespace Game.Players.Inputs;

public class UIInputFilter {

	public UIInputFilter(Player player) =>
		player.DamageableComponent.OnDeath += SetFilterModeOnDeath;
    public event Action<bool> OnFilterModeChanged = delegate{};
	
	bool _filterNonUiInput = false;	
	public bool FilterNonUiInput => _filterNonUiInput;

	public void SetFilterMode(bool @bool) {
		OnFilterModeChanged?.Invoke(@bool);
		_filterNonUiInput = @bool;
	}

	private void SetFilterModeOnDeath(DamageInstance _) => SetFilterMode(true);

}

public partial class InputController : Node {
	private Player attachedPlayer;
	
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
		UIInputFilter = new(player);
		
		// Init everything required
		DialogueController = new(this, GUI);
		InteractablesButtonController = new(player, GUI);
		MovementController.Init(player, this);
	}

	public void AddInput(IInput input, bool blockedByUI) {
		if (blockedByUI) NonUIInputs.Add(input);
		else UIInputs.Add(input);
	}

	private readonly List<IInput> NonUIInputs = new();
	private readonly List<IInput> UIInputs = new();

	public override void _Process(double delta) {
		if (attachedPlayer is null) return; // Enforce that this node is initialized
		// Allow player to interact with UI even if input is filtered.
		UpdateUIInput(delta);

		if (UIInputFilter.FilterNonUiInput) return;
		
		UpdateNonUIInput(delta);
	}

	private void UpdateUIInput(double delta) {
		DialogueController.Continue();
		InvokeGUILeftClick();

		UIInputs.ForEach(input => input.Update(delta));
	}

	private void UpdateNonUIInput(double delta) {
		MovementController.UpdateMovement();
		WeaponController?.Update(delta);
		ShieldInput?.Update();

		NonUIInputs.ForEach(input => input.Update(delta));
	}
}