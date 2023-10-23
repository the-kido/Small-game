using Godot;
using System;
using Game.Players.Mechanics;
using Game.UI;

namespace Game.Players.Inputs;

public interface IInput {
	void Update(double delta);
}

#region Player required
public class ShieldInput  {
	readonly Player player;

	public ShieldInput(Player player) {
		this.player = player;
	}
	public event Action<bool> PlayerShieldsDamage;
	bool cache; 
	public void Update() {
		if (cache != IsBlockingDamage()) {
			cache = IsBlockingDamage();
			player.DamageableComponent.BlocksDamage = IsBlockingDamage();
			PlayerShieldsDamage?.Invoke(cache);
		}
	}

	private bool IsBlockingDamage() {
		if (player.ShieldManager.HeldShield is null) 
			return false;

		if (!player.ShieldManager.HeldShield.Alive)
			return false;
		
		if (Input.IsActionPressed("utility_used"))
			return true;
		
		return false;
	}
}

public class HeldItemInputController {
	readonly WeaponManager hand;
	public HeldItemInputController(WeaponManager hand) {
		this.hand = hand;
	}

	public void Update() {
		if (Input.IsActionJustPressed("select_slot_1"))
			hand.SwitchHeldWeapon(0);

		if (Input.IsActionJustPressed("select_slot_2"))
			hand.SwitchHeldWeapon(1);
		
		if (Input.IsActionJustPressed("select_slot_3"))
			hand.SwitchHeldWeapon(2);
	}
}

public class DialogueController {
	readonly InputController inputController;
	readonly GUI GUI;
	public DialogueController(InputController inputController, GUI GUI) {
		this.inputController = inputController;
		this.GUI = GUI;
		// TODO: I don't like having to use this event :(
		DialogueControlInit();
	}

	private bool WithinDialogueBar() {
		var rect = GUI.DialogueBar.GetGlobalRect();
		var mouse_position = GUI.DialogueBar.GetGlobalMousePosition();
		return rect.HasPoint(mouse_position);
	}
	private void DialogueControlInit() {

		GUI.DialoguePlayer.Started += (info) => {
			inputController.UIInputFilter.SetFilterMode(info.PausePlayerInput);
		};
		
		GUI.DialoguePlayer.Ended += () => 
			inputController.UIInputFilter.SetFilterMode(false);
	}
	
	public void Continue() { 
		if (Input.IsActionJustPressed("default_attack") && WithinDialogueBar()) GUI.DialoguePlayer.Clicked?.Invoke();
	}	
}

public class InteractablesButtonController {

	readonly Player player;
	public InteractablesButtonController(Player player, GUI GUI) {
		this.player = player;
		GUI.InteractButton.Pressed += InvokeInteracted;
	}

	public event Action<Player> Interacted;

	public void InvokeInteractPressed() {
		if (Input.IsActionJustPressed("interaction_made")) 
			InvokeInteracted();
	}

	private void InvokeInteracted() {
		// this is a seperate method incase I wanna add some more logic to this sometime
		Interacted?.Invoke(player);
	}

	public void ClearInteractionInvokations() {
		Interacted = null;
	}
}

#endregion Player required
