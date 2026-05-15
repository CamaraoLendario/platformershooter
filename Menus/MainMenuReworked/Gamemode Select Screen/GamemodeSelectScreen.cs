using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class GamemodeSelectScreen : MainMenuScreen
{
    [Export] GamemodeSelectScroll gamemodescroll;
    //TODO: make a visual representation of press start to continue
    public override bool OnAccept()
    {
        gamemodescroll.SetGamemode();
        Move(Vector2.Left);
        CharacterSelectScreen characterSelectScreen = GetParent().GetNode<CharacterSelectScreen>("NewCharacterSelectScreen");
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
