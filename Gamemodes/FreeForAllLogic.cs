using Godot;

[GlobalClass]
public partial class FreeForAllLogic : GamemodeLogic
{

	public override void OnFinishedSpawningPlayers()
	{
		base.OnFinishedSpawningPlayers();
		Player[] players = Game.Instance.players;
		for(int i = 0; i < players.Length; i++)
		{
			RegisterTeam(players[i], i);
		}
	}

}	