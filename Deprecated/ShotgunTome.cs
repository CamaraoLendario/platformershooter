using Godot;
using GodotPlugins.Game;
using System;

public partial class ShotgunTome : Weapon
{
	[Export] float coneAngle = 30f;
	[Export] int bulletCount = 5;
	public override void OnShoot(Vector2 inputDir)
	{
		if (!shootCooldownTimer.IsStopped()) return;
		base.OnShoot(inputDir);
		shootCooldownTimer.Start(shootCooldownTime);
		float coneAngleRad = Mathf.DegToRad(coneAngle);
		for (int bulletNum = 1; bulletNum <= bulletCount; bulletNum++)
		{
			LinearProjectile newBullet = GetNewBullet(owner.colorIdx, inputDir);
			float rot = inputDir.Angle();

			rot -= coneAngleRad / 2;
			rot += (bulletNum - 1) * coneAngleRad / (bulletCount - 1);

			newBullet.SetDirection(Vector2.FromAngle(rot));
			newBullet.direction.Y -= 0.5f;
			newBullet.direction = newBullet.direction.Normalized(); 
			newBullet.owner = owner;

			world.CallDeferred(MethodName.AddChild, newBullet);
		}
		
		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();
	}
}