using Godot;
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
    protected Dictionary<int, int> teamSore = [];
    public List<(int, int)> teamScoreChanges = [];
    
    public virtual void OnGameStarted()
	{
		
	}   
    public virtual void OnPlayerDied(Player died, Player killer)
    {
        if ((playerTeam[killer] == playerTeam[died]) && suicideLosesPoints)
		{
			teamScoreChanges.Add((playerTeam[killer], -1));
			GD.Print($"the team {playerTeam[killer]} has Lost a point");
		}
		else {
			teamScoreChanges.Add((playerTeam[killer], +1));
			GD.Print($"the team {playerTeam[killer]} has Gained a point");
		}
		GD.Print($"teamScoreChanges now has {teamScoreChanges.Count} entries");
		RestartRound();
//		CheckRoundOver();
    }
    protected void CheckRoundOver(){
        if (!IsRoundOver()) return;
        
        RestartRound();
    }
    public virtual bool IsRoundOver()
	{
		//TODO need to put second place here and compare that
		int winningTeam = -1;
		int currentWinnerPoints = -1;
		int potentialPoints = 0;
		for(int i = 0; i < teamSore.Count; i++){
			if (teamSore[i] > currentWinnerPoints){
				currentWinnerPoints = teamSore[i];
				winningTeam = i;
			}
		}
        if (currentWinnerPoints < GetNecessaryScore()) return false;
		
        foreach (Player player in playerTeam.Keys)
			if (!player.IsDead)
				potentialPoints ++;

		potentialPoints--; // not counting self
		for(int i = 0; i < teamSore.Count; i++)
		{
			if (i == winningTeam) continue;

			if(teamSore[i] + potentialPoints >= currentWinnerPoints) return false;
		}
		return true;
	}

    protected async void RestartRound()
    {
		await ToSignal(Game.Instance.GetTree().CreateTimer(roundEndDelay), Timer.SignalName.Timeout);		
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.RoundFinished);
		
		CallDeferred(MethodName.MergeScoreChanges);
		//SetDeferred();
    }
    void MergeScoreChanges()
	{
		foreach((int team, int points) in teamScoreChanges)
		{
			teamSore[team] += points;
		}
		teamScoreChanges.Clear();
	}
	public int GetNecessaryScore()
	{
		return pointsToWin[(int)currentGameLength];
	}
	protected void RegisterTeam(Player player, int teamIdx)
	{
		if(!playerTeam.Keys.Contains(player))
			playerTeam.Add(player, teamIdx);
		if (teamSore.Keys.Contains(teamIdx))
			return;
		teamSore.Add(teamIdx, 0);
	} 
}
