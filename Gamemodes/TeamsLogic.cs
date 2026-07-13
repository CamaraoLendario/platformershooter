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
	Vector3 redTeamColor = new Vector3(0.8f, 0.1f, 0.1f);
	Vector3 blueTeamColor = new Vector3(0.1f, 0.1f, 0.8f);
    public override void OnGameStarted()
	{
		base.OnStartGame();

		teamScore.Add(0, 0); // Blue
		teamScore.Add(1, 0); // Red

		teams.Add(("Red Team", 0));
		teams.Add(("Blue Team", 2));
	}
    public override void OnFinishedSpawningPlayers()
	{
		foreach(Player player in Game.GetPlayers())
		{
			if (playerTeamByInputIdx[player.GetInputIdx()] == 0)
			{
				(player.Material as ShaderMaterial).SetShaderParameter("Color", redTeamColor);
				(player.pilotShield.Material as ShaderMaterial).SetShaderParameter("Color", redTeamColor);
				(player.particlesHandler.pilotShieldBreak.ProcessMaterial as ShaderMaterial).SetShaderParameter("Color", redTeamColor);	
			}
			else
			{
				(player.Material as ShaderMaterial).SetShaderParameter("Color", blueTeamColor);
				(player.pilotShield.Material as ShaderMaterial).SetShaderParameter("Color", blueTeamColor);
				(player.particlesHandler.pilotShieldBreak.ProcessMaterial as ShaderMaterial).SetShaderParameter("Color", blueTeamColor);
			}
		}
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
			teamScoreChanges.Add((aliveTeams[0], 1));
		}
		else if (aliveTeams.Count == 0){
			return; // for now. there should be code that checks for if the player/team dies or something during the little delay before starting a new round
		}
		else
		{
			return;
		}

		if (IsGameOver())
		{
			SetupGameOverOvertime();
		}
		else RestartRound();
    }
    public override bool IsGameOver()
    {
		if (Mathf.Max(teamScore[0], teamScore[1]) >= GetWinningScore() && teamScore[0] != teamScore[1])
		{
			if (teamScore[0] > teamScore[1])
				winningTeam = 0;
			else winningTeam = 1;
			return true;
		}
		
        return false;
    }
    protected override void CheckRoundOver()
    {
        
    }


}