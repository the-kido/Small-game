using Godot;
using System.Collections.Generic;
using Game.Players;
using System;
using Game.LevelContent;

namespace Game.UI;

[Tool]
public partial class Camera : Camera2D {
	[Export]
	private TileMap tileMap;
	
	// These two functions allow outside classes to change how the camera behaves. 
	public Func<Vector2> PositionOverride;
	public Func<Vector2> ZoomOverride;

	public static Camera CurrentCamera {get; private set;}
	// public ShakePlayer ShakePlayer {get; init;} = new();
	
	private Camera() => CurrentCamera = this;
	
	//0.9 -- 1.4
	private const float SCALE_MAX = 1.9f, SCALE_MIN = 1.5f;
	private float diagonalLength;

	Player player;


	// Allows the camera to immediately move to its correct position instead of it weirdly panning from the 
	// original position of the camera in the scene and settling. 	
	static Camera() {
		Level.LevelStarted += () => CurrentCamera.CallDeferred("FixCamera");
	}

	public void FixCamera(){
        Zoom = FinalCameraZoom(ImportantObjectsRect());
		Position = FinalCameraPosition(ImportantObjectsRect().PointAverage());
	}

	public void Init(Player player) {
		KidoUtils.ErrorUtils.AvoidNullExportedVariables(tileMap, this);
		this.player = player;
		
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

		// Set diagonal lenght equal to the diagonal size of the USUAL scene.
		diagonalLength = Mathf.Sqrt(Mathf.Pow(sides.X, 2) + Mathf.Pow(sides.Y, 2));
		diagonalLength /= (SCALE_MAX + SCALE_MIN) * 2;
		
		importantObjects.Add(player);
	}

    public override string[] _GetConfigurationWarnings() {
		if (tileMap is null) return new string[] {"Tilemap is null!"};
        return null;
    }

    public override void _Process(double delta) {
		if (player is null) return;

		Rect2 importantObjectsRect = ImportantObjectsRect();
		Vector2 finalPosition = FinalCameraPosition(importantObjectsRect.PointAverage());
		Vector2 finalZoom = FinalCameraZoom(importantObjectsRect);

        Zoom = Zoom.Lerp(finalZoom, (float) delta);
		Position = Position.Lerp(finalPosition, (float) delta * 2);
		
		UpdateShake(delta);
		// Offset = ShakePlayer.GetShakeOffset(delta);
	}

	private readonly List<Node2D> importantObjects = new();

	public Vector2 FinalCameraZoom(Rect2 importantObjectsRect) {
		if (ZoomOverride is not null) return ZoomOverride();

		Vector2 bottomLeft = importantObjectsRect.Position;
		Vector2 topRight = importantObjectsRect.Size;
		Vector2 diff = topRight - bottomLeft;

		float diagonal = Mathf.Sqrt(diff.X*diff.X + diff.Y*diff.Y);

		//				the scale factor 				The default zoom.
		float dif = (SCALE_MAX + SCALE_MIN) / 2 / (diagonal / diagonalLength);
		
		//If the dif is greater than the max, set it to the lower number.
		dif = Mathf.Min(dif, SCALE_MAX);
        
		//if the dif is lower than the min, set it to the min.
		dif = Mathf.Max(dif, SCALE_MIN);
		
		return Vector2.One * dif;  
	}
	
	public Vector2 ConstantCameraOffset => player.GlobalPosition - GetLocalMousePosition();

	public Vector2 FinalCameraPosition(Vector2 importantObjectsRectCenter) {
		if (PositionOverride is not null) return PositionOverride();
		
		Vector2 FinalCameraPosition = Vector2.Zero;

		FinalCameraPosition += importantObjectsRectCenter * 5;

		FinalCameraPosition += PlayerCameraShift() * 2;

		return FinalCameraPosition / 7f;
	}

	private Vector2 PlayerCameraShift() {
		
		Vector2 cameraShift = player.Velocity.Normalized() * 100;
		cameraShift.Y *= 1.5f;

		return player.GlobalPosition + cameraShift;
	}
	
	private Rect2 ImportantObjectsRect() { 

		Vector2 pos = importantObjects[0].GlobalPosition;
		Vector2 size = importantObjects[0].GlobalPosition;

		foreach (Node2D body in importantObjects) {
			pos.X = Mathf.Min(body.GlobalPosition.X, pos.X);
			pos.Y = Mathf.Max(body.GlobalPosition.Y, pos.Y);
			
			size.X = Mathf.Max(body.GlobalPosition.X, size.X);
			size.Y = Mathf.Min(body.GlobalPosition.Y, size.Y);
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
		if (currentShakeTime <= 0) return;

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
		currentShakeStrength = shakeStrength;
		maxShakeTime = shakeTime;
		currentShakeTime = shakeTime;
		this.shakeSpeed = shakeSpeed;
	}

	#region signal methods
	private void OnBodyEntered(Node2D body) =>
		importantObjects.Add(body);
	
	private void OnBodyLeave(Node2D body) =>
		importantObjects.Remove(body);
	
	#endregion
}
