using Godot;
using static SpaceMages.SpaceMagesVars;
using System;
using System.Threading.Tasks;

public partial class RoundStartAnnouncer : Control
{
	[Export] Label readyGoLabel; 
	[Export] AudioStreamPlayer READYSound; 
	[Export] AudioStreamPlayer GOSound;
	bool isRoundStarting = false;
	
	Tween tween;
	World world;
	public override void _Ready()
	{
		world = GetTree().GetFirstNodeInGroup("World") as World;
		SignalBus.Instance.GameStarted += OnNewRound;
		SignalBus.Instance.NewRoundStart += OnNewRound;
		SignalBus.Instance.GameStarted += OnGameStart;
	}

	void OnGameStart()
	{
		Game.Instance.CallDeferred(Game.MethodName.PauseGame);
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	void OnNewRound()
	{
		isRoundStarting = true;
		AnimateReadyGo();	
	}

	async void AnimateReadyGo()
	{
		Game.PauseGame();
		readyGoLabel.Text = "READY!!";
		READYSound.Play();
		tween = GetEasedTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(readyGoLabel, "position", new Vector2(607.5f, 419.0f), 0.75f);
		await ToSignal(GetTree().CreateTimer(2f), "timeout");
		readyGoLabel.Text = "GO!!";
		GOSound.Play();
		Game.UnPauseGame();
		isRoundStarting = false;
		tween = GetEasedTween();
		tween.SetEase(Tween.EaseType.In);
		tween.TweenProperty(readyGoLabel, "position", new Vector2(607.5f, -300.0f), 0.5f);
	}

	Tween GetEasedTween()
	{
		Tween tween = tween = CreateTween();
		
		tween.SetTrans(Tween.TransitionType.Sine);

		return tween;
	}

	public override void _Draw()
	{
		if (!isRoundStarting) return;
		foreach (Player player in Game.Instance.players)
		{
			Camera2D camera = Game.GetMap().camera;
			Vector3 playerColor = teamColors[player.colorIdx];
			Vector2 endPos = player.Position + new Vector2(960.0f, 540.0f) - camera.Position;
			Vector2 startPos = new Vector2(960.0f, 540.0f);
			endPos -= (startPos - endPos) * camera.Zoom.X /	4;
			DrawLine(startPos, endPos, new Color(playerColor.X, playerColor.Y, playerColor.Z, 1), 5f, true);
			DrawPolyline([
				endPos + (new Vector2(-1, -1) * 15).Rotated((endPos - startPos).Angle()),
				endPos,
				endPos + (new Vector2(-1, 1) * 15).Rotated((endPos - startPos).Angle())
				], new Color(playerColor.X, playerColor.Y, playerColor.Z, 1), 5f, true);
		}
	}

	public override void _ExitTree()
	{
		SignalBus.Instance.GameStarted -= OnNewRound;
		SignalBus.Instance.NewRoundStart -= OnNewRound;
		base._ExitTree();
	}
}
