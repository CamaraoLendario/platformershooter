using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameMode : Node
{
	[Signal] public delegate void WinnerDecidedEventHandler(int colorIdx);
	[Signal] public delegate void GameOverEventHandler();
	
	public int scoreToWin = 12;
	public Dictionary<int, int> teamsScore = [];
	public Dictionary<int, int> addedTeamsScore = [];
	
	Timer RoundEndDelayTimer = new Timer();
	float RoundEndDelayTime = 5f;
	
	public GameMode()
	{
		Game.Instance.playerDied += OnPlayerDead;
	}

	public virtual void OnGameStart()
	{
		teamsScore.Clear();
		RoundEndDelayTimer.OneShot = true;
		Game.Instance.world.AddChild(RoundEndDelayTimer);
		RoundEndDelayTimer.Timeout += () => CallDeferred(MethodName.OnGameOver);
		foreach (Player player in Game.Instance.playerNodesByColor.Values)
		{
			if (!teamsScore.Keys.Contains(player.colorIdx))
			{
				teamsScore.Add(player.colorIdx, 0);
				addedTeamsScore.Add(player.colorIdx, 0);
			}
		}
	}

	protected virtual void OnPlayerDead(Player died, Player killer)
	{
		for(int i = 0; i < teamsScore.Count; i++)
        {
            GD.Print(teamsScore.Keys.ElementAt(i) + " | " + teamsScore[teamsScore.Keys.ElementAt(i)]);
        }
		if (killer == died && this is FFA)
		{
			teamsScore[killer.colorIdx]--;
			addedTeamsScore[killer.colorIdx]--;
		}
		else
			teamsScore[killer.colorIdx]++;
			addedTeamsScore[killer.colorIdx]++;
	}
	
	public void ResetAddedTeamsScore()
    {
		foreach(int key in addedTeamsScore.Keys)
        {
			addedTeamsScore[key] = 0;
        }
    }

	public virtual bool IsGameOver()
    {
		foreach(int teamScore in teamsScore.Values)
        {
            if (teamScore >= scoreToWin)
            {
				if (RoundEndDelayTimer == null || (RoundEndDelayTimer != null && !RoundEndDelayTimer.IsInsideTree()))
				{
					RoundEndDelayTimer = new Timer();
					RoundEndDelayTimer.OneShot = true;
					Game.Instance.world.AddChild(RoundEndDelayTimer);
					RoundEndDelayTimer.Timeout += () => CallDeferred(MethodName.OnGameOver);
				}
				RoundEndDelayTimer.CallDeferred(Timer.MethodName.Start, RoundEndDelayTime);
                return true;
            }
        }
		return false;
    }

	public void OnGameOver()
    {
		QueueFree();
		InputGenerator.Instance.ClearExtraInputs(true);
		EmitSignal(SignalName.GameOver);
		Game.Instance.playerDied -= OnPlayerDead;
    }
}
