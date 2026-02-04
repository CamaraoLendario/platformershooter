using Godot;
using System;

public partial class RockTome : Weapon
{
	[Export] int bulletCount = 5;
	[Export] float coneAngle = 15;
    public override void OnShoot(Vector2 inputDir)
    {
        if (!shootCooldownTimer.IsStopped()) return;
		base.OnShoot(inputDir);
		
		shootCooldownTimer.Start(shootCooldownTime);

		float coneAngleRad = Mathf.DegToRad(coneAngle);
		for (int bulletNum = 1; bulletNum <= bulletCount; bulletNum++)
		{
			LinearProjectile rockPellet = GetNewBullet(owner.colorIdx, inputDir);
			float rot = inputDir.Angle();

			rot -= coneAngleRad / 2;
			rot += (bulletNum - 1) * coneAngleRad / (bulletCount - 1);

			rockPellet.SetDirection(Vector2.FromAngle(rot));
			rockPellet.owner = owner;
			
			AtlasTexture newTexture = (rockPellet.sprite as Sprite2D).Texture.Duplicate() as AtlasTexture;
			int bulletIdx = bulletNum - 1;
 
			/*/*Vector2 pelletSpriteRegionPosition = new Vector2(
				bulletIdx - (3 * (bulletIdx / 3)),
				bulletIdx / 3
			); */
			Vector2 pelletSpriteRegionPosition = new(GD.RandRange(0, 2), GD.RandRange(0, 1));

			newTexture.Region = new Rect2(pelletSpriteRegionPosition * 16, Vector2.One * 16);
			(rockPellet.sprite as Sprite2D).Texture = newTexture;
			
			world.CallDeferred(MethodName.AddChild, rockPellet);
		}

		currentAmmo--;
		if (currentAmmo <= 0) holder.DropWeapon();
    }
}
