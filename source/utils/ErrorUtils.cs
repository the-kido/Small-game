using Godot;
namespace KidoUtils;
using System;

public static class ErrorUtils {
    public static void AvoidEmptyCollisionLayers(CollisionObject2D collisionObject) {
        if (collisionObject.CollisionLayer == 0 || collisionObject.CollisionMask == 0) {
            string message = "The CollisionObject2D " + collisionObject.Name + " does not have any collision layers/masks selected";
            
            Node parent = collisionObject.GetParent();
            if (parent is not null)
                message = $"For child node of {parent.Name}: {message}";

            GD.PushError(message);
        }
    } 
}