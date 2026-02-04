using Godot;
using System;

public partial class ScoreHUD : CanvasLayer
{
	[Export] public ScoreBoard scoreBoard;

    public override void _Ready()
    {
        Game.Instance.GameStarted += ConnectSignals;
    }

    void ConnectSignals()
    {
        Game.Instance.gameMode.WinnerDecided += AnnounceWinner;
    }

	void AnnounceWinner(int colorIdx)
    {
        GetNode<AndTheWinnerIs>("AndTheWinnerIs").AnnounceWinner(colorIdx);
    }

    public override void _ExitTree()
    {
        Game.Instance.GameStarted -= ConnectSignals;
        Game.Instance.gameMode.WinnerDecided -= AnnounceWinner;
        base._ExitTree();
    }

}
