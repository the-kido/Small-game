using System;
using System.Collections.Generic;
using System.Linq;
using Game.LevelContent;
using Game.LevelContent.Criteria;
using Godot;

/// <summary>
/// This class is special in that it will have different level structures
/// depending on when it is entered. So, we have a class for it.
/// </summary>
public partial class CenterChamber : Node {
	[Export]
	Level level;
	[Export]
	// Put this in order of Dungeon, Nature, Tech, ice.
	Godot.Collections.Array<Node> criterionParents;

	// Check what the current region is.
	// depending on the current region, play some thing and then 
	// mark the level as complete (?)
	static int? futureCriterionParentIndex = 0; 

	public static void NotifyForEntery() {	
		// What i want to do:
		//  Depending on the region, get the right value.
		futureCriterionParentIndex = Array.IndexOf(RegionManager.Regions, RegionManager.CurrentRegionName);
	}
	
	public List<LevelCriteria> GetLevelCriterion() {
		return criterionParents[futureCriterionParentIndex ?? 0].GetChildren().Cast<LevelCriteria>().ToList();
	}

	public static bool WithinCenterChamber => Level.CurrentLevel.GetParent() is CenterChamber;  

	public override void _Process(double delta) {
	}
}
