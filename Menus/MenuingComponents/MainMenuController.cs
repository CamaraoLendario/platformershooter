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
	MainMenu main;
	bool enabled = true;
	
	(string controllerInput, string keyboardInput, StringName menuAction)[] menuInputs = [
		("MenuInteract", "MenuInteractKeyboard", MainMenuScreen.MethodName.OnInteract),
		("MenuAltInteract", "MenuAltInteractKeyboard", MainMenuScreen.MethodName.OnAltInteract),
		("MenuAccept", "MenuAcceptKeyboard", MainMenuScreen.MethodName.OnAccept),
		("MenuBack", "MenuBackKeyboard", MainMenuScreen.MethodName.OnNegativeAction),
	]; 

	public override void _Ready()
	{
		base._Ready();
		main = GetParent<MainMenu>();
		SignalBus.Instance.StartGame += () => {enabled = false;};
		SignalBus.Instance.GameExited += (MainMenuScreen exitedTo) => {enabled = true;};
	}

	public override void _Input(InputEvent @event)
	{
		if (!enabled || @event is InputEventMouse) return;

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
				menuDirs[(int)dirKeyMenu.Left] + "Keyboard", 
				menuDirs[(int)dirKeyMenu.Right] + "Keyboard", 
				menuDirs[(int)dirKeyMenu.Up] + "Keyboard", 
				menuDirs[(int)dirKeyMenu.Down] + "Keyboard");
				break;
			}
			if (Input.IsActionJustPressed(inputName) || Input.IsActionJustReleased(inputName))
			{
				newInputVec = GetInputVectorNotNormalized(
				menuDirs[(int)dirKeyMenu.Left], 
				menuDirs[(int)dirKeyMenu.Right], 
				menuDirs[(int)dirKeyMenu.Up], 
				menuDirs[(int)dirKeyMenu.Down]);
				break;
			}
		}
		
		UpdateInputVec(newInputVec);
		
		if (!@event.IsReleased())
			foreach((string controllerInput, string keyboardInput, StringName menuAction) in menuInputs)
			{
				if ((Input.IsActionJustPressed(controllerInput) || Input.IsActionJustPressed(keyboardInput)) && (bool)currentScreen.Call(menuAction))
				{
					GetViewport().SetInputAsHandled();
					return;
				}
			}
		else
			foreach((string controllerInput, string keyboardInput, StringName menuAction) in menuInputs)
			{
				if ((Input.IsActionJustReleased(controllerInput) || Input.IsActionJustReleased(keyboardInput)) && (bool)currentScreen.Call(menuAction + "Released"))
				{
					GetViewport().SetInputAsHandled();
					return;
				}
			}
	}

    public override void OnMoveAction(Vector2 inputvec)
	{
		currentScreen.OnMoveAction(inputvec);
	}

	public MainMenuScreen SetCurrentScreen(MainMenu.Screens screen)
	{
		currentScreen.Move(Vector2.Down, false, false, true);
		currentScreen = main.GetScreen(screen);
		currentScreen.Move(Vector2.Down, true, false, true);
		if (currentScreen == null) {
			GD.PrintErr("screen not found. defaulting to MainMenuScreen");
			currentScreen = GetNodeOrNull<MainMenuScreen>("MainMenuMainScreen");
		}
		return currentScreen;
	}

 
}	