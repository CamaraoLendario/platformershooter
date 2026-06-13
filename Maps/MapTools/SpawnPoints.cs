using Godot;
using static SpaceMages.SpaceMagesVars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public partial class SpawnPoints : Node
{
	List<Node2D> spawnPoints = new List<Node2D>();
	
	public override void _Ready()
	{
		SignalBus.Instance.GameStarted += SpawnPlayers;
		CallDeferred(MethodName.OnNewRoundStart);
//		SignalBus.Instance.NewRoundStart += OnNewRoundStart;
	}
	void OnNewRoundStart()
	{
		ScrambleSpawnPoints();
		foreach (Player player in Game.Instance.players)
		{
			player.Position = spawnPoints[player.colorIdx].Position;
			player.CallDeferred(Player.MethodName.Reset);
		}
	}

	void ScrambleSpawnPoints()
	{
		spawnPoints.Clear();
		foreach (Node2D spawnPoint in GetChildren())
			spawnPoints.Add(spawnPoint);

		for (int i = 0; i <= 5; i++)
		{
			int randIdx = GD.RandRange(0, spawnPoints.Count() - 1);
			Node2D heldNode = spawnPoints[randIdx];
			spawnPoints.RemoveAt(randIdx);
			spawnPoints.Add(heldNode);
		}
	}

    void SpawnPlayers() {
		
		ScrambleSpawnPoints();

		Player[] players = new Player[Game.Instance.playersInfo.Length];

		for(int i = 0; i < players.Length; i++)
		{
			PlayerInfo playerInfo = Game.Instance.playersInfo[i];
			Player newPlayer = Player.New(playerInfo);
			newPlayer.Position = spawnPoints[newPlayer.colorIdx].Position;
			newPlayer.pilotSprite.SpriteFrames = GD.Load<SpriteFrames>(pilotSpriteFramesUIDs[newPlayer.colorIdx]);

			Game.GetOverworld().GetWorld().CallDeferred(MethodName.AddChild, newPlayer);
			newPlayer.CallDeferred(Player.MethodName.Reset);
			players[i] = newPlayer;
		}

		Game.Instance.players = players;
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.FinishedSpawningPlayers);
	}

	public override void _ExitTree() {
		//SignalBus.Instance.NewRoundStart -= OnNewRoundStart;
		SignalBus.Instance.GameStarted -= SpawnPlayers;
		base._ExitTree();
	}

}
