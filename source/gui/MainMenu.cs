using Godot;
using System;

public partial class MainMenu : Control
{

	[Export]
	private Resource level = new Resource();

	private void OnButtonDown() {
		GetNode<SceneSwitcher>("/root/SceneSwitcher").ChangeSceneWithPackedMap((PackedScene) level);
	}
}