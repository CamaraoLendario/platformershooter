using Godot;
using System;
using System.Collections.Generic;

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
    [Export] float roundStartDelay = 3f;
   	protected Dictionary<Player, int> playerTeam = [];
    protected Dictionary<int, int> teamPoints = [];
    public Dictionary<int, int> teamScoreDiference;
    
    public virtual void OnGameStarted(){}   
    public virtual void OnPlayerDied(Player died, Player killer)
    {
        if ((playerTeam[killer] == playerTeam[died]) && suicideLosesPoints)
		{
			teamPoints[playerTeam[killer]] --;
		}
		else {
			teamPoints[playerTeam[killer]] ++;
		}

		RestartRound();
//		CheckRoundOver();
    }
    protected void CheckRoundOver(){
        if (!IsRoundOver()) return;
        
        RestartRound();
    }
    public virtual bool IsRoundOver()
	{
		int winningTeam = -1;
		int currentWinnerPoints = -1;
		int potentialPoints = 0;
		for(int i = 0; i < teamPoints.Count; i++){
			if (teamPoints[i] > currentWinnerPoints){
				currentWinnerPoints = teamPoints[i];
				winningTeam = i;
			}
		}
        if (currentWinnerPoints < pointsToWin[(int)currentGameLength]) return false;
		
        foreach (Player player in playerTeam.Keys)
			if (!player.IsDead)
				potentialPoints ++;

		potentialPoints--; // not counting self
		for(int i = 0; i < teamPoints.Count; i++)
		{
			if (i == winningTeam) continue;

			if(teamPoints[i] + potentialPoints >= currentWinnerPoints) return false;
		}
		return true;
	}

    protected void RestartRound()
    {
		GD.Print("SENDING ROUND FINISHED SIGNAL NOW");
//        SignalBus.Emit(SignalBus.SignalName.RoundFinished);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.RoundFinished);
    }
}
