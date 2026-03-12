using Godot;
using System;
using System.Collections.Generic;

public partial class ShipPowerUpChestSpawner : Node2D
{
	[Export] float spawnCooldowntime = 20f;
	PackedScene chestScene = GD.Load<PackedScene>("uid://d2381diivxfax");
	Timer spawnCooldownTimer = new();


    public override void _Ready()
    {
        spawnCooldownTimer.OneShot = true;
		AddChild(spawnCooldownTimer);
		spawnCooldownTimer.CallDeferred(Timer.MethodName.Start, spawnCooldowntime);
		spawnCooldownTimer.Timeout += SpawnNewChest;
		CallDeferred(MethodName.SpawnNewChest);

		foreach(var child in GetChildren())
		{
			if (child is not ShipPowerUpChest chest) continue;
			chest.Opened += () =>
			{
				if(spawnCooldownTimer.IsStopped()) 
					spawnCooldownTimer.Start(spawnCooldowntime);
			};
		}
    }

	void SpawnNewChest()
	{
		GD.Print("spawning a new chest");
		List<ShipPowerUpChest> brokenChests = [];
		foreach(var child in GetChildren())
		{
			if (child is not ShipPowerUpChest chest) continue;
			if (!chest.Visible)
				brokenChests.Add(chest);
		}
		
		if (brokenChests.Count == 0)
		{
			GD.Print("no broken chests found, returning");
			return;
		}

		brokenChests[GD.RandRange(0, brokenChests.Count - 1)].Spawn();
		GD.Print("a new chest has been spawned");
		

		if (brokenChests.Count >= 2 && spawnCooldownTimer.IsStopped())
		{
			spawnCooldownTimer.Start(spawnCooldowntime);
		} 
	}
}
