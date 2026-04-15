using Godot;
using System;

public partial class ScoreHUD : CanvasLayer
{
	[Export] public ScoreBoard scoreBoard;

    public override void _Ready()
    {
        SignalBus.Instance.GameStarted += ConnectSignals;
    }

    void ConnectSignals()
    {
        SignalBus.Instance.GameFinished += AnnounceWinner;
    }

	void AnnounceWinner(int colorIdx)
    {
        GetNode<AndTheWinnerIs>("AndTheWinnerIs").AnnounceWinner(colorIdx);
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.GameStarted -= ConnectSignals;
        SignalBus.Instance.GameFinished -= AnnounceWinner;
        base._ExitTree();
    }

}
