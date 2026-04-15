using Godot;
using System;

public partial class SettingsMenu : MainMenuScreen
{
	
	int currentButtonIdx = 0;

	public override void OnMoveAction(Vector2 dir)
    {
		currentButtonIdx = menuSelectPanel.Move(dir);
    }

    public override void OnNegativeAction()
	{
        HideMenu();
        (GetNode("%Menu") as Menu).ShowRight();
	}
}
