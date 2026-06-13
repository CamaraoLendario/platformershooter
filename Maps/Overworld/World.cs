using System;
using Godot;

public partial class World : Node2D
{
	[Export] public ScoreHUD Hud;
	public override void _Ready()
	{
		SignalBus.Instance.StartGame += OnStartGame;
	}

	void OnStartGame()
	{
		generateMap();
		SignalBus.Instance.NewRoundStart += generateMap;
	}
	void generateMap()
	{
		Map currentMap = Game.GetMap();
		if (currentMap == null) {
			currentMap = Game.GetMapPlaylist().GetRandMap(false);
		} 
		else if (currentMap.IsInsideTree()) {
			currentMap.QueueFree();
			currentMap = Game.GetMapPlaylist().GetNextMap();
		}
		
		AddChild(currentMap);
	}

	public void UpdateWeaponPickups()
	{
		foreach (Node pickup in Game.GetMap().Pickups.GetChildren()) {
			if (pickup is not WeaponPickup weaponPickup) return;
			weaponPickup.CheckForPlayers();
		}
	}

    public void ClearMap()
	{
		foreach(Node child in GetChildren())
		{
			child.QueueFree();
		}
		SignalBus.Instance.NewRoundStart -= generateMap;
	}
}
