using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static SpaceMages.SpaceMagesVars;
using System.Xml;
[GlobalClass]
public partial class MapPlaylist : Resource
{
	[Export] PackedScene[] mapPackedScenes = [
		GD.Load<PackedScene>("uid://cck3f1axqqkvm"), // base Map
		GD.Load<PackedScene>("uid://ckjbxccmrmhmb"), // Pilot Only Small
		GD.Load<PackedScene>("uid://coklf46qo3sam"), // Pilot Only
		GD.Load<PackedScene>("uid://dxgxqieibdi5n"), // Cave Map
		GD.Load<PackedScene>("uid://cjg0yfrc2mlc0"), // Poison Ridden
		GD.Load<PackedScene>("uid://tw7pvvfcnrf1"),  // Tall Pilot Zone
	];

	Map currentMap;
	int currentIdx = -1;
	List<int> mapUIDsMarblesBag = [];

	public Map GetRandMapMarbleBag()
	{
		if (mapUIDsMarblesBag.Count == 0){
			for (int i = 0; i < mapPackedScenes.Count(); i++){
				mapUIDsMarblesBag.Add(i);
			}
			mapUIDsMarblesBag.Remove(currentIdx);
		}
		currentIdx = GD.RandRange(0, mapUIDsMarblesBag.Count-1);
		currentMap = mapPackedScenes[mapUIDsMarblesBag[currentIdx]].Instantiate<Map>();
		mapUIDsMarblesBag.RemoveAt(currentIdx);
		return currentMap;
	}

	public Map GetRandMap(bool excludeCurrent)
	{	
		if (!excludeCurrent && currentIdx != -1) {
			currentIdx = GD.RandRange(0, mapPackedScenes.Length-1);
		}
		else {
			List<int> mapIdxs = [];
			for (int i = 0; i < mapPackedScenes.Count(); i++){
				mapIdxs.Add(i);
			}
			mapIdxs.Remove(currentIdx);
			currentIdx = GD.RandRange(0, mapIdxs.Count-1);
		}
		currentMap = mapPackedScenes[currentIdx].Instantiate<Map>();
		return currentMap;
	}
	public Map GetNextMap(int scrollCount = 1)
	{
		currentIdx = NormalizeIdx(currentIdx + scrollCount, mapPackedScenes.Length);
		currentMap = mapPackedScenes[currentIdx].Instantiate<Map>();

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
		currentMap = mapPackedScenes[mapIdx].Instantiate<Map>();
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
