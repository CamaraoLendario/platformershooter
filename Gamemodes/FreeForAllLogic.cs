using System.Linq;
using Godot;

[GlobalClass]
public partial class FreeForAllLogic : GamemodeLogic
{

    public override void OnGameStarted(){
		PlayerInfo[] playersInfo = Game.GetPlayersInfo();
		for(int i = 0; i < playersInfo.Length; i++)
		{
			RegisterPlayer(playersInfo[i].inputIdx, i);
			teams.Add((playersInfo[i].Name, playersInfo[i].colorIdx));
		}
	}

    protected override void RegisterPlayer(int playerInputIdx, int teamIdx) {
		if(!playerTeamByInputIdx.Keys.Contains(playerInputIdx))
			playerTeamByInputIdx.Add(playerInputIdx, teamIdx);
		if (teamScore.Keys.Contains(teamIdx))
			return;
		teamScore.Add(teamIdx, 0);
		
    }


}	