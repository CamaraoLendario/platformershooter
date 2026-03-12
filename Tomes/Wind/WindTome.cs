using Godot;
using System;

public partial class WindTome : Weapon
{
	public override bool OnShoot(Vector2 inputDir)
	{
		if (!base.OnShoot(inputDir))
		{
			return false;
		}
		LinearProjectile newBullet = GetNewBullet(owner.colorIdx, inputDir);

		world.CallDeferred(MethodName.AddChild, newBullet);

		currentAmmo--;
		if (currentAmmo <= 0)
		{
			holder.DropWeapon();
		}
		
		return true;
	}
}
