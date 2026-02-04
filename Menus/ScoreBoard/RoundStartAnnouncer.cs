using Godot;
using SpaceMages;
using System;
using System.Threading.Tasks;

public partial class RoundStartAnnouncer : Control
{
	[Export] Label readyGoLabel; 
	[Export] AudioStreamPlayer READY; 
	[Export] AudioStreamPlayer GO;
	bool isRoundStart = false;
	
	Tween tween;
	public override void _Ready()
	{
		Game.Instance.GameStarted += OnNewRound;
		Game.Instance.NewRoundStarted += OnNewRound;
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	async void OnNewRound()
	{
		Game.Instance.CallDeferred(Game.MethodName.PauseGame);
		isRoundStart = true;
		AnimateReadyGo();	
	}

	async void AnimateReadyGo()
	{
		readyGoLabel.Text = "READY!!";
		READY.Play();
		tween = GetEasedTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(readyGoLabel, "position", new Vector2(607.5f, 419.0f), 0.75f);
		await ToSignal(GetTree().CreateTimer(2f), "timeout");
		readyGoLabel.Text = "GO!!";
		GO.Play();
		Game.Instance.UnPauseGame();
		isRoundStart = false;
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
		if (!isRoundStart) return;
		foreach (Player player in Game.Instance.playerNodesByColor.Values)
		{
			Camera2D camera = Game.Instance.world.currentMap.camera;
			Vector3 playerColor = SpaceMagesVars.teamColors[player.colorIdx];
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
		Game.Instance.GameStarted -= OnNewRound;
		Game.Instance.NewRoundStarted -= OnNewRound;
		base._ExitTree();
	}
}
