using Godot;
using System;

public partial class PoisonTome : Weapon
{
	[Export] float projectileInitialVelocity = 450;
	
	public override bool OnShoot(Vector2 inputDir)
	{
		if (!base.OnShoot(inputDir))
		{
			return false;
		}
		PoisonMinePlacerBody newBullet = bullet.Instantiate<PoisonMinePlacerBody>();

		newBullet.direction = inputDir.Normalized();
		newBullet.direction.Y -= 0.5f;
		newBullet.speed = projectileInitialVelocity;
		newBullet.Position = GlobalPosition;
		newBullet.direction = newBullet.direction.Normalized();
		newBullet.owner = owner;

		world.CallDeferred(MethodName.AddChild, newBullet);

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();
		return true;
	}
}
