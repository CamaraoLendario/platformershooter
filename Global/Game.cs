using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public partial class Game : Node
{
	[Signal] public delegate void PausedGameEventHandler();	
	[Signal] public delegate void UnPausedGameEventHandler();
	[Signal] public delegate void playerDiedEventHandler(Player died, Player killer);
	[Signal] public delegate void PlayerAddedEventHandler(Player player);
	[Signal] public delegate void GameStartedEventHandler();
	[Signal] public delegate void NewRoundStartedEventHandler();
	[Signal] public delegate void RoundFinishedEventHandler();	
	public static Game Instance { get; private set; }
	public List<Dictionary<string, int>> players { get; private set; }
	public Dictionary<int, Player> playerNodesByColor = []; //color, node
	public Dictionary<int, Player> playerNodesByInputIdx = []; //color, node
	public List<LinearProjectile> BulletsNodes = [];
	public int playerCount = 1;
	public int alivePlayerCount;
	public World world;
	public ScoreBoard scoreBoard;
	public GameMode gameMode;
	public Map selectedMap;
	public int chosenWinningScore = 12;
	public int chosenWinningScoreIdx = 1;
	public List<Dictionary <string, int>> currentPlayerInfoList;
	public ExperimentalFeatures experimentalFeatures;

	public override void _Ready()
	{
		Instance ??= this;
	}

	void ChooseGamemode(FFA gameMode)
	{
		gameMode.scoreToWin = chosenWinningScore;
		this.gameMode = gameMode;
		(this.gameMode as FFA).OnGameStart();
		gameMode.GameOver += () => {
			BackToMapSelector();
		};
		EmitSignal(SignalName.GameStarted);
		GPUParticlesPool.NormalizePoolCountToPlayerCount();
	}

	public void BackToMapSelector()
    {
		if (gameMode != null && !gameMode.IsQueuedForDeletion())
        {
			gameMode.OnGameOver();
        }
		else
		{
			GetTree().ChangeSceneToFile("uid://gclokew2nun0");
		}
    }

	public void AddPlayer(Player newPlayer)
	{
		newPlayer.died += OnPlayerDeath;
		world.AddChild(newPlayer);
		playerNodesByColor.Add(newPlayer.colorIdx, newPlayer);
		playerNodesByInputIdx.Add(newPlayer.inputIdx, newPlayer);
		CallDeferred(MethodName.EmitSignal, SignalName.PlayerAdded, newPlayer);
	}
	
	public void OnFinishedSpawningPlayers()
	{
		ChooseGamemode(new FFA()); //TODO: have a place to choose the gamemode
		scoreBoard.Initialize(playerNodesByColor);
	}
	
	void OnPlayerDeath(Player died, Player killer)
	{
		Dictionary<int, Player> newPlayerNodes = playerNodesByColor;
		alivePlayerCount -= 1;

		EmitSignal(SignalName.playerDied, died, killer);
		GD.Print("player Died", alivePlayerCount);
		gameMode.IsGameOver();
		if (alivePlayerCount <= 1)
		{
			if (!gameMode.IsGameOver())
			{
				GD.Print("--ROUND FINISHED--");
				EmitSignal(SignalName.RoundFinished);
			}
		}
	}

	public void RestartRound()
	{
		alivePlayerCount = playerCount;
		EmitSignal(SignalName.NewRoundStarted);
	}
	
	public void StartGame(List<Dictionary<string, int>> players, Map selectedMap)
	{
		this.selectedMap = selectedMap; 
		this.players = players;
		SendInputGenerationRequest();
		playerNodesByColor.Clear();
		playerNodesByInputIdx.Clear();
		playerCount = players.Count();
		alivePlayerCount = playerCount;
		CallDeferred(MethodName.StartGame);
	}
	void StartGame()
	{
		GetTree().ChangeSceneToFile("res://world.tscn");
	}

	void SendInputGenerationRequest()
	{
		List<int> inputIdxs = [];

		foreach (Dictionary<string, int> player in players)
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
}
