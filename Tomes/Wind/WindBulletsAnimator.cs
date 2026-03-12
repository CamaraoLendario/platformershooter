using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;

public partial class WindBulletsAnimator : Node2D
{
	[Export] float animationSpeed = 1f;
	List<AnimatedSprite2D> ammo = []; 
	float time = 0f;
    public override void _Ready()
    {
		AnimatedSprite2D baseAmmoTemplate = null;
		foreach(Node node in GetChildren())
		{
			if (node is not AnimatedSprite2D sprite) continue;
			if (baseAmmoTemplate == null) baseAmmoTemplate = sprite.Duplicate() as AnimatedSprite2D; 
			sprite.QueueFree();
		}

		int maxAmmo = GetParent<WindTome>().maxAmmo;
		for(int i = 0; i < maxAmmo; i++)
		{
			AddChild(baseAmmoTemplate.Duplicate());
		}

		GetParent<Weapon>().Shot += () =>
		{
			ammo[0].QueueFree();
			ammo.Remove(ammo[0]);
			animationSpeed += 1;
		};
        foreach(Node node in GetChildren())
		{
			if (node is not AnimatedSprite2D sprite) return;
			if (sprite.IsQueuedForDeletion()) continue;
			sprite.Frame = GD.RandRange(0, sprite.SpriteFrames.GetFrameCount("default") - 1);

			ammo.Add(sprite);
		}
	}


    public override void _Process(double delta)
    {
		int idx = 1;
		int ammoCount = ammo.Count;
	    foreach(AnimatedSprite2D sprite in ammo)
		{
			Vector2 oldPosition = sprite.Position;
			float step = Mathf.Pi * 2 / ammoCount;
			float Angle = (time * Mathf.Pi * 2) + (step * idx);
			sprite.Position = new Vector2(
				Mathf.Cos(Angle),
			 	Mathf.Sin(Angle) * 0.69f
			) * 10;
			
			if (oldPosition.X > sprite.Position.X)
			{
				sprite.FlipH = true;
				sprite.ZIndex = 0;
			}
			else
			{
				sprite.FlipH = false;
				sprite.ZIndex = 2;
			}

			sprite.Rotation = Mathf.DegToRad(sprite.Position.X);

			idx ++;
		}
		if (animationSpeed > 5) animationSpeed = 5;
		if (animationSpeed >= 1) animationSpeed -= (float)delta;

		time -= (float)delta * animationSpeed;
    }

}