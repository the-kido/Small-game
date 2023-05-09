using Godot;
using KidoUtils;
public partial class MainMenu : Control
{

	[Export]
	private Resource level = new Resource();

	private void OnButtonDown() {
		KidoUtils.Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher)	
			.ChangeSceneWithPackedMap((PackedScene) level);
	}
}