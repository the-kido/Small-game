using Godot;
using System;
using Game.UI;
using Game.Damage;
using System.Collections.Generic;

namespace Game.Players.Inputs;

public class UIInputFilter {

	public UIInputFilter(Player player) {
		player.DamageableComponent.OnDeath += SetFilterModeOnDeath;
		OnFilterModeChanged = null;
	}
	

	public void Die() {
		GD.Print(OnFilterModeChanged?.GetInvocationList().Length, " size");
		OnFilterModeChanged = null;	
	}
	
    public Action<bool> OnFilterModeChanged = null;
	
	bool _filterNonUiInput = false;	
	public bool FilterNonUiInput => _filterNonUiInput;

	public void SetFilterMode(bool @bool) {
		OnFilterModeChanged?.Invoke(@bool);
		_filterNonUiInput = @bool;
		GD.Print("Filter mode set");
	}

	private void SetFilterModeOnDeath(DamageInstance _) => SetFilterMode(true);

}

public partial class InputController : Node {
	private Player attachedPlayer;
	
	private GUI GUI => attachedPlayer.GUI;

	#region GUI
	public event Action LeftClicked; 
	public event Action PressedEscape; 
	private void InvokeGUILeftClick() {
		if (Input.IsActionJustPressed("default_attack")) LeftClicked?.Invoke();
	}
	private void InvokeEscape() {
		if (Input.IsActionJustPressed("escape")) PressedEscape?.Invoke();
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
		GD.Print("how many times is INIT called");
		
		attachedPlayer = player;

		UIInputFilter = new(player);
		
		// Init everything required
		MovementController.Init(player, this);
		DialogueController = new(this, GUI);
		InteractablesButtonController = new(player, GUI);
	}

	private readonly List<IInput> NonUIInputs = new();
	private readonly List<IInput> UIInputs = new();
	
	public void AddInput(IInput input, bool blockedByUI) {
		if (blockedByUI) 
			NonUIInputs.Add(input);
		else 
			UIInputs.Add(input);
	}
	
	public void RemoveInput(IInput input, bool blockedByUI) {
		if (blockedByUI) 
			NonUIInputs.Remove(input);
		else 
			UIInputs.Remove(input);
	}

	public override void _Process(double delta) {
		if (attachedPlayer is null) 
			return; // Enforce that this node is initialized
		
		UpdateUIInput(delta); // Allow player to interact with UI even if input is filtered.

		if (UIInputFilter.FilterNonUiInput) 
			return;
		
		UpdateNonUIInput(delta);
	}

	private void UpdateUIInput(double delta) {
		DialogueController.Continue();
		InvokeGUILeftClick();
		InvokeEscape();

		UIInputs.ForEach(input => input.Update(delta));
	}

	private void UpdateNonUIInput(double delta) {
		MovementController.UpdateMovement();
		WeaponController?.Update(delta);
		ShieldInput?.Update();

		NonUIInputs.ForEach(input => input.Update(delta));
	}
}