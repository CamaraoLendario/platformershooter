using Godot;
using SpaceMages;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FFA : GameMode
{
	protected override void OnPlayerDead(Player died, Player killer)
	{
		base.OnPlayerDead(died, killer);

		string currentScoreAnnouncement = "Current Score is: \n";
		foreach ((int Team, int Score) in teamsScore)
		{
			currentScoreAnnouncement += SpaceMagesVars.teamColorsDict.Keys.ElementAt(Team) + " Score: " + Score + "\n";
		}

		GD.Print(currentScoreAnnouncement);
		if (teamsScore[killer.colorIdx] >= scoreToWin)
        {
			GD.Print("AND WE HAVE A WINNER! " + killer.colorIdx + " has won!");
			EmitSignal(SignalName.WinnerDecided, killer.colorIdx);
        }
	}

}
