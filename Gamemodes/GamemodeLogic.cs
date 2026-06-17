using Godot;
using Godot.NativeInterop;
using Microsoft.VisualBasic;
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
   	public Dictionary<int, int> playerTeamByInputIdx = []; //inputIdx, Team
	public List<(string teamName, int teamColorIdx)> teams = [];
    protected Dictionary<int, int> teamScore = [];
    public List<(int, int)> teamScoreChanges = [];
	protected bool isRoundRestarting = false;
	int winningTeam = -1;
	
	public void Ready()
	{

	}
	public virtual void Reset()
	{
		playerTeamByInputIdx = [];
		teams = [];
		teamScore = [];
		teamScoreChanges = [];
		winningTeam = -1;
	}

    public virtual void OnStartGame()
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
		int diedIdx = died.GetInputIdx();
		int killerIdx = killer.GetInputIdx();
        if (playerTeamByInputIdx[killerIdx] == playerTeamByInputIdx[diedIdx])
		{
			if(suicideLosesPoints) {
				teamScoreChanges.Add((playerTeamByInputIdx[killerIdx], -1));
				teamScore[playerTeamByInputIdx[killerIdx]] -= 1;
				GD.Print($"the team {playerTeamByInputIdx[killerIdx]} has Lost a point");
			}
		}
		else {
			teamScoreChanges.Add((playerTeamByInputIdx[killerIdx], +1));
			teamScore[playerTeamByInputIdx[killerIdx]] += 1;
			GD.Print($"the team {playerTeamByInputIdx[killerIdx]} has Gained a point");
		}
		GD.Print($"teamScoreChanges now has {teamScoreChanges.Count} entries");
		foreach ((int Team, int Score) scoreToAddInfo in teamScoreChanges)
		{
			GD.Print($"The team {scoreToAddInfo.Team} gets {scoreToAddInfo.Score} points");	
		}
		if (IsGameOver())
		{
			SetupGameOverOvertime();
		}
		else
			CheckRoundOver();
		GD.Print("----End OnPlayerDied() in GamemodeLogic:----");
    }
    protected void CheckRoundOver(){
		if (GetAlivePlayerCount() <= 1)
     	   RestartRound();
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

	async void SetupGameOverOvertime()
	{
		await ToSignal(Game.Instance.GetTree().CreateTimer(roundEndDelay), Timer.SignalName.Timeout);
		if (IsGameOver()) {
			Game.PauseGame();
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameFinished, GetWinningTeamColor());
		}
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
		teamScoreChanges.Clear();
		SetDeferred(PropertyName.isRoundRestarting, false);
    }
	
	public int GetWinningScore()
	{
		return pointsToWin[(int)currentGameLength];
	}
	protected virtual void RegisterPlayer(int playerInputIdx, int teamIdx) {

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

	public int GetWinningTeamColor()
	{
		return teams[winningTeam].teamColorIdx;
	}

}
