using Godot;
using System;

public partial class ShipSprite : PlayerSprite
{
	[Export] protected ShipController controller;
	public override void _Process(double delta)
	{
		ProcessRotation((float)delta);
	}
	
	void ProcessRotation(float delta)
	{
		Vector2 normalizedInputDir = controller.inputVector.Normalized();
		if (normalizedInputDir.X == 0 && normalizedInputDir.Y == 0) return;
		if (Rotation < 0) Rotation += MathF.Tau;

		float targetRotation;
		if (normalizedInputDir.Y >= 0)
			targetRotation = MathF.Acos(normalizedInputDir.X);
		else
			targetRotation = -MathF.Acos(normalizedInputDir.X) + MathF.Tau;

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
