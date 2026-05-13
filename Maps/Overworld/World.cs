using Godot;

public partial class World : Node2D
{
	[Export] public ScoreHUD Hud;
	public override void _Ready()
	{
		generateMap();
	}

	void generateMap()
	{
		Map currentMap = Game.GetMap();
		if (currentMap == null) 
			currentMap = Game.Instance.currentMap;
			
		if (!currentMap.IsInsideTree())
			AddChild(currentMap);
	}

	public void UpdateWeaponPickups()
	{
		foreach (Node pickup in Game.GetMap().Pickups.GetChildren())
		{
			if (pickup is not WeaponPickup weaponPickup) return;
			weaponPickup.CheckForPlayers();
		}
	}
}
