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

    public override void _Ready()
    {
        base._Ready();
        GamemodeLogic gamemodeLogic = Game.GetGamemodeLogic();
        if (gamemodeLogic != null)
        {
            gamemodeScroll.SetIdx((int) gamemodeLogic.INDEX);
            gameLengthScroll.SetIdx((int) gamemodeLogic.CurrentGameLength);
            SetSettings();
        }
    }
   
    public override bool OnAccept()
    {
        SetSettings();
        
        Move(Vector2.Left);
        CharacterSelectScreen characterSelectScreen = GetParent().GetNode<CharacterSelectScreen>("CharacterSelectScreen");
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

    void SetSettings()
    {
        gamemodeScroll.SetGamemode();
        gameLengthScroll.SetGamemodeLength();
    }
}
