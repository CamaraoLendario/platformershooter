using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    public override void _Ready()
    {
        Game.Instance.mainMenu = this;
    }
}
