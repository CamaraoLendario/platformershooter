using Godot;
using System;

[Tool]
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
        // if (@event is InputEventMouseMotion ||
		// 	//!playerCapsule.isEnabled||
		// 	@event.IsReleased()) 
		// 	return;
		
		// if(inputIdx == -1 && !isKeyboardControlled) return;

        if (Input.IsActionJustPressed("MenuUpKeyboard")){// +  keyboardKeyword + inputIdx)){
            GD.Print("MenuUp");
            playerCapsule.OnMoveAction(new Vector2(0, -1)); return;
        }
        if (Input.IsActionJustPressed("MenuDownKeyboard")){// +  keyboardKeyword + inputIdx)){
            GD.Print("MenuDown");
            playerCapsule.OnMoveAction(new Vector2(0, 1)); return;
        }
        if (Input.IsActionJustPressed("MenuLeftKeyboard")){// +  keyboardKeyword + inputIdx)){
            GD.Print("MenuLeft");
            playerCapsule.OnMoveAction(new Vector2(-1, 0)); return;
        }
        if (Input.IsActionJustPressed("MenuRightKeyboard")){// +  keyboardKeyword + inputIdx)){
            GD.Print("MenuRight");
            playerCapsule.OnMoveAction(new Vector2(1, 0)); return;
        }

		if (Input.IsActionJustPressed("MenuInteractKeyboard")){// +  keyboardKeyword + inputIdx)){
            GD.Print("MenuInteract");
            playerCapsule.OnInteract(); return;
        }
		
		if (Input.IsActionJustPressed("MenuBackKeyboard")){// +  keyboardKeyword + inputIdx)){
            GD.Print("MenuBack");
            playerCapsule.OnNegativeAction(); return;
        }
    }
}
