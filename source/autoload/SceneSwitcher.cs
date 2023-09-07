using Godot;
using System;

namespace Game.Autoload;

public partial class SceneSwitcher : Node {

	public static event Action SceneSwitched;

	private CanvasLayer canvasLayer;
	private AnimationPlayer animationPlayer;
	public override void _Ready(){
		canvasLayer = GetNode("CanvasLayer") as CanvasLayer;
		animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
	}

	private async void ChangeScene(Action changeSceneTo) {
		
		canvasLayer.Layer = 127;
		
		await ShowPanel(true);

		changeSceneTo.Invoke();

		await ToSignal(GetTree().CreateTimer(0), "timeout"); // Wait a frame to let the scene load

		SceneSwitched?.Invoke();
		GD.Print("Don't crash plz");
		
		await ShowPanel(false);

		canvasLayer.Layer = -128;
	}

	SignalAwaiter ShowPanel(bool enable) {
		if (enable) 
			animationPlayer.Play("panel_fade");
		else
			animationPlayer.PlayBackwards("panel_fade");

		return ToSignal(animationPlayer, "animation_finished");
	}

	public void ChangeSceneWithPath(string resourcePath) =>
		ChangeScene(() => GetTree().ChangeSceneToFile(resourcePath));

	public void ChangeSceneWithPackedMap(PackedScene scene) =>
		ChangeScene(() => GetTree().ChangeSceneToPacked(scene));
}
