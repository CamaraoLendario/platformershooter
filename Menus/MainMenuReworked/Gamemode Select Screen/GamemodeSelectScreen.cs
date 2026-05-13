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
        ChangeScreen(
            GetParent().GetNode<MainMenuMainScreen>("MainMenuMainScreen"),
            Vector2.Right, Vector2.Left);
    }

}
