using System;
using System.Collections.Generic;
using System.Linq;
using Game.LevelContent;
using Godot;

namespace Game.Data;

public partial class ResourcesViewer : Control {
	[Export]
	TextureRect prototype;
	[Export]
	VBoxContainer vBox;

	static ResourcesViewer viewer;

	internal static List<ViewedResource> allResources = new();
	static readonly DataSaver discoveredResourceSaver = new(() => new("Discovered Resources", discoveredResources));
	static Godot.Collections.Array<string> discoveredResources = new();

	public static void DiscoverNewResource(RunDataEnum runData) {
		if (discoveredResources.Contains(runData.ToString())) return;

		discoveredResources.Add(runData.ToString());
		// Find resource via the RunDataEnum "runData"
		ViewedResource discoveredResource = allResources.Find(item => item.runData == runData);
		
		if (discoveredResource.ViewableEverywhere || discoveredResource.viewableRegions.Contains(RegionManager.CurrentRegion)) {
			viewer.ChangeResourceVisability(discoveredResource, true);
		}
	}

	private bool GetVisibility() {
		if (RegionManager.CurrentRegion is Regions.Center) return false;
		
		// if any child is visible, return true.
		return vBox.GetChildren().Cast<TextureRect>().Any(child => Visible);
	}

	public override void _Ready() {
		viewer = this;
		discoveredResources = (Godot.Collections.Array<string>) discoveredResourceSaver.LoadValue();
		
		RegionManager.RegionSwitched += UpdateResourceVisibility;

		foreach (var a in allResources) AddViewedResource(a);
		
		Visible = GetVisibility();
		
		UpdateResourceVisibility(RegionManager.CurrentRegion); // Call this for when we just enter the game
    }

    private void UpdateResourceVisibility(Regions regionSwitched) {
		if (regionSwitched is Regions.Center) {
			// hide the children
			foreach (var resource in instancedResources.Keys) ChangeResourceVisability(resource, false);
		} else {
			foreach (var resource in allResources) {
				// If the region we're in doesn't show this resource, continue.
				if (!resource.ViewableEverywhere && !resource.viewableRegions.Contains(regionSwitched)) continue;
				// AND, if the resource is inside our discovered resources, then show it.
				if (discoveredResources.Any(discovered => resource.runData.ToString() == discovered)) ChangeResourceVisability(resource, true); 
			}
		}
    }

	private void OnResourcesChanged() {
		Visible = GetVisibility();
	}

	private void ChangeResourceVisability(ViewedResource resource, bool visible) {
		Label label = instancedResources[resource];
		label.GetParent<TextureRect>().Visible = visible;

		OnResourcesChanged();
	}

    private void AddViewedResource(ViewedResource newResource) {
        TextureRect dup = prototype.Duplicate() as TextureRect;
        dup.Texture = newResource.image;

		Label label = dup.GetChild<Label>(0);
		label.Text = newResource.getText();

        vBox.AddChild(dup);
		RunData.GetRunDataFromEnum(newResource.runData).ValueChanged += (_) => UpdateText(newResource);
		instancedResources.Add(newResource, label);

		OnResourcesChanged();
    }

	private static void UpdateText(ViewedResource viewedResource) {
		Label label = instancedResources[viewedResource];
		label.Text = viewedResource.getText();
	}

	static readonly Dictionary<ViewedResource, Label> instancedResources = new();
}

public class ViewedResource {
	public readonly Func<string> getText;
	public readonly Texture2D image;
	public readonly Regions[] viewableRegions;
	public readonly RunDataEnum runData;

	public bool ViewableEverywhere => viewableRegions.Length == 0;

	public ViewedResource(RunDataEnum runData, Func<string> getText, Texture2D image, params Regions[] viewableRegions) {
		this.runData = runData;
		this.getText = getText;
		this.image = image;
		this.viewableRegions = viewableRegions;

		ResourcesViewer.allResources.Add(this);
	}
}