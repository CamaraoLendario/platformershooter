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
    public override void OnStartGame()
	{
		base.OnStartGame();
		foreach(PlayerInfo playerInfo in Game.GetPlayersInfo())
		{
			/* playerTeamByInputIdx.Add(playerInfo.inputIdx, ); */ // TODO make it so this can read teams from team select screen
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
		foreach(Player player in Game.GetPlayers())
		{
			if (player.isDead) continue;
			if (!aliveTeams.Contains(playerTeamByInputIdx[player.GetInputIdx()])) {
				aliveTeams.Add(playerTeamByInputIdx[player.GetInputIdx()]);
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