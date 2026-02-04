using Godot;
using System;

public partial class WindTome : Weapon
{
	public override void OnShoot(Vector2 inputDir)
	{
		base.OnShoot(inputDir);
		LinearProjectile newBullet = GetNewBullet(owner.colorIdx, inputDir);

		world.CallDeferred(MethodName.AddChild, newBullet);

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();
	}
}
