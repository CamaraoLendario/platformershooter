using Godot;
using System;
using System.Linq;
using System.Transactions;

public partial class WinningScoreSelector : Button
{
	(string, int)[] gameLengths = [
		("Short (6)", 6),
		("Medium (12)", 12),
		("Long (18)", 18),
		("debug (1)", 1),
	];
	int CurrentIdx = 1;
	public override void _Ready()
	{
		CurrentIdx = Game.Instance.chosenWinningScoreIdx;
		ProcessIdx();
		Pressed += OnPressed();
	}

	private void OnPressedHelper()
	{
		CurrentIdx += 1;
		if (CurrentIdx > gameLengths.Count() - 1)
		{
			CurrentIdx = 0;
		}

		ProcessIdx();
	}

	private Action OnPressed()
	{
		return OnPressedHelper;
	}

	void ProcessIdx()
    {
		Text = gameLengths[CurrentIdx].Item1;
		Game.Instance.chosenWinningScore = gameLengths[CurrentIdx].Item2;  
    }
}