using Godot;
using System;

public partial class SettingsMenu : MainMenuScreen
{
	[Export] Control videoSettingsOption;
	int currentButtonIdx = 0;

	public override void OnMoveAction(Vector2 dir)
    {
		  currentButtonIdx = menuSelectPanel.Move(dir);
    }
}
