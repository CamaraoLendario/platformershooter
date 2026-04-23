using Godot;
using System;

public partial class GamemodeSettings : MainMenuScreen
{
	[Export] MenuScroll gamemodeScroll;
	Control gamemodeScrollPanelHelper;
	[Export] Label gameLength;
	[Export] Label Handicap;
	bool isHandicapEnabled = false;

	public override void _Ready()
	{
		base._Ready();
		gamemodeScrollPanelHelper = gamemodeScroll.GetPanelHelper();    
	}
}
