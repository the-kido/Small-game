using Godot;
using System;

public partial class SceneSwitcher : Node {

	public static event Action SceneSwitched;

	private CanvasLayer canvasLayer;
	private AnimationPlayer animationPlayer;
	public override void _Ready(){
		canvasLayer = (CanvasLayer) GetNode("CanvasLayer");
		animationPlayer = (AnimationPlayer) GetNode("AnimationPlayer");
	}

	private async void ChangeScene(Action changeSceneTo) {
		
		canvasLayer.Layer = 127;
		animationPlayer.Play("panel_fade");
		
		await ToSignal(animationPlayer, "animation_finished");

		changeSceneTo.Invoke();
		SceneSwitched?.Invoke();

		animationPlayer.PlayBackwards("panel_fade");

		await ToSignal(animationPlayer, "animation_finished");

		canvasLayer.Layer = -128;
	}

	public void ChangeSceneWithPath(string resourcePath) {
		ChangeScene(() => GetTree().ChangeSceneToFile(resourcePath));
	}

	public void ChangeSceneWithPackedMap(PackedScene scene) {
		ChangeScene(() => GetTree().ChangeSceneToPacked(scene));
	}
}
