using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class FreeForAllLogic : GamemodeLogic
{

	public override void OnGameStarted()
	{
		base.OnGameStarted();
		Player[] players = Game.Instance.players;
		for(int i = 0; i < players.Length; i++)
		{
			RegisterTeam(players[i], i);
		}
	}

}	