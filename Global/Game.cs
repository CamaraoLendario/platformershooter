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
	public Map currentMap;
	public List<Dictionary<string, int>> playerInfoList;
	public List<Player> players;
	public ExperimentalFeatures experimentalFeatures;
	public Gamemode gamemode;

    public override void _Ready()
	{
		Instance ??= this;
	}

	public void StartGame(List<Dictionary<string, int>> playerInfoList, Map map)
	{
		this.playerInfoList = playerInfoList;
		this.currentMap = map;
		GeneratePlayerInputs();

		// delete main menu
		// spawn map
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

		foreach (Dictionary<string, int> player in playerInfoList)
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
		GD.PrintErr($"no player was fount with the requested inputIdx ({inputIdx}). returning null");
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
}
