using Godot;
using System.Collections.Generic;

public partial class MainMenuController : Node
{
    [Export] public AnimationPlayer[] skippableAnimationPlayers; 
	[Export] public MainCard mainCard;
	[Export] public Menu mainMenu;
	[Export] public GamemodeSettings gamemodeSettings;
	[Export] public MainMenuScreen currentScreen;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsReleased() || @event is InputEventMouse || @event is InputEventJoypadMotion) return;
        if (currentScreen is MainCard mainCard)
        {
            mainCard.AnimateOut();
            currentScreen = mainMenu;
            return;
        }
        
        foreach (AnimationPlayer animationPlayer in skippableAnimationPlayers)
        {
            if (animationPlayer.IsPlaying())
            {
                animationPlayer.Seek(animationPlayer.CurrentAnimationLength);
            }
        }

        if (Input.IsActionJustPressed("MenuUp") || Input.IsActionJustPressed("MenuUpKeyboard")){
            currentScreen.OnMoveAction(new Vector2(0, -1));
        }
        else if (Input.IsActionJustPressed("MenuDown") || Input.IsActionJustPressed("MenuDownKeyboard")){
            currentScreen.OnMoveAction(new Vector2(0, 1));
        }
        else if (Input.IsActionJustPressed("MenuLeft") || Input.IsActionJustPressed("MenuLeftKeyboard")){
            currentScreen.OnMoveAction(new Vector2(1, 0));
        }
        else if (Input.IsActionJustPressed("MenuRight") || Input.IsActionJustPressed("MenuRightKeyboard")){
            currentScreen.OnMoveAction(new Vector2(1, 0));
        }
    
        if (Input.IsActionJustPressed("MenuAccept") || Input.IsActionJustPressed("MenuAcceptKeyboard")){
            currentScreen.OnPositiveAction();
        }
        if (Input.IsActionJustPressed("MenuBack") || Input.IsActionJustPressed("MenuBackKeyboard")){
            currentScreen.OnNegativeAction();
        }
    }

}
