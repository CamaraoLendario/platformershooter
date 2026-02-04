using Godot;
using GodotPlugins.Game;
using System;

public partial class GrapplingHook : Node2D
{
	[Export] RayCast2D hookRay;
	[Export] PilotAttack pilot;
	[Export] float hookAcceleration = 5000f;
	public bool isActive = false;
	Vector2 hookPosition = Vector2.Zero;
	public void Shoot(Vector2 inputVec)
	{
		hookRay.TargetPosition = inputVec * 100000;

		hookRay.ForceUpdateTransform();
		hookRay.ForceRaycastUpdate();

		if (!hookRay.IsColliding()) { GD.Print("Didn't find hookable surface :("); return; }

		isActive = true;
		hookPosition = hookRay.GetCollisionPoint();
	}

	public void UnShoot()
	{
		isActive = false;
	}

	public void ProcessPhysics(float delta)
	{
		if (!isActive) return;
		Vector2 hookVector = hookPosition - GlobalPosition;
		if(pilot.Velocity.AngleTo(hookVector) > Mathf.Pi/2 || pilot.Velocity.AngleTo(hookVector) < -Mathf.Pi/2 )
			pilot.Velocity -= hookVector.Normalized() * pilot.Velocity.Length() * Mathf.Cos(pilot.Velocity.AngleTo(hookVector));
	}
	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		base._Draw();
		if (!isActive) return;	
		DrawLine(Vector2.Zero, (hookPosition - GlobalPosition).Rotated(-(GetParent<Node2D>().Rotation)), new Color(1, 1, 1, 1), 1, true);
	}

}