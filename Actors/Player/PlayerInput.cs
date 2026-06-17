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
#endregion
	protected Player Main = null;
	public int inputIdx = -2;
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
		}
		if (Main.GetInputIdx() == -1) keyboardKeyword = "Keyboard";
	}

	public override void _Input(InputEvent @event)
	{
		if (!GodotObject.IsInstanceValid(Main))
    		return;
		if (@event is InputEventMouseMotion) return;
		
		float X, Y;

		if (Main.GetInputIdx() == -1)
		{
			inputIdx = -1;
			X = Input.GetAxis("LeftKeyboard", "RightKeyboard");
			Y = Input.GetAxis("UpKeyboard", "DownKeyboard");
			EmitSignal(SignalName.InputDirChanged, X, Y);

		}
		else
		{
			Vector2 DpadVec = Input.GetVector("LeftDpad" + inputIdx, "RightDpad" + inputIdx, "UpDpad" + inputIdx, "DownDpad" + inputIdx);
			if (Mathf.Abs(DpadVec.X) > 0.1)
				X = DpadVec.X;
			else
				X = Input.GetJoyAxis(inputIdx, JoyAxis.LeftX);
			if (Mathf.Abs(DpadVec.Y) > 0.1)
				Y = DpadVec.Y;
			else
				Y = Input.GetJoyAxis(inputIdx, JoyAxis.LeftY);
			EmitSignal(SignalName.InputDirChanged, X, Y);
		}

		foreach(string input in inputs)
		{
			if (Input.IsActionJustPressed(input + keyboardKeyword + inputIdx))
			{
				EmitSignal(input + "Start");
			}
			
			if (Input.IsActionJustReleased(input + keyboardKeyword + inputIdx))
			{
				EmitSignal(input + "End");
			}
		}

		if (Input.IsActionJustPressed("Pause" + keyboardKeyword + inputIdx))
		{
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.PauseRequest, Main, false);
		}
	}
}