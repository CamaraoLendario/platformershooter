using Godot;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class PlayerMenuInput : MenuController
{
	[Signal] public delegate bool MoveEventHandler(Vector2 vec);
	[Signal] public delegate bool InteractEventHandler();
	[Signal] public delegate bool AltInteractEventHandler();
	[Signal] public delegate bool NegativeActionEventHandler();
	[Signal] public delegate bool InteractReleasedEventHandler();
	[Signal] public delegate bool AltInteractReleasedEventHandler();
	[Signal] public delegate bool NegativeActionReleasedEventHandler();
	public bool isEnabled = false;
	public bool inputEnabled = true;
	int inputIdx = -2;
	string keyboardKeyword = "";
	(string inputName, StringName menuAction)[] playerCapsuleInputs = [
		("MenuInteract", SignalName.Interact),
		("MenuAltInteract", SignalName.AltInteract),
		("MenuBack", SignalName.NegativeAction),
	]; 

	public override void _UnhandledInput(InputEvent @event)
	{
		if (inputIdx < -1 ||
			@event is InputEventMouseMotion ||
			!isEnabled ||
			!inputEnabled) 
			return;
		if (@event is InputEventJoypadMotion motion){
			UpdateInputVec(GetMenuDirFromJoyStick(motion));
			//GetViewport().SetInputAsHandled();
			return;
		}
		string inputSufix = keyboardKeyword + inputIdx;
		foreach(string inputName in menuDirs)
		{
			if (Input.IsActionJustPressed(inputName + inputSufix) || Input.IsActionJustReleased(inputName + inputSufix)){
				//GD.Print(inputName + inputSufix);
				UpdateInputVec(GetInputVectorNotNormalized(
				menuDirs[(int)dirKeyMenu.Left] + inputSufix, 
				menuDirs[(int)dirKeyMenu.Right] + inputSufix, 
				menuDirs[(int)dirKeyMenu.Up] + inputSufix, 
				menuDirs[(int)dirKeyMenu.Down] + inputSufix));
				//GetViewport().SetInputAsHandled();
				return;
			}
		}
		
		foreach((string inputName, StringName menuAction) in playerCapsuleInputs) {
			if (Input.IsActionJustPressed(inputName + inputSufix)) {
				EmitSignal(menuAction);
				//GetViewport().SetInputAsHandled();
				return;
			}
		}

		foreach((string inputName, StringName menuAction) in playerCapsuleInputs) {
			if (Input.IsActionJustReleased(inputName + inputSufix)) {
				EmitSignal(menuAction + "Released");
				//GetViewport().SetInputAsHandled();
				return;
			}
		}

/* 		if (Input.IsActionJustPressed("MenuInteract" + inputSufix)){
			//GD.Print("MenuInteract" + inputSufix);
			EmitSignal(SignalName.Interact);
			GetViewport().SetInputAsHandled();
			return;
		}
		if (Input.IsActionJustPressed("MenuAltInteract" + inputSufix)){
			//GD.Print("MenuAltInteract" + inputSufix);
			EmitSignal(SignalName.AltInteract);
			GetViewport().SetInputAsHandled();
			return;
		}
		if (Input.IsActionJustPressed("MenuBack" + inputSufix)){
			//GD.Print("MenuBack" + inputSufix);
			EmitSignal(SignalName.NegativeAction);
			GetViewport().SetInputAsHandled();
			return;
		} */
	}

	public int GetInputIdx()
	{
		return inputIdx;
	}
	public void SetInputIdx(int newIdx)
	{
		if (newIdx < -1 && !Engine.IsEditorHint())
			InputGenerator.Instance.RemovePlayerMenuInput(inputIdx);
		
		inputIdx = newIdx;

		if (inputIdx == -1)
			keyboardKeyword = "Keyboard";
		else
			keyboardKeyword = "";
		
	}
	public override void OnMoveAction(Vector2 inputvec)
	{
		EmitSignal(SignalName.Move, inputvec);
	}

    public override void UpdateInputVec(Vector2 newinputVec)
    {
        base.UpdateInputVec(newinputVec);
    }
}
