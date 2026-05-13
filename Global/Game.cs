using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

public partial class Game : Node
{
	[Signal] public delegate void PausedGameEventHandler();	
	[Signal] public delegate void UnPausedGameEventHandler();
	public static Game Instance { get; private set; }
	public Main main;
	public MainMenu mainMenu;
	public Map currentMap = GD.Load<PackedScene>("uid://cck3f1axqqkvm").Instantiate<Map>();
	public Dictionary<string, int>[] playersInfo = [];
	public List<Player> players;
	public ExperimentalFeatures experimentalFeatures;
	public Gamemode gamemode;
	public enum GamemodeIdxs {
		FreeForAll,
		TEAMS,
		CaptureTheFlag
	}
	public Gamemode[] Gamemodes
	{
		
	}

    public override void _Ready(){
		Instance ??= this;
	}

	public void StartGame(Dictionary<string, int>[] newPlayersInfo, Map map)
	{
		playersInfo = newPlayersInfo;
		currentMap = map;
		GeneratePlayerInputs();
		(GetTree().GetFirstNodeInGroup("Main") as Main).StartGame();
		
	}
	
	public void AddPlayer(Player player)
	{
		players.Add(player);
	}

	public void BackToMapSelector()
	{
		
	}

	public void RestartRound()
	{
		
	}

	void GeneratePlayerInputs()
	{
		List<int> inputIdxs = [];

		foreach (Dictionary<string, int> player in playersInfo)
		{
			inputIdxs.Add(player["inputIdx"]);
		}

		InputGenerator.Instance.GeneratePlayersInput(inputIdxs);
	}

	public async void ApplyHitstop(float stopTime)
	{
		//Engine.TimeScale = 0.01f;
		PauseGame();
		await ToSignal(GetTree().CreateTimer(stopTime), "timeout");
		//Engine.TimeScale = 1f;
		UnPauseGame();
	}

	public void PauseGame()
	{
		GetTree().Paused = true;
		EmitSignal(SignalName.PausedGame);
	}
	public void UnPauseGame()
	{
		GetTree().Paused = false;
		EmitSignal(SignalName.UnPausedGame);
	}
	public Player GetPlayerFromInputIdx(int inputIdx)
	{
		foreach (Player player in players)
		{
			if (player.inputIdx == inputIdx) return player;
		}
		GD.PrintErr($"No player was found with the requested inputIdx ({inputIdx}). returning null");
		return null;
	}

	public int GetAlivePlayerCount()
	{
		int aliveCount = 0;
		foreach (Player player in players)
		{
			if (!player.IsDead)
				aliveCount ++;
		}
		return aliveCount;
	}
	public static Map GetMap()
	{
		return Game.Instance.currentMap;
	}

	Gamemode GetGamemode(GamemodeIdxs gamemodeIdx)
	{
		
	}
}
