using Godot;
using static SpaceMages.SpaceMagesVars;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;

[Tool]
public partial class MainMenuController : MenuController
{
	[Export] public MainMenuScreen currentScreen;

	public override void _Ready()
	{
		base._Ready();
		if (Engine.IsEditorHint()) return;
		foreach (Node node in GetParent().GetChildren())
		{
			if (node is not MainMenuScreen mainMenuScreen) continue;
			mainMenuScreen.Position *= 0;
			if (mainMenuScreen != currentScreen)
				mainMenuScreen.CallDeferred(MainMenuScreen.MethodName.Move, Vector2.Right, false, false, true);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse) return;

		if (currentScreen is MainMenuMainScreen mainMenuMainScreen && @event is not InputEventJoypadMotion && !@event.IsReleased())
		{
			if (mainMenuMainScreen.LeaveThemeCard())
				return;
		}

		Vector2 newInputVec = inputVec;
		if (@event is InputEventJoypadMotion motion)
		{
			UpdateInputVec(GetMenuDirFromJoyStick(motion));
			GetViewport().SetInputAsHandled();
			return;
		}

		foreach(string inputName in menuDirs)
		{
			if (Input.IsActionJustPressed(inputName + "Keyboard") || Input.IsActionJustReleased(inputName + "Keyboard"))
			{
				newInputVec = GetInputVectorNotNormalized(
				menuDirs[0] + "Keyboard", 
				menuDirs[1] + "Keyboard", 
				menuDirs[2] + "Keyboard", 
				menuDirs[3] + "Keyboard");
				break;
			}
			if (Input.IsActionJustPressed(inputName) || Input.IsActionJustReleased(inputName))
			{
				newInputVec = GetInputVectorNotNormalized(
				menuDirs[0], 
				menuDirs[1], 
				menuDirs[2], 
				menuDirs[3]);
				break;
			}
		}
		GD.Print(newInputVec);		
		UpdateInputVec(newInputVec);

		if ((Input.IsActionJustPressed("MenuInteract") || Input.IsActionJustPressed("MenuInteractKeyboard")) && currentScreen.OnInteract()){
			GetViewport().SetInputAsHandled();
			return;}
		if ((Input.IsActionJustPressed("MenuAltInteract") || Input.IsActionJustPressed("MenuAltInteractKeyboard")) && currentScreen.OnAltInteract()){
			GetViewport().SetInputAsHandled();
			return;}
		if ((Input.IsActionJustPressed("MenuAccept") || Input.IsActionJustPressed("MenuAcceptKeyboard")) && currentScreen.OnAccept()){
			GetViewport().SetInputAsHandled();
			return;}
		if ((Input.IsActionJustPressed("MenuBack") || Input.IsActionJustPressed("MenuBackKeyboard")) && currentScreen.OnNegativeAction()){
			GetViewport().SetInputAsHandled();
			return;}
	}

    public override void OnMoveAction(Vector2 inputvec)
	{
		currentScreen.OnMoveAction(inputvec);
	}

}	