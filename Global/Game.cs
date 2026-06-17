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
	public Map currentMap;
	public MapPlaylist mapPlaylist;
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
	// public bool isGamePaused = false;

    public override void _Ready(){
		Instance ??= this;
	}

	public static void StartGame(PlayerInfo[] newPlayersInfo, MapPlaylist playlist, int mapIdx = 0)
	{
		Main main = GetMain();
		//await main.ToSignal(main.constantOverlay.FadeOut(), Tween.SignalName.Finished);
		Instance.playersInfo = newPlayersInfo;
		Instance.mapPlaylist = playlist;
		Instance.currentMap = playlist.GetMap(mapIdx);
		//map.SetGamemode();
		Instance.GeneratePlayerInputs();
		main.StartGame();
	}

	void GeneratePlayerInputs()
	{
		List<int> inputIdxs = [];

		foreach (PlayerInfo playerInfo in playersInfo) {
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
		GD.Print("Paused Game");
		if(Instance.GetTree().Paused) return;
		Instance.GetTree().Paused = true;
		// if(Instance.isGamePaused) return;
		// Instance.isGamePaused = true;
		Instance.EmitSignal(SignalName.PausedGame);
	}
	public static void UnPauseGame()
	{
		GD.Print("UnPaused Game");
		if(!Instance.GetTree().Paused) return;
		Instance.GetTree().Paused = false;
		// if(!Instance.isGamePaused) return;
		// Instance.isGamePaused = false;
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
			if (!player.isDead)
				aliveCount ++;
		}
		return aliveCount;
	}
	public static Map GetMap()
	{
		return Instance.mapPlaylist.GetCurrentMap();
	}
	public static MapPlaylist GetMapPlaylist()
	{
		return Instance.mapPlaylist;
	}

	public static void SetGamemode(GamemodeIdxs idx)
	{
		Instance.currentGamemode = GD.Load<GamemodeLogic>(Instance.gamemodeUIDs[(int)idx]).Duplicate() as GamemodeLogic;
	}
	public static GamemodeLogic GetGamemodeLogic()
	{
		return Instance.currentGamemode;
	}
	public static Player[] GetPlayers()
	{
		return Instance.players;
	}
	public static Player[] GetAlivePlayers()
	{
		List<Player> alivePlayers = [];
		foreach (Player player in Instance.players)
		{
			if (!player.isDead)
				alivePlayers.Add(player);
		}
		return alivePlayers.ToArray();
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

	public static void ClearPlayerInfo()
	{
		Instance.players = [];
		//Instance.playersInfo = [];
		Instance.currentMap = null;
		Instance.mapPlaylist = null;
	}

    public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (Input.IsKeyPressed(Key.Ctrl))
			{
				if (Input.IsKeyPressed(Key.I))
				{
					foreach (StringName action in InputMap.GetActions())
					{
						if (action.ToString().StartsWith("ui")) continue;
						GD.Print($"Action: {action}");
						foreach (InputEvent key in InputMap.ActionGetEvents(action))
						{
							GD.Print(key.AsText());
						}
					}
				}
			}
		}
	}


}
