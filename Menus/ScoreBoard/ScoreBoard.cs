using Godot;
using SpaceMages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

public partial class ScoreBoard : Control
{
	[Export] PackedScene individualScoreCounterScene;
	[Export] VBoxContainer scoreCountersContainer;
	Dictionary<int, int> oldTeamsScore = [];
	List<IndividualScoreCounter> scoreCounters = [];

	public override void _Ready()
	{
		SignalBus.Instance.RoundFinished += AddScoreToAll;
	}

	public void Initialize(Dictionary<int, Player> players)
	{
		for (int i = 0 ; i < players.Count; i++)//int colorIdx in players.Keys)
		{
			int colorIdx = players.Keys.ElementAt(i);
			IndividualScoreCounter counter = individualScoreCounterScene.Instantiate<IndividualScoreCounter>();
			counter.colorIdx = colorIdx;
			scoreCountersContainer.GetChild(i/3).AddChild(counter);
			scoreCounters.Add(counter);
		}
	}

	public void AddScoreToAll()
	{
		//TODO: fix add scrore to all
		//AddScoreToAll(Game.Instance.gamemode.teamScoreDiference);
	}
	public async void AddScoreToAll(Dictionary<int, int> scoreToAdd)
	{
		(GetTree().GetFirstNodeInGroup("OverWorld") as OverWorld).canPause = false;
		Tween tween = CreateTween();
		tween.TweenMethod(Callable.From((float tweenedValue)=>
		{
			foreach (IndividualScoreCounter counter in scoreCounters)
			{
			}
		
		}), 0f, 1f, 1f);
		// Show score animation node animation
		// ^ await animation finished ^
		// foreach score counter animate adding its score
		// ^ await finished signal ^
		// Hide score animation node animation
		// ^ await animation finished ^
		(GetTree().GetFirstNodeInGroup("OverWorld") as OverWorld).canPause = true;
		Game.Instance.CallDeferred(Game.MethodName.RestartRound);
	}

    public override void _ExitTree()
    {
		SignalBus.Instance.RoundFinished -= AddScoreToAll;
        base._ExitTree();
    }
}
