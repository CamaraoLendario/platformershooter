using Godot;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;

public partial class PlayerInput : Node
{
	public static PlayerInput Instance { get; private set; }
#region Signals
	[Signal] public delegate void InputDirChangedEventHandler(float X, float Y);
	[Signal] public delegate void ShootStartEventHandler();
	[Signal] public delegate void ShootEndEventHandler();
	[Signal] public delegate void JumpStartEventHandler();
	[Signal] public delegate void JumpEndEventHandler();
	[Signal] public delegate void AimStartEventHandler();
	[Signal] public delegate void AimEndEventHandler();
	[Signal] public delegate void MeleeStartEventHandler();
	[Signal] public delegate void MeleeEndEventHandler();
	[Signal] public delegate void DropStartEventHandler();
	[Signal] public delegate void DropEndEventHandler();
	[Signal] public delegate void GrapplingHookStartEventHandler();
	[Signal] public delegate void GrapplingHookEndEventHandler(); 
	[Signal] public delegate void SpecialStartEventHandler(); 
	[Signal] public delegate void SpecialEndEventHandler(); 
	[Signal] public delegate void PauseRequestEventHandler(Player player, bool pausedByDisconnect);
#endregion
	protected Player Main = null;
	public string keyboardKeyword = "";
	List<string> inputs = new List<string>()
	{
		"Shoot",
		"Jump",
		"Aim",
		"Melee",
		"Drop",
		"Special"
	};
	
	public override void _Ready()
	{
		/* if (Instance == null)
			Instance = this; */
		if (Main == null){
			Main = GetParentOrNull<Player>();
			GD.Print(Main);
		}
		if (Main.isKeyboardControlled) keyboardKeyword = "Keyboard";
	}

	public override void _Input(InputEvent @event)
	{
		if (!GodotObject.IsInstanceValid(Main))
    		return;
		if (@event is InputEventMouseMotion) return;
		int inputIndex = Main.inputIdx;
		
		float X, Y;

		if (Main.isKeyboardControlled)
		{
			inputIndex = -1;
			X = Input.GetAxis("LeftKeyboard", "RightKeyboard");
			Y = Input.GetAxis("UpKeyboard", "DownKeyboard");
			EmitSignal(SignalName.InputDirChanged, X, Y);

		}
		else
		{
			X = Input.GetJoyAxis(Main.inputIdx, JoyAxis.LeftX);
			Y = Input.GetJoyAxis(Main.inputIdx, JoyAxis.LeftY);
			EmitSignal(SignalName.InputDirChanged, X, Y);
		}

		foreach(string input in inputs)
		{
			if (Input.IsActionJustPressed(input + keyboardKeyword + inputIndex))
			{
				GD.Print("Detected Input: ", input + "Start");
				EmitSignal(input + "Start");
			}
			
			if (Input.IsActionJustReleased(input + keyboardKeyword + inputIndex))
			{
				GD.Print("Detected Input: ", input + "End");
				EmitSignal(input + "End");
			}
		}
#region ZombieCodeLookLater

		/* if (Input.IsActionJustPressed("Shoot" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.ShootStart);
		}

		if (Input.IsActionJustReleased("Shoot" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.ShootEnd);
		}

		if (Input.IsActionJustPressed("Jump" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.JumpStart);
		}

		if (Input.IsActionJustReleased("Jump" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.JumpEnd);
		}

		if (Input.IsActionJustPressed("Aim" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.AimStart);
		}

		if (!Input.IsActionPressed("Aim" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.AimEnd);
		}

		if (Input.IsActionJustPressed("Melee" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.MeleeStart);
		}

		if (Input.IsActionJustReleased("Melee" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.MeleeEnd);
		}

		if (Input.IsActionJustPressed("Drop" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.DropStart);
		}

		if (Input.IsActionJustReleased("Drop" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.DropEnd);
		}

		// if (Input.IsActionJustPressed("GrapplingHook" + keyboardKeyword + inputIndex))
		// {
		// 	EmitSignal(SignalName.GrapplingHookStart);
		// }

		// if (Input.IsActionJustReleased("GrapplingHook" + keyboardKeyword + inputIndex))
		// {
		// 	EmitSignal(SignalName.GrapplingHookEnd);
		// }
	
		if (Input.IsActionJustPressed("Special" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.SpecialStart, Main, false);
		}
		
		if (Input.IsActionJustReleased("Special" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.SpecialEnd, Main, false);
		}
 */
#endregion
		if (Input.IsActionJustPressed("Pause" + keyboardKeyword + inputIndex))
		{
			EmitSignal(SignalName.PauseRequest, Main, false);
		}
	}
}