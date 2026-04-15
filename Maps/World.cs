using Godot;
using System;
using System.Collections.Specialized;
using System.Data;

public partial class World : Node2D
{
	[Export] public ScoreHUD Hud;
	[Export] 
	public Map currentMap = null;
	public override void _Ready()
	{
		generateMap();
	}

	void generateMap()
	{
		if (currentMap == null) 
			currentMap = Game.Instance.currentMap;
			
		if (!currentMap.IsInsideTree())
			AddChild(currentMap);
	}

	public void UpdateWeaponPickups()
	{
		foreach (Node pickup in currentMap.Pickups.GetChildren())
		{
			if (pickup is not WeaponPickup weaponPickup) return;
			weaponPickup.CheckForPlayers();
		}
	}
}
