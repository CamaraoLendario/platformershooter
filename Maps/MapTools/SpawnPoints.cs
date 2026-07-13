using Godot;
using static SpaceMages.SpaceMagesVars;

public partial class SpawnPoints : Node
{
	Node2D[] spawnPoints;
	Node ffaPoints;
	Node teamsPoints;
	Node2D[] team0;
	Node2D[] team1;
	
	public override void _Ready()
	{
		ffaPoints = GetNode("FFAPoints");
		teamsPoints = GetNode("TEAMSPoints");
		ScrambleSpawnPoints();
		SignalBus.Instance.GameStarted += SpawnPlayers;
		CallDeferred(MethodName.OnNewRoundStart);
//		SignalBus.Instance.NewRoundStart += OnNewRoundStart;
	}
	void OnNewRoundStart()
	{
		ScrambleSpawnPoints();
		Player[] players = Game.Instance.players;
		if (!Game.GetGamemodeLogic().isTeamed)
			for(int i = 0; i < players.Length; i++)
			{
				Player player = players[i];
				player.Position = spawnPoints[i].Position;
				player.CallDeferred(Player.MethodName.Reset);
			}
		else
		{
			int red = 0;
			int blue = 0;

			for(int i = 0; i < players.Length; i++)
			{
				Player player = players[i];
				if (player.GetTeam() == 0) {
					player.Position = team0[red].Position;
					player.CallDeferred(Player.MethodName.Reset);
					red++;
				}
				else {
					player.Position = team1[blue].Position;
					player.CallDeferred(Player.MethodName.Reset);
					blue++;
				}
			}
		}
	}

	void ScrambleSpawnPoints()
	{
		if (!Game.GetGamemodeLogic().isTeamed)
			ScrambleFFA();
		else ScrambleTeams();
	}
	void ScrambleFFA() {
		int pointsCount = ffaPoints.GetChildCount();
		spawnPoints = new Node2D[pointsCount];
	
		for (int i = 0; i < pointsCount; i++)
			spawnPoints[i] = ffaPoints.GetChild<Node2D>(i);

		for (int i = 0; i < pointsCount; i++)
		{
			int randIdx = GD.RandRange(0, pointsCount - 1);
			(spawnPoints[i], spawnPoints[randIdx]) = (spawnPoints[randIdx], spawnPoints[i]);
		}
	}

	void ScrambleTeams()
	{
		Node team0Parent = teamsPoints.GetChild(0);
		Node team1Parent = teamsPoints.GetChild(1);

		int team0Count = team0Parent.GetChildCount();
		int team1Count = team1Parent.GetChildCount();

		team0 = new Node2D[team0Count];
		team1 = new Node2D[team1Count];
		
		for (int i = 0; i < team0Count; i++) {
			team0[i] = team0Parent.GetChild<Node2D>(i);
		}
		for (int i = 0; i < team1Count; i++) {
			team1[i] = team1Parent.GetChild<Node2D>(i);
		}
		
		for (int i = 0; i < team0Count; i++) {
			int randIdx = GD.RandRange(0, team0Count - 1);
			(team0[i], team0[randIdx]) = (team0[randIdx], team0[i]);
		}
		for (int i = 0; i < team1Count; i++) {
			int randIdx = GD.RandRange(0, team1Count - 1);
			(team1[i], team1[randIdx]) = (team1[randIdx], team1[i]);
		}
	}

    void SpawnPlayers() {
		
		ScrambleSpawnPoints();
		if (!Game.GetGamemodeLogic().isTeamed) {
			SpawnPlayersFFA();
		}
		else SpawnPlayersTeams();
	}

    private void SpawnPlayersFFA()
    {
        Player[] players = new Player[Game.Instance.playersInfo.Length];

		for(int i = 0; i < players.Length; i++) {
			PlayerInfo playerInfo = Game.Instance.playersInfo[i];
			Player newPlayer = Player.New(playerInfo);
			newPlayer.Position = spawnPoints[newPlayer.colorIdx].Position;
			newPlayer.pilotSprite.SpriteFrames = GD.Load<SpriteFrames>(pilotSpriteFramesUIDs[newPlayer.colorIdx]);

			Game.GetOverworld().GetWorld().CallDeferred(MethodName.AddChild, newPlayer);
			newPlayer.CallDeferred(Player.MethodName.Reset);
			players[i] = newPlayer;
		}

		Game.Instance.players = players;
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.FinishedSpawningPlayers);
    }

    private void SpawnPlayersTeams()
	{
        Player[] players = new Player[Game.Instance.playersInfo.Length];
		
		int red = 0;
		int blue = 0;

		for(int i = 0; i < players.Length; i++)
		{
			PlayerInfo playerInfo = Game.Instance.playersInfo[i];
			Player newPlayer = Player.New(playerInfo);
			if (newPlayer.GetTeam() == 0) {
				newPlayer.Position = team0[red].Position;
				red++;
			}
			else {
				newPlayer.Position = team1[blue].Position;
				blue++;
			}
			newPlayer.pilotSprite.SpriteFrames = GD.Load<SpriteFrames>(pilotSpriteFramesUIDs[newPlayer.colorIdx]);

			Game.GetOverworld().GetWorld().CallDeferred(MethodName.AddChild, newPlayer);
			newPlayer.CallDeferred(Player.MethodName.Reset);
			players[i] = newPlayer;

		}

		Game.Instance.players = players;
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.FinishedSpawningPlayers);
	}

    public override void _ExitTree() {
		//SignalBus.Instance.NewRoundStart -= OnNewRoundStart;
		SignalBus.Instance.GameStarted -= SpawnPlayers;
		base._ExitTree();
	}

}
