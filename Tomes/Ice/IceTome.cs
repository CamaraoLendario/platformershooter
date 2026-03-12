using Godot;
using System;

public partial class IceTome : Weapon
{
	public override bool OnShoot(Vector2 inputDir)
	{
		if (!base.OnShoot(inputDir))
		{
			return false;
		}
		IceRay newBullet = bullet.Instantiate<IceRay>();
		newBullet.owner = owner;
		newBullet.inputDir = inputDir;
		newBullet.Position = holder.GlobalPosition;
		world.AddChild(newBullet);

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();
		return true;
	}
}
