using Godot;
using SpaceMages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public partial class SpawnPoints : Node
{
	List<Node2D> spawnPoints = new List<Node2D>();

	public override void _Ready()
	{
		ScrambleSpawnPoints();
		SpawnPlayers();
		SignalBus.Instance.NewRoundStarted += OnNewRoundStarted;
	}
	void SpawnPlayers()
	{
		foreach (Dictionary<string, int> playerInfo in Game.Instance.playerInfoList)
		{
			Player newPlayer = Player.playerScene.Instantiate<Player>();
			newPlayer.Name = playerInfo.Keys.First();
			newPlayer.NameLabel.Text = newPlayer.Name;
			newPlayer.inputIdx = playerInfo["inputIdx"];
			if (newPlayer.inputIdx == -1) newPlayer.isKeyboardControlled = true;
			newPlayer.SetColor(playerInfo["colorIdx"]);
			newPlayer.Position = spawnPoints[newPlayer.colorIdx].Position;

			Game.Instance.AddPlayer(newPlayer);
			newPlayer.CallDeferred("Reset");
		}
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.FinishedSpawningPlayers);
	}
	void OnNewRoundStarted()
	{
		ScrambleSpawnPoints();
		foreach (Player player in Game.Instance.players)
		{
			player.Position = spawnPoints[player.colorIdx].Position;
			player.Reset();
			player.IsDead = false;
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

	public override void _ExitTree()
	{
		SignalBus.Instance.NewRoundStarted -= OnNewRoundStarted;
		base._ExitTree();
	}

}
