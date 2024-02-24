using Godot;
using KidoUtils;
using Game.Autoload;
using Game.Data;
using Game.LevelContent;

namespace Game.UI;

public partial class MainMenu : Control {

	[Export]
	private Button settings;
	[Export]
	private Button startGame;

	GUI gui;
	
	public override void _Ready() {
		gui = Utils.GetPreloadedScene<GUI>(this, PreloadedScene.GUI);

		startGame.ButtonDown += OnButtonDown;
		gui.Visible = false;
		
		
		settings.Pressed += () => {
			gui.OpenSettingsPage();
			gui.Visible = true;
			gui.settingsPage.Disable += () => gui.Visible = false;
		};
	}
	
	private static PackedScene GetLastLevelLeftOff() {
		string level = (string) Level.lastLevelPlayedSaver.LoadValue();
		
		if (string.IsNullOrEmpty(level))
			level = "res://assets/levels/debug/spawn.tscn"; 

		return ResourceLoader.Load<PackedScene>(level);
	}

	private void OnButtonDown() {
		Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPackedMap(GetLastLevelLeftOff());
		SceneSwitcher.SceneSwitched += OnSceneSwitch; 
	}

	private void OnSceneSwitch() {
		SceneSwitcher.SceneSwitched -= OnSceneSwitch; 
		gui.Visible = true;
	}
}
