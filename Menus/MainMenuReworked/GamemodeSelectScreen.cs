using Godot;
using System;

[Tool]
public partial class GamemodeSelectScreen : MainMenuScreen
{
    public override bool OnAccept()
    {
        Move(Vector2.Left);
        NewCharacterSelectScreen characterSelectScreen = GetParent().GetNode<NewCharacterSelectScreen>("NewCharacterSelectScreen");
        characterSelectScreen.Move(Vector2.Right, true);
        GetMenuController().currentScreen = characterSelectScreen;
        return true;
    }

    public override void Back()
    {
        Move(Vector2.Right);
		MainMenuMainScreen mainMenuMain = GetParent().GetNode<MainMenuMainScreen>("MainMenuMainScreen");
		mainMenuMain.Move(Vector2.Left, true);
		GetNode<MainMenuController>("%MainMenuController").currentScreen = mainMenuMain;
    }

}
