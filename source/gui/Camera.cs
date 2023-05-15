using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Camera : Camera2D
{
	[Export]
	private Player player;
	[Export]
	private TileMap tileMap;

	public static Camera currentCamera;
	
	private Camera() {
		currentCamera = this;
	}
	
	//0.9 -- 1.4
	private const float SCALE_MAX = 1.9f, SCALE_MIN = 1.5f;
	private float diagonalLength;

	public override void _Ready() {
		
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
		

		//Set diagonal lenght equal to the diagonal size of the USUAL Screne.

		diagonalLength = Mathf.Sqrt(Mathf.Pow(sides.X, 2) + Mathf.Pow(sides.Y, 2));
		diagonalLength /= (SCALE_MAX + SCALE_MIN) * 2;

		importantObjects.Add(player);

	}
	public override void _Process(double delta) {

		Rect2 importantObjectsRect = ImportantObjectsRect();

		Vector2 finalPosition = FinalCameraPosition(importantObjectsRect.PointAverage());
		
		Vector2 finalZoom = FinalCameraZoom(importantObjectsRect);

        Zoom = Zoom.Lerp(finalZoom, (float) delta);

		Position = Position.Lerp(finalPosition, (float) delta * 2) ;

		UpdateShake(delta);

	}

	private List<Node2D> importantObjects = new();


	public Vector2 FinalCameraZoom(Rect2 importantObjectsRect) {
		Vector2 bottomLeft = importantObjectsRect.Position;
		Vector2 topRight = importantObjectsRect.Size;
		Vector2 diff = topRight - bottomLeft;

		float diagonal = Mathf.Sqrt(diff.X*diff.X + diff.Y*diff.Y);


		//				the scale factor 				The default zoom.
		float dif = ((SCALE_MAX + SCALE_MIN) / 2) / (diagonal / diagonalLength);
		//If the dif is greater than the max, set it to the lower number.

		dif = Mathf.Min(dif, SCALE_MAX);
        
		//if the dif is lower than the min, set it to the min.
		dif = Mathf.Max(dif, SCALE_MIN);
		
		return Vector2.One * (dif); //TODsO. 
	}
	

	public Vector2 ConstantCameraOffset() {
		Vector2 finalCameraOffset = Vector2.Zero;
		

		finalCameraOffset += player.Position - GetLocalMousePosition();

		return finalCameraOffset;
	}

	public Vector2 FinalCameraPosition(Vector2 importantObjectsRectCenter) {
		
		Vector2 FinalCameraPosition = Vector2.Zero;

		FinalCameraPosition += importantObjectsRectCenter * 5;

		FinalCameraPosition += PlayerCameraShift() * 2;

		return FinalCameraPosition / 7f;
	}

	
	private Vector2 PlayerCameraShift() {
		Vector2 cameraShift = player.Velocity.Normalized() * 100;
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

	float noiseIndex = 0;
	FastNoiseLite noise = new();
	float currentShakeStrength = 0;
	float shakeSpeed;
	double maxShakeTime;
	double currentShakeTime;
	private void UpdateShake(double delta) {
		if (currentShakeTime <= 0)
			return;

		currentShakeTime -= delta;
		currentShakeStrength *= (float) (currentShakeTime / maxShakeTime); 

		//tween.InterpolateValue (shakeStrength, 0, delta, 10, Tween.TransitionType.Linear, Tween.EaseType.In);
		RandomNumberGenerator rand = new();
		rand.Randomize();

		noiseIndex += (float) delta * shakeSpeed;

		Offset = new(
			noise.GetNoise2D(1, noiseIndex) * currentShakeStrength,
			noise.GetNoise2D(100, noiseIndex) * currentShakeStrength
		);
	}
	
	///<summary>
	///shakeSpeed: 0 - frozen. 300 - decently fast. 
	///</summary>
	public void StartShake(float shakeStrength, int shakeSpeed, double shakeTime) {
		this.currentShakeStrength = shakeStrength;
		this.maxShakeTime = shakeTime;
		this.currentShakeTime = shakeTime;
		this.shakeSpeed = shakeSpeed;
	}

	#region signal methods
	private void OnBodyEntered(Node2D body) {
		importantObjects.Add(body);
	}
	private void OnBodyLeave(Node2D body) {
		importantObjects.Remove(body);
	}
	#endregion
}