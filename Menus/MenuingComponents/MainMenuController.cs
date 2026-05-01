using Godot;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;

[Tool]
public partial class MainMenuController : Node
{
	[Export] public MainMenuScreen currentScreen; // Mannualy set this export to the first screen that's supposed to be selected. Otherwise the first one found will be selected

	Vector2 inputVec = Vector2.Zero;
	string[] dirInputsController = [
		"MenuLeft", "MenuRight", "MenuUp", "MenuDown"];
	string[] dirInputsKeyboard = [
		"MenuLeftKeyboard", "MenuRightKeyboard", "MenuUpKeyboard", "MenuDownKeyboard"];
	
	Timer inputSpammerDelay = new Timer()
	{
		OneShot = true
	};
	bool keepSpammingInputs = false;

	public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;	
		foreach (Node node in GetParent().GetChildren())
		{
			if (node is not MainMenuScreen mainMenuScreen) continue;
			mainMenuScreen.Position *= 0;
			if (mainMenuScreen != currentScreen)
				mainMenuScreen.CallDeferred(MainMenuScreen.MethodName.Move, Vector2.Right, false, false, true);
		}
		AddChild(inputSpammerDelay);
		inputSpammerDelay.Timeout += OnSpammerDelayTimeout;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouse || @event is InputEventJoypadMotion) return;

		if (currentScreen is MainThemeScreen mainThemeScreen && !@event.IsReleased())
		{
			mainThemeScreen.OnInteract();
			return;
		}

		Vector2 newInputVec = inputVec;
		foreach(string inputName in dirInputsController)
		{
			if (Input.IsActionJustPressed(inputName) || Input.IsActionJustReleased(inputName))
			{
				newInputVec = Input.GetVector("MenuLeft", "MenuRight", "MenuUp", "MenuDown");
				break;
			}
		}

		foreach(string inputName in dirInputsKeyboard)
		{
			if (Input.IsActionJustPressed(inputName) || Input.IsActionJustReleased(inputName))
			{
				newInputVec = Input.GetVector("MenuLeftKeyboard", "MenuRightKeyboard", "MenuUpKeyboard", "MenuDownKeyboard");
				break;
			}
		}
		
		UpdateInputVec(newInputVec);

		if (Input.IsActionJustPressed("MenuInteract") || Input.IsActionJustPressed("MenuInteractKeyboard")){
			if(currentScreen.OnInteract())
				GetViewport().SetInputAsHandled();
			return;
		}
		if (Input.IsActionJustPressed("MenuAltInteract") || Input.IsActionJustPressed("MenuAltInteractKeyboard")){
			if(currentScreen.OnAltInteract())
				GetViewport().SetInputAsHandled();
			return;
		}
		if (Input.IsActionJustPressed("MenuAccept") || Input.IsActionJustPressed("MenuAcceptKeyboard")){
			if(currentScreen.OnAccept())
				GetViewport().SetInputAsHandled();
			return;
		}
		if (Input.IsActionJustPressed("MenuBack") || Input.IsActionJustPressed("MenuBackKeyboard")){
			if(currentScreen.OnNegativeAction())
				GetViewport().SetInputAsHandled();
			return;
		}
	}

	void UpdateInputVec(Vector2 newinputVec)
	{
		if (inputVec == newinputVec) return;
		inputVec = newinputVec;

		inputSpammerDelay.Stop();
		keepSpammingInputs = false;
		if (inputVec.X == 0 && inputVec.Y == 0)
			return;
		inputSpammerDelay.Start(0.6);

		currentScreen.OnMoveAction(inputVec);
	}
	void OnSpammerDelayTimeout()
	{
		keepSpammingInputs = true;
		StartInputSpam();
	}

	async void StartInputSpam()
	{
		if (!keepSpammingInputs) return;
		currentScreen.OnMoveAction(inputVec);
		await ToSignal(GetTree().CreateTimer(0.2f), Timer.SignalName.Timeout);
		StartInputSpam();
	}
}
