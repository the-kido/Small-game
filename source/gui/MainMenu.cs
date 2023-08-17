using Godot;
using KidoUtils;
using Game.Autoload;

namespace Game.UI;

public partial class MainMenu : Control{

	[Export]
	private Resource level = new Resource();

	private void OnButtonDown() {
		Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher)	
			.ChangeSceneWithPackedMap((PackedScene) level);
	}
}