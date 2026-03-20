using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Resolvers;

// Jokerz_Trix: if(shrimp) shramp;
public partial class MagicMissileComponent : Node
{
	[Export] PackedScene magicMissile = GD.Load<PackedScene>("uid://d1rvfwvuso0x4");
	[Export] Map mapNode;
	float spawnTime = 1f;
	float extraDistance = 48;
	Timer SpawnTimer = new()
	{
		OneShot = true,
	};

	public override void _Ready()
	{
		if (!Game.Instance.experimentalFeatures.isActivated)
		{
			QueueFree();
			return;
		}
		spawnTime = Game.Instance.experimentalFeatures.interval;
		AddChild(SpawnTimer);
		SpawnTimer.Timeout += OnSpawnTimerTimeout;
		SpawnTimer.Start(spawnTime);
	}


	void OnSpawnTimerTimeout()
	{
		SpawnMagicMissile();
		SpawnTimer.Start(spawnTime);
	}

	void SpawnMagicMissile()
	{
		MagicMissile newMagicMissile = magicMissile.Instantiate<MagicMissile>();
		
		newMagicMissile.GlobalPosition = GetSpawnPoint(GetPlayerPosAverage());
		if (newMagicMissile.GlobalPosition == Vector2.Zero) return;

		AddSibling(newMagicMissile); 
	}

	Vector2 GetSpawnPoint(Vector2 centerPoint)
	{
		Vector2 spawnPoint;
		Vector2 pixelMapSize = mapNode.PixelsMapSize;
		float X = Mathf.Abs(centerPoint.X);
		float Y = Mathf.Abs(centerPoint.Y);
		
		float NormalizedY = Y * pixelMapSize.X/pixelMapSize.Y;
		if (NormalizedY > X)
		{
			float distance = Mathf.Abs(Y) + pixelMapSize.Y/2;
			spawnPoint = -Abs(centerPoint) + Abs(new Vector2(centerPoint.X/centerPoint.Y, centerPoint.Y/centerPoint.Y) * distance);
			spawnPoint = -centerPoint.Normalized() * spawnPoint.Length();
		}
		else
		{
			float distance = Mathf.Abs(X) + pixelMapSize.X/2;
			spawnPoint = -Abs(centerPoint) + Abs(new Vector2(centerPoint.X/centerPoint.X, centerPoint.Y/centerPoint.X) * distance);
			spawnPoint = -centerPoint.Normalized() * spawnPoint.Length();
		}

		return spawnPoint;
	}

	Vector2 GetPlayerPosAverage()
	{
		Vector2 averagePos = Vector2.Zero;
		int alivePlayerCount = 0;
		foreach (Player player in Game.Instance.playerNodesByColor.Values)
		{
			if (!player.IsDead)
			{
				averagePos += player.GlobalPosition;
				alivePlayerCount += 1;
			}
		}
	
		if (alivePlayerCount == 0)
		{
			GD.PrintErr("0 players alive, returning average as zero");
			return Vector2.Zero;
		}

		averagePos /= alivePlayerCount;	
		return averagePos;
	}

	static Vector2 Abs(Vector2 vector)
	{
		return new Vector2(Mathf.Abs(vector.X), Mathf.Abs(vector.Y));
	}
}
