using Godot;
using SpaceMages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

public partial class ScoreBoard : Control
{
	public Callable HideScoreButton => Callable.From(HideScore);
	[Export] PackedScene individualScoreCounterScene;
	[Export] HBoxContainer scoreCountersContainer;
	Dictionary<int, int> oldTeamsScore = [];
	List<IndividualScoreCounter> scoreCounters = [];

	public override void _Ready()
	{
		Game.Instance.scoreBoard = this;
		Game.Instance.GameStarted += OnNewGame;
		Game.Instance.RoundFinished += AddScoreToAll;
	}

	private void OnNewGame()
	{
		oldTeamsScore = Game.Instance.gameMode.teamsScore;
	}

	public void Initialize(Dictionary<int, Player> players)
	{
		foreach (int colorIdx in players.Keys)
		{
			IndividualScoreCounter counter = individualScoreCounterScene.Instantiate<IndividualScoreCounter>();
			counter.colorIdx = colorIdx;
			scoreCountersContainer.AddChild(counter);
			scoreCounters.Add(counter);
		}
	}

	public void ShowScore()
	{
		Tween tween = CreateShowHideTween();
		tween.SetEase(Tween.EaseType.Out);

		tween.TweenMethod(Callable.From((float Ypos) => SetPosition(Ypos)), 1000, 0, 1.0f);
	}

	private void HideScore()
	{
		Tween tween = CreateShowHideTween();
		tween.SetEase(Tween.EaseType.In);

		tween.TweenMethod(Callable.From((float Ypos) => SetPosition(Ypos)), 0, 1000, 1.0f);
	}

	public void AddScoreToAll()
	{
		GameMode gameMode = Game.Instance.gameMode;
		
		AddScoreToAll(gameMode.addedTeamsScore);
	}
	public async void AddScoreToAll(Dictionary<int, int> scoreToAdd)
	{
		(GetTree().GetFirstNodeInGroup("OverWorld") as OverWorld).canPause = false;
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		ShowScore();
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		foreach (IndividualScoreCounter counter in GetChild(0).GetChildren())
		{
			if (!scoreToAdd.ContainsKey(counter.colorIdx)) continue;
			counter.AddScore(scoreToAdd[counter.colorIdx]);
			await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		}
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		Game.Instance.gameMode.ResetAddedTeamsScore();
		HideScore();
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		(GetTree().GetFirstNodeInGroup("OverWorld") as OverWorld).canPause = true;
		Game.Instance.CallDeferred(Game.MethodName.RestartRound);
	}

	void SetPosition(float Y)
	{
		Position = Vector2.Down * Y;
	}

	Tween CreateShowHideTween()
	{
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);

		return tween;
	}

    public override void _ExitTree()
    {
		Game.Instance.GameStarted -= OnNewGame;
		Game.Instance.RoundFinished -= AddScoreToAll;
        base._ExitTree();
    }

}
