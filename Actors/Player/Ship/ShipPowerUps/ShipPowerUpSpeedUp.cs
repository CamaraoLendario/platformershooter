using Godot;
using System;

public partial class ShipPowerUpSpeedUp : ShipPowerUp
{
    Timer lifeTimer = new();
	const float LIFETIME = 7.5f;

	public override void _Ready()
    {
        base._Ready();
		controller.speedUp = this;
		lifeTimer.OneShot = true;
		lifeTimer.Timeout += QueueFree;
		AddChild(lifeTimer);
		lifeTimer.Start(LIFETIME);
	}

    public override void _ExitTree()
    {
		controller.speedUp = null;
        base._ExitTree();
    }
}
