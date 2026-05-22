using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

public partial class Game : Node
{
	[Signal] public delegate void PausedGameEventHandler();	
	[Signal] public delegate void UnPausedGameEventHandler();
	public Dictionary<int, int> playerTeams; //inputIdx, teamIdx
	public static Game Instance { get; private set; }
	public Main main;
	public MainMenu mainMenu;
	public Map currentMap = GD.Load<PackedScene>("uid://cck3f1axqqkvm").Instantiate<Map>();
	public PlayerInfo[] playersInfo = [];
	public Player[] players = [];
	public ExperimentalFeatures experimentalFeatures;
	public enum GamemodeIdxs {
		FreeForAll,
		TEAMS,
		CaptureTheFlag
	}
	public GamemodeLogic currentGamemode;
	string[] gamemodeUIDs = [
		"uid://dfnsondfrbd6", // FreeForAll
		"uid://jbg7icuafwb0", // TEAMS
		"uid://jbg7icuafwb0", // supposed to be CaptureTheFlag, also representing teams for now, this gamemode will probably not even exist 
    ];

    public override void _Ready(){
		Instance ??= this;
	}

	public static void StartGame(PlayerInfo[] newPlayersInfo, Map map)
	{
		Instance.playersInfo = newPlayersInfo;
		Instance.currentMap = map;
		//map.SetGamemode();
		Instance.GeneratePlayerInputs();
		GetMain().StartGame();	
	}

	public void BackToMapSelector()
	{
		
	}

	void GeneratePlayerInputs()
	{
		List<int> inputIdxs = [];

		foreach (PlayerInfo playerInfo in playersInfo)
		{
			inputIdxs.Add(playerInfo.inputIdx);
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

	public static void PauseGame()
	{
		Instance.GetTree().Paused = true;
		Instance.EmitSignal(SignalName.PausedGame);
	}
	public static void UnPauseGame()
	{
		Instance.GetTree().Paused = false;
		Instance.EmitSignal(SignalName.UnPausedGame);
	}
	public Player GetPlayerFromInputIdx(int inputIdx)
	{
		foreach (Player player in players)
		{
			if (player.GetInputIdx() == inputIdx) return player;
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
		return Instance.currentMap;
	}

	public static void SetGamemode(GamemodeIdxs idx)
	{
		Instance.currentGamemode = GD.Load<GamemodeLogic>(Instance.gamemodeUIDs[(int)idx]);
	}
	public static GamemodeLogic GetGamemodeLogic()
	{
		return Instance.currentGamemode;
	}
	public static Player[] GetPlayers()
	{
		return Instance.players;
	}
	public static PlayerInfo[] GetPlayersInfo()
	{
		return Instance.playersInfo;
	}
    public static Main GetMain()
	{
		return Instance.GetTree().GetFirstNodeInGroup("Main") as Main;
	}
	public static OverWorld GetOverworld()
	{
		return Instance.GetTree().GetFirstNodeInGroup("OverWorld") as OverWorld;
	}
}
