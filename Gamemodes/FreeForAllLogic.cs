using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class FreeForAllLogic : GamemodeLogic
{

	public override void OnGameStarted()
	{
		int i = 0;
		foreach(Player player in Game.Instance.players)
		{
			playerTeam.Add(player, i);
			teamPoints.Add(i, 0);
			i++;
		}
	}

	
}	