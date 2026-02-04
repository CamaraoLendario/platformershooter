using Godot;
using System;
/* 
public partial class InputReceiver : Node
{
	public static InputReceiver Instance { get; private set; }

	[Signal] public delegate void InputDirChangedEventHandler(int inputID, float X, float Y);
	[Signal] public delegate void ShootStartEventHandler(int inputID);
	[Signal] public delegate void ShootEndEventHandler(int inputID);
	[Signal] public delegate void JumpStartEventHandler(int inputID);
	[Signal] public delegate void JumpEndEventHandler(int inputID);
	[Signal] public delegate void AimStartEventHandler(int inputID);
	[Signal] public delegate void AimEndEventHandler(int inputID);
	[Signal] public delegate void MeleeStartEventHandler(int inputID);
	[Signal] public delegate void MeleeEndEventHandler(int inputID);

	public override void _Ready()
	{
		if (Instance == null)
		Instance = this;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion) return;
		int inputIndex = @event.Device;
		if (@event is InputEventJoypadButton || @event is InputEventJoypadMotion)
		{
			inputIndex += 1;
		}
		else inputIndex = 0;

		float X;
		float Y;

		if (inputIndex == 0)
		{
			X = Input.GetAxis("Left", "Right");
			Y = Input.GetAxis("Up", "Down");
		}
		else
		{
			X = Input.GetJoyAxis(@event.Device, JoyAxis.LeftX);
			Y = Input.GetJoyAxis(@event.Device, JoyAxis.LeftY);
		}

		EmitSignal(SignalName.InputDirChanged, inputIndex, X, Y);

		if (Input.IsActionJustPressed("Shoot"))
		{
			EmitSignal(SignalName.ShootStart, inputIndex);
			return;
		}

		if (Input.IsActionJustReleased("Shoot"))
		{
			EmitSignal(SignalName.ShootEnd, inputIndex);
			return;
		}

		if (Input.IsActionJustPressed("Jump"))
		{
			EmitSignal(SignalName.JumpStart, inputIndex);
			return;
		}

		if (Input.IsActionJustReleased("Jump"))
		{
			EmitSignal(SignalName.JumpEnd, inputIndex);
			return;
		}

		if (Input.IsActionJustPressed("Aim"))
		{
			EmitSignal(SignalName.AimStart, inputIndex);
			return;
		}

		if (Input.IsActionJustReleased("Aim"))
		{
			EmitSignal(SignalName.AimEnd, inputIndex);
			return;
		}
		
		if (Input.IsActionJustPressed("Melee"))
		{
			EmitSignal(SignalName.MeleeStart, inputIndex);
			return;
		}
		
		if (Input.IsActionJustReleased("Melee"))
		{
			EmitSignal(SignalName.MeleeEnd, inputIndex);
			return;
		}
	}
} */