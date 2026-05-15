using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using static SpaceMages.SpaceMagesVars;

[GlobalClass]
public partial class TeamsLogic : GamemodeLogic
{
	[Export] bool objectiveIsTeamSurvive = true;
    public override void OnGameStarted()
	{
		foreach(Player player in Game.GetPlayers())
		{
			playerTeam.Add(player, Game.Instance.playerTeams[player.GetInputIdx()]);
		}
		teamPoints.Add(0, 0); // Blue
		teamPoints.Add(1, 0); // Red
	}
    public override void OnPlayerDied(Player died, Player killer)
    {
		if (!objectiveIsTeamSurvive)
		{
			base.OnPlayerDied(died, killer);
			return;
		}
		int[] aliveTeams = [];
		foreach(Player player in playerTeam.Keys)
		{
			if (player.IsDead) continue;
			if (!aliveTeams.Contains(playerTeam[player]))
			{
				aliveTeams = aliveTeams.Append(playerTeam[player]).ToArray();
			}
		}

		if (aliveTeams.Length == 1){
			teamPoints[aliveTeams[0]]++;
		}
		else if (aliveTeams.Length == 0){
			
		}
		else
		{
			return;
		}

		RestartRound();
    }
}