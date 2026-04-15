using Godot;
using System;

public partial class PlayerMenuInput : Node
{
	[Signal] public delegate void MenuWASDEventHandler(int XMenuInput, int YMenuInput);
	[Signal] public delegate void AcceptStartEventHandler();
	[Signal] public delegate void BackStartEventHandler();
    [Signal] public delegate void StartStartEventHandler();
	public int inputIdx = -2;
	CharacterCapsule capsule;
	public bool IsKeyboardControlled
    {
        get
        {
            return isKeyboardControlled;
        }
        set
        {
            isKeyboardControlled = value;
			if (value)
            {
                keyboardKeyword = "Keyboard";
            }
			else keyboardKeyword = "";
        }
    }
	private bool isKeyboardControlled = false;
	string keyboardKeyword = "";
    public override void _Ready()
    {
		capsule = GetParent() as CharacterCapsule;
    }

    // TODO: add analog stick support for menu WASD 
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion ||
			!capsule.isEnabled||
			@event.IsReleased()) 
			return;
		
		if(inputIdx == -1 && !isKeyboardControlled) return;

        if (Input.IsActionJustPressed("MenuUp" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuUp was pressed by " + keyboardKeyword + inputIdx);
            EmitSignal(SignalName.MenuWASD, 0, -1); return;
        }
        if (Input.IsActionJustPressed("MenuDown" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuDown was pressed by " + keyboardKeyword + inputIdx);
            EmitSignal(SignalName.MenuWASD, 0, 1); return;
        }
        if (Input.IsActionJustPressed("MenuLeft" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuLeft was pressed by " + keyboardKeyword + inputIdx);
            EmitSignal(SignalName.MenuWASD, -1, 0); return;
        }
        if (Input.IsActionJustPressed("MenuRight" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuRight was pressed by " + keyboardKeyword + inputIdx);
            EmitSignal(SignalName.MenuWASD, 1, 0); return;
        }

		if (Input.IsActionJustPressed("MenuAccept" +  keyboardKeyword + inputIdx))
        {
            GD.Print("MenuAccept was pressed by " + keyboardKeyword + inputIdx);
            EmitSignal(SignalName.AcceptStart);
            return;
        }
		
		if (Input.IsActionJustPressed("MenuBack" +  keyboardKeyword + inputIdx))
        {
            GD.Print("MenuBack was pressed by " + keyboardKeyword + inputIdx);
            EmitSignal(SignalName.BackStart);
            return;
        }

        if (Input.IsActionJustPressed("MenuStart" +  keyboardKeyword + inputIdx))
        {
            GD.Print("Start was pressed by " + keyboardKeyword + inputIdx);
            EmitSignal(SignalName.StartStart);
            return;
        }
    }
}
