using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class GamemodeLogic : Resource
{
    [Export] public bool isTeamed = true;
    [Export] public bool suicideLosesPoints = false;
    [Export] public Game.GamemodeIdxs INDEX {get; protected set;}
    [Export] int[] pointsToWin = [8, 12, 18];
	public enum GameLength {
		Short,
		Medium,
		Long,
	}
	public GameLength CurrentGameLength {
		get {
			return currentGameLength;
		}
		set {
			currentGameLength = value;
		}
	}
	GameLength currentGameLength;
    [Export] float roundEndDelay = 2f;
   	protected Dictionary<Player, int> playerTeam = [];
    protected Dictionary<int, int> teamScore = [];
    public List<(int, int)> teamScoreChanges = [];
	protected bool isRoundRestarting = false;
	int winningTeam = -1;
	
	public void Ready()
	{
		
	}
    public virtual void OnGameStarted()
	{

	} 
	public virtual void OnFinishedSpawningPlayers()
	{
		
	} 
    public virtual void OnPlayerDied(Player died, Player killer)
    {
		GD.Print("----Begin OnPlayerDied() in GamemodeLogic:----");
        if (playerTeam[killer] == playerTeam[died])
		{
			if(suicideLosesPoints) {
				teamScoreChanges.Add((playerTeam[killer], -1));
				GD.Print($"the team {playerTeam[killer]} has Lost a point");
			}
		}
		else {
			teamScoreChanges.Add((playerTeam[killer], +1));
			GD.Print($"the team {playerTeam[killer]} has Gained a point");
		}
		GD.Print($"teamScoreChanges now has {teamScoreChanges.Count} entries");
	
		CheckRoundOver();
		GD.Print("----End OnPlayerDied() in GamemodeLogic:----");
    }
    protected void CheckRoundOver(){
        RestartRound();
    }
	public virtual bool IsRoundOver()
	{
		return GetAlivePlayerCount() <= 1;
	}
    public virtual bool IsGameOver()
	{
		winningTeam = teamScore.Keys.First();
		int winningScore = 0;
		int nextTeam = teamScore.Keys.First();
		int nextScore = 0;

		foreach (int team in teamScore.Keys)
		{
			int currentTeamScore = teamScore[team];
			if (currentTeamScore > winningScore)
			{
				if (winningScore >= nextScore)
				{
					nextTeam = winningTeam;
					nextScore = winningScore;
				}
				winningTeam = team;	
				winningScore = currentTeamScore;
			}
			else if (currentTeamScore > nextScore)
			{
				nextTeam = team;
				nextScore = currentTeamScore;
			}
		}
		GD.Print($"score: {winningScore} GetWinningScore(): {GetWinningScore()}");
		if (winningScore < GetWinningScore()) return false; 
		if (nextScore + GetAvailableScore(nextTeam) >= winningScore)
		{
			//do something for deathmatch maybe? but game is not over yet
			return false;
		}
		return true;
	}

    protected async void RestartRound()
    {
		GD.Print("RestartingRound...");
		GD.Print("Is Round restarting? ", isRoundRestarting);
		if (isRoundRestarting) return;
		isRoundRestarting = true;
		await ToSignal(Game.Instance.GetTree().CreateTimer(roundEndDelay), Timer.SignalName.Timeout);		
		GD.Print("Emitting the RoundFinished signal");
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.RoundFinished);
		CallDeferred(MethodName.MergeScoreChanges);
		SetDeferred(PropertyName.isRoundRestarting, false);
    }
    void MergeScoreChanges()
	{
		foreach((int team, int points) in teamScoreChanges)
		{
			teamScore[team] += points;
		}
		teamScoreChanges.Clear();
	}
	public int GetWinningScore()
	{
		return pointsToWin[(int)currentGameLength];
	}
	protected void RegisterTeam(Player player, int teamIdx)
	{
		if(!playerTeam.Keys.Contains(player))
			playerTeam.Add(player, teamIdx);
		if (teamScore.Keys.Contains(teamIdx))
			return;
		teamScore.Add(teamIdx, 0);
	} 

	protected virtual int GetAvailableScore(int forTeam)
	{
		return 0;
	}
	int GetAlivePlayerCount()
	{
		int count = 0;
		foreach(Player player in Game.GetPlayers())
		{
			if (!player.isDead)
				count++;
		}
		
		return count;
	}

	public int GetWinningTeam()
	{
		return winningTeam;
	}



}
