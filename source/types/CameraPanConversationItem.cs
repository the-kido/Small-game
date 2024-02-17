using Game.UI;

namespace Game.Characters;

public partial class CameraPanConversationItem : ConversationItem {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}

	public override void _Process(double delta) {
	}
}

public class CameraPanPlayer {

	public void Start(CameraPanConversationItem item) {

	}

	readonly ConversationController conversationController;
	readonly DialogueBar bar;
	public CameraPanPlayer (ConversationController conversationController, DialogueBar bar) {
        this.conversationController = conversationController;
        this.bar = bar;
    }
}
