using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
public partial class GODMODEPlayerInput : PlayerInput
{
	#region Signals
	[Signal] public delegate void DEBUGMENUStartEventHandler();
	[Signal] public delegate void DEBUGMENUEndEventHandler();
	[Signal] public delegate void NOCLIPStartEventHandler();
	[Signal] public delegate void NOCLIPEndEventHandler();
	[Signal] public delegate void CHANGESHIELDSTATEStartEventHandler();
	[Signal] public delegate void CHANGESHIELDSTATEEndEventHandler();
	[Signal] public delegate void CHANGEPILOTSHIPSTATEStartEventHandler();
	[Signal] public delegate void CHANGEPILOTSHIPSTATEEndEventHandler();		
	#endregion

	List<string> debugInputs = new List<string>()
	{
		"DEBUGMENU",
		"NOCLIP",
		"CHANGESHIELDSTATE",
		"CHANGEPILOTSHIPSTATE",
	};
    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseMotion) return;
        base._Input(@event);

		foreach(string input in debugInputs)
		{
			int inputIndex = Main.inputIdx;
			if (Input.IsActionJustPressed(input + keyboardKeyword + inputIndex))
			{
				EmitSignal(input + "Start");
			}
			
			if (Input.IsActionJustReleased(input + keyboardKeyword + inputIndex))
			{
				EmitSignal(input + "End");
			}
		}
    }
}
