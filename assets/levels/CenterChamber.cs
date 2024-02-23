using System;
using System.Linq;
using Game.Data;
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

	static int? futureCriterionParentIndex = null; 
	public static void NotifyForEnteryOnDeath() {
		// This is only for the first death of the player
		if (RunData.Deaths.Count is 1) futureCriterionParentIndex = 4;  
	}	

	public static void NotifyForEnteryAfterWinning() {	
		int index = (int) RegionManager.CurrentRegion;
		
		if (RegionManager.RegionsWon[index]) { 
			return;
		}

		futureCriterionParentIndex = index;
	}
	
	public override void _Ready() {
		if (futureCriterionParentIndex is null) return;

		level.CompleteAllEvents(0, criterionParents[futureCriterionParentIndex ?? 0].GetChildren().Cast<LevelCriteria>().ToList());

		futureCriterionParentIndex = null;
	}

	public static bool WithinCenterChamber => Level.CurrentLevel.GetParent() is CenterChamber;  
}
