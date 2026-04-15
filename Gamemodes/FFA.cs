using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

public partial class FreeForAll : Gamemode
{
	Dictionary<Player, int> playerPoints = [];
	int pointsToWin = 12;

    public override void _Ready()
    {
        SignalBus.Instance.GameStarted += OnGameStarted;
		SignalBus.Instance.playerDied += OnPlayerDied;
    }

	void OnGameStarted()
	{
		foreach(Player player in Game.Instance.players)
		{
			playerPoints.Add(player, 0);
		}
	}

	void OnPlayerDied(Player died, Player killer)
	{
		if (killer == died)
		{
			if(playerPoints[killer] > 0) 
				playerPoints[killer] --;
		}
		else
		{
			playerPoints[killer] ++;
		}

		if (IsRoundOver())
		{
			// EndRound
		}
		else
		{
			
		}
	}

	bool IsRoundOver()
	{
		Player currentWinner = null;
		int currentWinnerPoints = -1;
		int potentialPoints = 0;
		foreach(Player player in playerPoints.Keys)
		{
			if (playerPoints[player] > currentWinnerPoints)
			{
				currentWinnerPoints = playerPoints[player];
				currentWinner = player;
			}
			if (!player.IsDead)
				potentialPoints ++;
		}
		if (currentWinner == null) return false;
		potentialPoints--; // not counting self
		foreach(Player player in playerPoints.Keys)
		{
			if (player == currentWinner) continue;

			if(playerPoints[player] + potentialPoints >= currentWinnerPoints) return false;
		}
		return true;
	}

}