using Godot;
using System;

public partial class IceTome : Weapon
{
	public override void OnShoot(Vector2 inputDir)
	{
		base.OnShoot(inputDir);
		IceRay newBullet = bullet.Instantiate<IceRay>();
		newBullet.owner = owner;
		newBullet.inputDir = inputDir;
		newBullet.Position = holder.GlobalPosition;
		world.AddChild(newBullet);

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();
	}
}
