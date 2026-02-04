using Godot;
using System;

public partial class ShipPowerUpDualGuns : ShipGunPowerUp
{
	int shotCount = 3;

    protected override void Shoot()
	{
		Rotation = controller.sprite.Rotation;
		
		foreach(Marker2D marker in GetChildren())
		{
			LinearProjectile newBullet = controller.GetNewBullet();
			newBullet.Position = marker.GlobalPosition;
			Game.Instance.world.AddChild(newBullet);
		}

		shotCount --;
		if (shotCount <= 0)
		{
			End();
		}
	}
}
