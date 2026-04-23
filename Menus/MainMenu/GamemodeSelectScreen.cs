using Godot;
using System;

[Tool]
public partial class GamemodeSelectScreen : MainMenuScreen
{
    public override void Back()
    {
        Move(Vector2.Right);
		MainMenuMainScreen mainMenuMain = GetParent().GetNode<MainMenuMainScreen>("MainMenuMainScreen");
		mainMenuMain.Move(Vector2.Left, true);
		GetNode<MainMenuController>("%MainMenuController").currentScreen = mainMenuMain;
    }

}
