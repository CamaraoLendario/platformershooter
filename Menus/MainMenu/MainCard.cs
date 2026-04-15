using Godot;
using System;

public partial class MainCard : MainMenuScreen
{
    public void AnimateOut()
    {
        MainMenuAnimator.Play("MainCardPressedAnyKey");
    }
}
