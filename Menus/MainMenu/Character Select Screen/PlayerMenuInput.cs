using Godot;
using System;

public partial class PlayerMenuInput : Node
{
    [Export] PlayerCapsule playerCapsule;
	public int inputIdx = -2;
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
		playerCapsule = GetParent() as PlayerCapsule;
    }

    // TODO: add analog stick support for menu WASD 
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion ||
			!playerCapsule.isEnabled||
			@event.IsReleased()) 
			return;
		
		if(inputIdx == -1 && !isKeyboardControlled) return;

        if (Input.IsActionJustPressed("MenuUp" +  keyboardKeyword + inputIdx)){
            playerCapsule.OnMoveAction(new Vector2(0, -1)); return;
        }
        if (Input.IsActionJustPressed("MenuDown" +  keyboardKeyword + inputIdx)){
            playerCapsule.OnMoveAction(new Vector2(0, 1)); return;
        }
        if (Input.IsActionJustPressed("MenuLeft" +  keyboardKeyword + inputIdx)){
            playerCapsule.OnMoveAction(new Vector2(-1, 0)); return;
        }
        if (Input.IsActionJustPressed("MenuRight" +  keyboardKeyword + inputIdx)){
            playerCapsule.OnMoveAction(new Vector2(1, 0)); return;
        }

		if (Input.IsActionJustPressed("MenuAccept" +  keyboardKeyword + inputIdx)){
            playerCapsule.OnPositiveAction(); return;
        }
		
		if (Input.IsActionJustPressed("MenuBack" +  keyboardKeyword + inputIdx)){
            playerCapsule.OnNegativeAction(); return;
        }
    }
}
