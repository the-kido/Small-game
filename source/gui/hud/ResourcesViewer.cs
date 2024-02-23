using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.LevelContent;
using Godot;

public partial class ResourcesViewer : Control {

	static ResourcesViewer instance;

	internal static List<ViewedResource> allViewableResources = new();
	static readonly DataSaver dataSaver = new(() => new("Discovered Resources", discoveredResources));
	static Godot.Collections.Array<string> discoveredResources = new();

	public static void DiscoverNewResource(RunDataEnum runData) {
		discoveredResources.Add(runData.ToString());
		ViewedResource viewedResource = allViewableResources.Find(item => item.runData == runData);
		if (viewedResource.viewableRegions.Length == 0 || viewedResource.viewableRegions.Contains(RegionManager.CurrentRegion)) {
			instance.Duplicate(viewedResource);
			instance.Visible = true;
		}
	}

	[Export]
	TextureRect prototype;
	[Export]
	VBoxContainer vBox;

	public override void _Ready() {
		discoveredResources = (Godot.Collections.Array<string>) dataSaver.LoadValue();
		instance = this;

		RegionManager.RegionSwitched += (regionSwitched) => {
			Level.LevelStarted += () => OnEnterRegion(regionSwitched);
		};

		Visible = false;
	}

    private void OnEnterRegion(Regions regionSwitched) {
		Level.LevelStarted -= () => OnEnterRegion(regionSwitched);

		if (regionSwitched is Regions.Center) {
			Visible = false;
			
			// Eradicate the children
			foreach (var child in vBox.GetChildren()) vBox.RemoveChild(child);
			return;
		}

		foreach (var resource in allViewableResources) {
			// if it's equal to 0, that means that the resource is for everyone
			if (resource.viewableRegions.Length != 0 && !resource.viewableRegions.Contains(regionSwitched)) continue;

			if (discoveredResources.Any(discovered => resource.runData.ToString() == discovered)) Duplicate(resource); 
        }

		if (vBox.GetChildren().Count >= 0) Visible = true;
    }

    private void Duplicate(ViewedResource a) {
        TextureRect dup = prototype.Duplicate() as TextureRect;
        dup.Visible = true; // will update lallalalaleter
        dup.Texture = a.image;
        vBox.AddChild(dup);
    }

	public override void _Process(double delta) {
	}

}

public class ViewedResource {
	public readonly Func<string> getText;
	public readonly Texture2D image;
	public readonly Regions[] viewableRegions;
	public readonly RunDataEnum runData;

	public ViewedResource(RunDataEnum runData, Func<string> getText, Texture2D image, params Regions[] viewableRegions) {
		this.runData = runData;
		this.getText = getText;
		this.image = image;
		this.viewableRegions = viewableRegions;

		ResourcesViewer.allViewableResources.Add(this);
	}
}