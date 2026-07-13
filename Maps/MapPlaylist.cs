using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static SpaceMages.SpaceMagesVars;
using System.Xml;
[GlobalClass]
public partial class MapPlaylist : Resource
{
	[Export] string[] mapUIDs = [
		"uid://cck3f1axqqkvm", // base Map
		"uid://ckjbxccmrmhmb", // Pilot Only Small
		"uid://coklf46qo3sam", // Pilot Only
		"uid://dxgxqieibdi5n", // Cave Map
		"uid://cjg0yfrc2mlc0", // Poison Ridden
		"uid://tw7pvvfcnrf1",  // Tall Pilot Zone
	];

	Map currentMap;
	int currentIdx = -1;

	public Map GetRandMap(bool excludeCurrent)
	{	
		string uid;
		if (!excludeCurrent && currentIdx != -1) {
			currentIdx = GD.RandRange(0, mapUIDs.Length);
			uid = mapUIDs[currentIdx];
		}
		else {
			List<string> newMapUIDs = mapUIDs.ToList();
			newMapUIDs.Remove(mapUIDs[currentIdx]);
			currentIdx = GD.RandRange(0, newMapUIDs.Count);
			uid = newMapUIDs[currentIdx];
		}
		currentMap = GD.Load<PackedScene>(uid).Instantiate<Map>();
		return currentMap;
	}
	public Map GetNextMap(int scrollCount = 1)
	{
		currentIdx = NormalizeIdx(currentIdx + scrollCount, mapUIDs.Length);
		currentMap = GD.Load<PackedScene>(mapUIDs[currentIdx]).Instantiate<Map>();

		GD.Print("maxplayerCount: ", currentMap.maxPlayerCount);
		GD.Print("player count:", Game.GetPlayersInfo().Count());

		if (!hasEnoughSlots(currentMap)) {
			return GetNextMap();
		}
		return currentMap;
	}
	public Map GetMap(int mapIdx)
	{
		currentIdx = mapIdx;
		currentMap = GD.Load<PackedScene>(mapUIDs[mapIdx]).Instantiate<Map>();
		if (!hasEnoughSlots(currentMap)) {
			return GetNextMap();
		}
		return currentMap;
	}
	public Map GetCurrentMap()
	{
		return currentMap;
	}

	bool hasEnoughSlots(Map map)
	{
		return currentMap.maxPlayerCount >= Game.GetPlayersInfo().Count();
	}
}
