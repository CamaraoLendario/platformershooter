using Godot;
using System;

public partial class PilotSprite : PlayerSprite
{
	[Export] protected PilotAttack controller;

	bool isJumping = false;

	public override void _Ready()
	{
		Play();
		controller.Jumped += OnJump;
		AnimationFinished += onAnimationEnd;
	}

	public override void _Process(double delta)
	{
        if (!Main.IsInPilotArea)
		{
			ProcessRotation((float)delta);
			if (controller.inputVector != Vector2.Zero)
				Animation = "Running";
			else
				Animation = "Idle";
			return;
        }
		else Rotation = 0 + (controller.Velocity.X / PilotAttack.SOFTMAXHSPEED)* (Mathf.Pi / 16);

		if (controller.facing < 0) FlipH = true;
		else FlipH = false;

		if (Main.IsOnWall() && !Main.IsOnFloor())
		{
			isJumping = false;
			Animation = "OnWall";
			return;
		}

		if (isJumping && !Main.IsOnFloor()) return;
		else
        {
			isJumping = false;
			Play();
        }

		if (Main.IsOnFloor())
		{
			if (controller.Velocity.X != 0)
				Animation = "Running";
			else
				Animation = "Idle";
		}
		else
		{
			if (Main.Velocity.Y > 0)
				Animation = "FallDown";
			else Animation = "FallUp";
		}
	}

	void OnJump(int wallSide)
	{
		if (wallSide != 0) controller.facing = wallSide * -1;
		
		isJumping = true;
		Animation = "Jump";
		Play();
	}

	void onAnimationEnd()
	{
		if (Animation == "Jump")
		{
			isJumping = false;
		}
	}
	
	void ProcessRotation(float delta)
	{
		Vector2 normalizedInputDir = controller.GetProcessedInput().Normalized();
		if (normalizedInputDir.X == 0 && normalizedInputDir.Y == 0) return;
		if (Rotation < 0) Rotation += MathF.Tau;

		float targetRotation;
		if (normalizedInputDir.Y >= 0)
			targetRotation = MathF.Acos(normalizedInputDir.X);
		else
			targetRotation = -MathF.Acos(normalizedInputDir.X) + MathF.Tau;
		targetRotation += Mathf.Pi / 2;
		float rotationDifference = targetRotation - Rotation;

		if (rotationDifference > Math.PI)
			rotationDifference -= MathF.Tau;
		else if (rotationDifference < -Math.PI)
			rotationDifference += MathF.Tau;

		int direction;
		if (rotationDifference >= 0)
			direction = 1;
		else
			direction = -1;

		float step = MathF.Tau * (delta / turnSpeed) * direction;

		if (step * step > rotationDifference * rotationDifference)
		{
			Rotation = targetRotation;
		}
		else
		{
			Rotation += step;
		}

		//Keeps rotation between 0 and PI*2
		Rotation = (Rotation + MathF.Tau) % MathF.Tau;
	
	}
}