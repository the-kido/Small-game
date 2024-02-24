using Game.Actors;
using Godot;

namespace KidoUtils;

public static class ErrorUtils {
    static string Message1(string objectName) => "The CollisionObject2D " + objectName + " does not have any collision layers/masks selected";
public static void AvoidEmptyCollisionLayers(CollisionObject2D collisionObject) {
        if (collisionObject.CollisionLayer == 0 || collisionObject.CollisionMask == 0) {
            string message = Message1(collisionObject.Name);

            Node parent = collisionObject.GetParent();
            if (parent is not null)
                message = $"For child node of {parent.Name}: {message}";

            GD.PushWarning(message);
        }
    }

    static string Message2(string exporterName) => 
        $"An exported variable is null for exporter {exporterName}";

    public static void AvoidNullExportedVariables(Node exportedValue, Node exporter) {
        if (exportedValue is null)
            GD.PushWarning(Message2(exporter.Name));
        
    }

    static string Message3(string actorName) => 
        $"The actor {actorName} either has Y Sorting disabled or Z as relative enabled, both of which are invald.";
    
    public static void AvoidImproperOrdering(CanvasItem item) {
        return;
        
        if (!item.YSortEnabled || item.ZAsRelative) {
            CanvasItem parent = item.GetParentOrNull<CanvasItem>();
            if (parent is not null) {
                GD.PushError($"For node whose parent is {parent.Name}: {Message3(item.Name)}");
            } else {
                GD.PushError(Message3(item.Name));
            }
        } 
    }
}