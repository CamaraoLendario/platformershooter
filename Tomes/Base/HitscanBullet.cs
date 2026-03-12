using Godot;
using System;
using System.IO;

public partial class HitscanBullet : RayCast2D
{
	[Export] protected float lifeTime = 0.5f; 
	[ExportGroup("Debug")]
	[Export] protected bool printDebug = false;
	public Player owner;
	public Vector2 inputDir;
	float maxLength = 10000f;
	Timer lifeTimer = new();


	public override void _Ready()
	{
		TargetPosition = new Vector2(1, 0) * maxLength;
		Rotation = inputDir.Angle();
		lifeTimer.OneShot = true;
		lifeTimer.Timeout += OnLifeEnd;
		AddChild(lifeTimer);
		lifeTimer.Start(lifeTime);
		ForceRaycastUpdate();
	}

    private void OnLifeEnd()
    {
		End();
    }
    protected virtual void Hit(Player player)
	{
		player.TakeDamage(owner);
	}
	protected virtual void Hit(StaticBody2D body)
	{
		GD.Print(body);
	}
	public void End()
    {
		QueueFree();
    }
}
