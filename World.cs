using Godot;
using System;
using System.Collections.Specialized;
using System.Data;

public partial class World : Node2D
{
	[Export] public ScoreHUD Hud;
	public Map currentMap = null;
	public override void _Ready()
	{
		Game.Instance.world = this;
		generateMap();
	}

	void generateMap()
	{
		currentMap = Game.Instance.selectedMap;
		AddChild(currentMap);
	}

	public void UpdateWeaponPickups()
	{
		foreach (WeaponPickup pickup in currentMap.weaponPickups.GetChildren())
		{
			pickup.CheckForPlayers();
		}
	}
}
