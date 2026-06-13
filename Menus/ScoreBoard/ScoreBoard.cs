using Godot;
using System.Collections.Generic;
using System.Linq;
using static SpaceMages.SpaceMagesVars;

public partial class ScoreBoard : Control
{
	MenuItemsListContainer scoreContiners;
	IndividualScoreCounter[] scoreCounters;
	Dictionary<int, IndividualScoreCounter> scoreCountersTeam = [];
	public override void _Ready()
	{
		scoreContiners = GetNode<MenuItemsListContainer>("%scoreContainers");
		SignalBus.Instance.StartGame += Initialize;
		SignalBus.Instance.RoundFinished += AddScoreToAll;
		SignalBus.Instance.GameExited += (MainMenuScreen toScreen) => {Reset();};
	}

	public void Initialize()
	{
		PlayerInfo[] playersInfo = Game.GetPlayersInfo();
		scoreCounters = new IndividualScoreCounter[playersInfo.Length];
		for (int i = 0 ; i < playersInfo.Length; i++)
		{
			if (scoreCountersTeam.Keys.Contains(-1)) return;
			IndividualScoreCounter counter = IndividualScoreCounter.New(playersInfo[i]);
			scoreCounters[i] = counter;
			scoreCountersTeam.Add(playersInfo[i].colorIdx, counter);
		}
		scoreContiners.MassAddChildren(scoreCounters);
		ScreenAnimateNodes(CreateTween(), scoreCounters, Vector2.Down, 0f, false, false, 0);
	}
	
	void Reset() {
		foreach(IndividualScoreCounter scoreCounter in scoreCounters) {
			scoreCounter.QueueFree();
		}
		scoreCounters = [];
		scoreCountersTeam = [];
	}

	public void AddScoreToAll()
	{
		AnimateAddScoreToAll(Game.GetGamemodeLogic().teamScoreChanges.ToList());
	}

	public async void AnimateAddScoreToAll(List<(int, int)> teamScoreChanges)
	{
		GD.Print("animating AddScoreToall");
		GamemodeLogic gamemodeLogic = Game.GetGamemodeLogic();
		Game.PauseGame();
		// Show score animation node animation
		Tween tween = CreateTween();
		ScreenAnimateNodes(tween, scoreCounters, Vector2.Down, 1f, true, false, 0);
		await ToSignal(tween, Tween.SignalName.Finished);
		// foreach score counter animate adding its score
		IndividualScoreCounter scoreCounter = null;
		
		foreach((int team, int points) in teamScoreChanges) {
			await ToSignal(GetTree().CreateTimer(0.1f), Timer.SignalName.Timeout);
			scoreCounter = scoreCountersTeam[team];
			scoreCounter.AddScore(points);
		}
		// await ^finished^ signal
		if (scoreCounter == null) {
			GD.PrintErr("no counter found");
			await ToSignal(GetTree().CreateTimer(0.5f + 0.2f), Timer.SignalName.Timeout);
		}
		else await ToSignal(GetTree().CreateTimer(scoreCounter.animationTime + 0.2f), Timer.SignalName.Timeout);
		// Hide score animation node animation
		tween = CreateTween(); 	
		ScreenAnimateNodes(tween, scoreCounters, Vector2.Down, 1f, false, false, 0);
		bool isGameOver = gamemodeLogic.IsGameOver();
		if (isGameOver)
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameFinished, gamemodeLogic.GetWinningTeam());
		else
		{
			// await ^animation^ finished
			await ToSignal(tween, Tween.SignalName.Finished);
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.NewRoundStart);
			//Game.UnPauseGame();
		}
		GD.Print("added score to all");
	}

    public override void _ExitTree()
    {
		SignalBus.Instance.StartGame -= Initialize;
		SignalBus.Instance.RoundFinished -= AddScoreToAll;
        base._ExitTree();
    }
}
