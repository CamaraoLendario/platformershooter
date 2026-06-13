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
		base.OnGameStarted();
		foreach(Player player in Game.GetPlayers())
		{
			playerTeam.Add(player, Game.Instance.playerTeams[player.GetInputIdx()]);
		}
		teamScore.Add(0, 0); // Blue
		teamScore.Add(1, 0); // Red
	}
    public override void OnPlayerDied(Player died, Player killer)
    {
		if (!objectiveIsTeamSurvive)
		{
			base.OnPlayerDied(died, killer);
			return;
		}
		List<int> aliveTeams = [];
		foreach(Player player in playerTeam.Keys)
		{
			if (player.isDead) continue;
			if (!aliveTeams.Contains(playerTeam[player]))
			{
				aliveTeams.Add(playerTeam[player]);
			}
		}

		if (aliveTeams.Count == 1){
			teamScore[aliveTeams[0]]++;
		}
		else if (aliveTeams.Count == 0){
			return; // for now. there should be code that checks for if the player/team dies or something during the little delay before starting a new round
		}
		else
		{
			return;
		}

		RestartRound();
    }
}