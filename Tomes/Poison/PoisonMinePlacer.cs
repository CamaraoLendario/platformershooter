using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;

public partial class PoisonMinePlacer : LinearProjectile
{
	[Export] PackedScene PoisonMineScene;
	Node2D wallDetectorsNode;
	const float GRAVITYFORCE = 1500;

	public override void _Ready()
	{
		base._Ready();
		wallDetectorsNode = GetNode<Node2D>("wallDetectorsNode");
	}

	protected override void OnBodyHit(Node2D body)
	{
		if (body is not TileMapLayer tileMapLayer) return;
		
		CollisionConfirm();
		SummonMine();
		
		QueueFree();
	}

	void SummonMine()
	{
		PoisonMine newPoisonMine = PoisonMineScene.Instantiate<PoisonMine>();
		Vector2 wallSide = GetWallSide();
		GD.Print(wallSide);
		newPoisonMine.wallSide = wallSide;
		newPoisonMine.Position = Position;

		Game.Instance.world.AddChild(newPoisonMine);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		ProcessGravity((float) delta);
		sprite.Rotation = velocity.Angle();
	}

	void ProcessGravity(float delta)
	{
		velocity += new Vector2(0, 1) * GRAVITYFORCE * delta;
	}

	Vector2I GetWallSide()
	{
		List<bool> bools = [false, false, false, false];
		int colidingCount = 0;

		foreach(RayCast2D ray in wallDetectorsNode.GetChildren())
		{
			ray.ForceRaycastUpdate();
			if (ray.IsColliding())
			{
				bools[ray.GetIndex()] = true;
				colidingCount++;
			}
		}

		switch (colidingCount)
		{
			case 0:
			return new Vector2I(0, 1);

			case 1:
			for (int i = 0; i < bools.Count - 1; i++)
				{
					if (bools[i])
					{
						return GetRayDir(GetRayFromIdx(i));
					}
				}
			break;

			case 2:
			float topDistance = 999;
			Vector2I currentWallDirection = new Vector2I(0, 1);
			for (int i = 0; i < bools.Count - 1; i++)
				{
					if (bools[i])
					{
						RayCast2D currentRay = GetRayFromIdx(i);
						float DistanceToCenter = (currentRay.GetCollisionPoint() - GlobalPosition).Length();
						if (DistanceToCenter >= topDistance) continue; 
						topDistance = DistanceToCenter;

						currentWallDirection = GetRayDir(currentRay);
					}
				}
			
			return currentWallDirection;

			case 3:
			for (int i = 0; i < bools.Count - 1; i++)
				{
					if (!bools[i])
					{
						if (i > 1)
						{
							return GetRayDir(GetRayFromIdx(i-2));
						}
						else
						{
							return GetRayDir(GetRayFromIdx(i+2));
						}
					}
				}
			break;
		}

		return new Vector2I(0, 1);
	}

	Vector2I GetRayDir(RayCast2D ray)
	{
		Vector2 targetNormalized = ray.TargetPosition.Normalized();
		return new Vector2I ((int)(targetNormalized.X / Math.Abs(targetNormalized.X)), (int)(targetNormalized.Y / Math.Abs(targetNormalized.Y)));
	}

	RayCast2D GetRayFromIdx(int idx)
	{
		return wallDetectorsNode.GetChild<RayCast2D>(idx);
	}
}
