using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

[Tool]
public partial class GamemodeSelectScreen : MainMenuScreen
{
    [Export] GamemodeSelectScroll gamemodeScroll;
    [Export] GameLengthScroll gameLengthScroll;
    [Export] string[] lengths = Enum.GetNames(typeof(GamemodeLogic.GameLength));
    //TODO: make a visual representation of press start to continue
    public override bool OnAccept()
    {
        gamemodeScroll.SetGamemode();
        gameLengthScroll.SetGamemodeLength();
        Game.GetGamemodeLogic().CurrentGameLength = GamemodeLogic.GameLength.Short;

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
