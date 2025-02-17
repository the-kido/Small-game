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
        if (exportedValue is null) GD.PushWarning(Message2(exporter.Name));
    }

    static string Message3(string exportedValueName, string exporter) => 
        $"{exporter}'s {exportedValueName} (exported value) is invalid!";
    public static void AvoidInvalidExportedVariables(bool isInvalid, Node exporter, string exportedValueName) {
        if (isInvalid is true) GD.PushWarning(Message2(exporter.Name));
    }

    public static void AvoidIncorrectVisibility(Control node, bool whatItShouldBe) {
        GD.PushWarning($"The control{node.Name}'s visibility should be {whatItShouldBe} but is {!whatItShouldBe}!");
    }
}