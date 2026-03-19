using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;

public partial class ShipController : Controller
{
	[Signal] public delegate void dashedEventHandler();
	[Export] AudioStreamPlayer2D DashAudio;
	const float Acceleration = 500f;
	float acceleration = Acceleration;
	public const float MAXSPEED = 200f;
	float maxSpeed = MAXSPEED;
	public const float DASHSPEEDMULTIPLIER = 3f;
	Timer dashCooldownTimer = new Timer();
	float dashCooldownTime = 0.5f;
	Timer dashTimer = new Timer();
	float dashTime = 0.15f;
	bool isDashing = false;
	public ShipPowerUpSpeedUp speedUp = null;
	protected Node2D world;

	public override void _Ready()
	{
		base._Ready();
        if (Game.Instance.world !=  null)
            world = Game.Instance.world;
        else
        {
            world = Main.GetParent<Node2D>();
        }
		isPilotController = false;
		SetupTimers(new List<Timer> { dashTimer, dashCooldownTimer });
		
		dashTimer.Timeout += dashEnd;
	}

	public override void ProcessWASD(float X, float Y)
	{
		if (Main.effectHandler.isFrozen) return;
		Vector2 newInputVec = new Vector2(X, Y);
		if (newInputVec.LengthSquared() < controllerDeadzone * controllerDeadzone) newInputVec *= 0;
		inputVector = newInputVec.Normalized();
    }

	public override void ProcessPhysics(double delta)
	{
		if (!IsAllowed()) return;
		Movement((float)delta);
		Main.MoveAndSlide();
	}

	void Movement(float delta)
	{
		float speedBuffs = 1;
		if (speedUp != null) speedBuffs *= 1.5f;
		if(!Main.isAiming) Velocity += inputVector * acceleration * 2 * speedBuffs * delta;

		float tempMaxSpeed = maxSpeed;
		float dragMag = acceleration * 0.1f;
		if (Main.GetSlideCollisionCount() != 0)
        {
            dragMag *= 1.2f;
			if (!isDashing) tempMaxSpeed *= 0.8f; 
        }
		tempMaxSpeed *= speedBuffs;
		float velBasedDragMag = Velocity.Length() / 2;

		if (velBasedDragMag > dragMag)
			dragMag = velBasedDragMag;


		Velocity -= Velocity.Normalized() * dragMag * speedBuffs * delta;

		if (Velocity.LengthSquared() > tempMaxSpeed * tempMaxSpeed)
			Velocity = Velocity.Normalized() * tempMaxSpeed;
		if (Velocity.LengthSquared() < 0.1)
			Velocity *= 0;

		if (!dashTimer.IsStopped() && dashCooldownTimer.IsStopped())
		{
			processDash((float)delta);
		}
	}

	public override void OnJumpStart()
	{
		if(!IsAllowed() || !dashCooldownTimer.IsStopped() || Main.effectHandler.isFrozen) return;
		if (Main.effectHandler.isFrozen) return;
		DashAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
		DashAudio.Play();
		maxSpeed *= DASHSPEEDMULTIPLIER;
		acceleration *= 10f;
		isDashing = true;
		Vector2 dashInput = inputVector.Normalized();
		
		if (inputVector == Vector2.Zero)
			dashInput = Main.Velocity.Normalized();
		if (inputVector == Vector2.Zero)
			dashInput = Vector2.FromAngle(sprite.Rotation).Normalized();

		Velocity = dashInput * maxSpeed;	
		if (dashInput.Y >= 0)
			sprite.Rotation = MathF.Acos(dashInput.X);
		else
			sprite.Rotation = -MathF.Acos(dashInput.X) + MathF.Tau;

		dashTimer.Start(dashTime);
		EmitSignal(SignalName.dashed);
		dashCooldownTimer.Start(dashCooldownTime);
	}

	void processDash(float delta)
	{
		maxSpeed -= 2 * MAXSPEED * (delta / dashTime);
	}

	void dashEnd()
	{
		maxSpeed = MAXSPEED;
		acceleration = Acceleration;
		isDashing = false;
	}
}