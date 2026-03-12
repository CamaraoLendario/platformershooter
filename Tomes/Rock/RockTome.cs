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
			
			AtlasTexture newTexture = (rockPellet.sprite as Sprite2D).Texture.Duplicate() as AtlasTexture;

			Vector2 pelletSpriteRegionPosition = new(GD.RandRange(0, 2), GD.RandRange(0, 1));

			newTexture.Region = new Rect2(pelletSpriteRegionPosition * 16, Vector2.One * 16);
			(rockPellet.sprite as Sprite2D).Texture = newTexture;
			
			pellets.Add(rockPellet);
			world.CallDeferred(MethodName.AddChild, rockPellet);
		}
		foreach(RockPellet pellet in pellets)
		{
			pellet.sisterPellets = pellets;
		}

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();

		Rect2 newRegion = new Rect2()
		{
			Size = Vector2.One * 16,
			Position = Vector2.Right * (3 - currentAmmo) * 16
		};

		((sprite as Sprite2D).Texture as AtlasTexture).Region = newRegion;
		return true;
    }
}
