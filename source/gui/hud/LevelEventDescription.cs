using Game.LevelContent;
using Game.LevelContent.Criteria;
using Godot;

public partial class LevelEventDescription : Control{
    [Export]
    Label label;

    public override void _Ready() {
        Level.CriterionStarted += UpdateLabel;
        label.Text = null;
    }


    private void UpdateLabel(LevelCriteria criteria) {
        if (string.IsNullOrEmpty(criteria.Description)) label.Text = "";

        criteria.Finished += Clear;

        label.Text = criteria.Description;
    }

    private void Clear() {
        label.Text = null;
    }
}