using Godot;
using GodotPlugins.Game;
using System;

public partial class InGameMenuInput : Node
{
	[Signal] public delegate void InGameMenuWASDEventHandler(float x, float y);
	[Signal] public delegate void InGameMenuAcceptEventHandler();
	[Signal] public delegate void InGameMenuBackEventHandler();
	InGameMenu Main;
    public override void _Ready()
    {
        Main = GetParent<InGameMenu>();
    }
	
    public override void _Input(InputEvent @event) 
	{	
		if (@event.IsReleased() || @event is InputEventMouseMotion || @event is InputEventJoypadMotion ||
			Main.isHidden)
			return;
		if (!Main.Visible || Main.inputIdx == -2) return;

		if (Input.IsActionJustPressed("MenuUp" +  Main.keyboardKeyword + Main.inputIdx))
			{EmitSignal(SignalName.InGameMenuWASD, 0, -1);	return;}
		if (Input.IsActionJustPressed("MenuDown" +  Main.keyboardKeyword + Main.inputIdx))
			{EmitSignal(SignalName.InGameMenuWASD, 0, 1);	return;}
		if (Input.IsActionJustPressed("MenuLeft" +  Main.keyboardKeyword + Main.inputIdx))
			{EmitSignal(SignalName.InGameMenuWASD, -1, 0);	return;}
		if (Input.IsActionJustPressed("MenuRight" +  Main.keyboardKeyword + Main.inputIdx))
			{EmitSignal(SignalName.InGameMenuWASD, 1, 0);	return;}

		if (Input.IsActionJustPressed("MenuAccept" +  Main.keyboardKeyword + Main.inputIdx))
        {
			EmitSignal(SignalName.InGameMenuAccept);
            return;
        }
		
		if (Input.IsActionJustPressed("MenuBack" +  Main.keyboardKeyword + Main.inputIdx))
        {
			EmitSignal(SignalName.InGameMenuBack);
            return;
        }
    }
}
