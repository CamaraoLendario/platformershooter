using Godot;
using GodotPlugins.Game;
using System;

public partial class FireTome : Weapon
{
	[Export] GpuParticles2D particles;
	public override bool OnShoot(Vector2 inputDir)
	{
		if (!base.OnShoot(inputDir))
		{
			return false;
		}
		LinearProjectile newBullet = GetNewBullet(owner.colorIdx, inputDir);

		world.CallDeferred(MethodName.AddChild, newBullet);

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();
		return true;
	}

    public override void _Process(double delta)
    {
		if (holder.Rotation > MathF.PI/2 || holder.Rotation < -MathF.PI/2)
        {
            (particles.ProcessMaterial as ParticleProcessMaterial).Direction = new Vector3(0, 1, 0);
        }
		else (particles.ProcessMaterial as ParticleProcessMaterial).Direction = new Vector3(0, -1, 0);
	}
}
