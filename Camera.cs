using Godot;
using System;
using System.Collections.Generic;

public partial class Camera : Camera2D
{
	[Export]
	private Player player;
	[Export]
	private TileMap tileMap;
	
	//0.9 -- 1.4
	private const float SCALE_MAX = 1.4f, SCALE_MIN = 0.9f;
	private float diagonalLength;


	private MovementController movementController;

	public override void _Ready() {
		movementController = player.GetNode<MovementController>("Movement Controller");
		
		#region initialize the size of the camera for this level
		Vector2I ts = tileMap.TileSet.TileSize;
		Rect2 rect = tileMap.GetUsedRect();

		LimitLeft = (int) (rect.Position.X * ts.X * tileMap.Scale.X);
		LimitTop = (int) (rect.Position.Y * ts.Y * tileMap.Scale.Y);
		LimitRight = (int) (rect.End.X * ts.X * tileMap.Scale.X);
		LimitBottom = (int) (rect.End.Y * ts.Y * tileMap.Scale.Y);
		#endregion

		Rect2 cameraRect = GetViewportRect();
		Vector2 sides = cameraRect.Size - cameraRect.Position;
		
		diagonalLength = Mathf.Sqrt(Mathf.Pow(sides.X, 2) + Mathf.Pow(sides.Y, 2));


		importantObjects.Add(player);


	}
	public override void _Process(double delta) {

		Rect2 importantObjectsRect = ImportantObjectsRect();

		Vector2 finalPosition = FinalCameraPosition(importantObjectsRect.PointAverage());
		
		Vector2 finalZoom = FinalCameraZoom(importantObjectsRect);

        Zoom = Zoom.Lerp(finalZoom, (float) delta);

        //Zoom = finalZoom;

		Position = Position.Lerp(finalPosition, (float) delta * 2);

	}

	private List<Node2D> importantObjects = new();


	public Vector2 FinalCameraZoom(Rect2 importantObjectsRect) {
		Vector2 bottomLeft = importantObjectsRect.Position;
		Vector2 topRight = importantObjectsRect.Size;
		Vector2 diff = topRight - bottomLeft;


		//x: 823
		//y: 464
		float reg = Mathf.Sqrt(823^2 + 464^2); //The diagonal's length at zoom 1.4. Can't be more than this.


		float diagonal = Mathf.Sqrt(diff.X*diff.X + diff.Y*diff.Y);
		GD.Print(diagonal, " annd " , diagonalLength);
		float dif = diagonal / diagonalLength;
		
        dif *= 2f;

		GD.Print("Dif:", dif);
        dif = Mathf.Max(dif, 0.8f);
		
		return Vector2.One / (dif); //TODO. 

		

	}
	
	public Vector2 FinalCameraPosition(Vector2 importantObjectsRectCenter) {
		
		Vector2 FinalCameraPosition = Vector2.Zero;


		FinalCameraPosition += importantObjectsRectCenter / 2;

		FinalCameraPosition += PlayerCameraShift() / 2;

		return FinalCameraPosition;
	}


	private Vector2 PlayerCameraShift() {
		Vector2 cameraShift = movementController.playerDirection * 100;
		cameraShift.Y *= 1.5f;

		return player.Position + cameraShift;
	}
	
	private Rect2 ImportantObjectsRect() { 
		Vector2 pos = importantObjects[0].Position;
		Vector2 size = importantObjects[0].Position;

		foreach(Node2D body in importantObjects) {
			pos.X = Mathf.Min(body.Position.X, pos.X);
			pos.Y = Mathf.Max(body.Position.Y, pos.Y);
			
			size.X = Mathf.Max(body.Position.X, size.X);
			size.Y = Mathf.Min(body.Position.Y, size.Y);
		}
		return new(pos,size);

	}



	private void OnBodyEntered(Node2D body) {
		importantObjects.Add(body);
		
		GD.Print("ENTERED SOMETHING DID, ", body.Name);
	}
	private void OnBodyLeave(Node2D body) {
		importantObjects.Remove(body);

		GD.Print("LEFT SOMETHING DID, ", body.Name);
	}
	
}
