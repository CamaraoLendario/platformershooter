using Godot;
using System.Collections.Generic;
using System;

public partial class RockTome : Weapon
{
	[Export] int bulletCount = 5;
	[Export] float coneAngle = 15;
    public override bool OnShoot(Vector2 inputDir)
    {
        if (!base.OnShoot(inputDir))
		{
			return false;
		}

		float coneAngleRad = Mathf.DegToRad(coneAngle);
		List<RockPellet> pellets = new();
		for (int bulletNum = 1; bulletNum <= bulletCount; bulletNum++)
		{
			RockPellet rockPellet = GetNewBullet(owner.colorIdx, inputDir) as RockPellet;
			float rot = inputDir.Angle();

			rot -= coneAngleRad / 2;
			rot += (bulletNum - 1) * coneAngleRad / (bulletCount - 1);

			rockPellet.SetDirection(Vector2.FromAngle(rot));
			rockPellet.owner = owner;

			rockPellet.sprite.Frame = GD.RandRange(0, 5);
			pellets.Add(rockPellet);
			world.CallDeferred(MethodName.AddChild, rockPellet);
		}
		foreach(RockPellet pellet in pellets)
		{
			pellet.sisterPellets = pellets;
		}

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();

		sprite.Frame = 3 - currentAmmo;// TODO changed this from using atlas texture in a sprite 2d to just an animated sprite 2d due to changing standard in main scene of weapons. untested, test
		
		return true;
    }
}
