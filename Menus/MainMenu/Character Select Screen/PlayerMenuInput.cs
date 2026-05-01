using Godot;
using System;
using System.Data.Common;

[Tool]
public partial class PlayerMenuInput : Node
{
    [Export] PlayerCapsule playerCapsule;
	int inputIdx = -2;
	string keyboardKeyword = "";
    public override void _Ready(){
		playerCapsule = GetParent() as PlayerCapsule;
    }
    // TODO: add analog stick support for menu WASD 
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion ||
            !playerCapsule.isEnabled||
            @event.IsReleased()) 
            return;
		

        if (Input.IsActionJustPressed("MenuUp" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuUp");
            if (playerCapsule.OnMoveAction(new Vector2(0, -1)))
                GetViewport().SetInputAsHandled();
            return;
        }
        if (Input.IsActionJustPressed("MenuDown" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuDown");
            if (playerCapsule.OnMoveAction(new Vector2(0, 1)))
                GetViewport().SetInputAsHandled();
            return;
        }
        if (Input.IsActionJustPressed("MenuLeft" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuLeft");
            if (playerCapsule.OnMoveAction(new Vector2(-1, 0)))
                GetViewport().SetInputAsHandled();
            return;
        }
        if (Input.IsActionJustPressed("MenuRight" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuRight");
            if (playerCapsule.OnMoveAction(new Vector2(1, 0)))
                GetViewport().SetInputAsHandled();
            return;
        }
		if (Input.IsActionJustPressed("MenuInteract" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuInteract");
            if (playerCapsule.OnInteract())
                GetViewport().SetInputAsHandled();
            return;
        }
        if (Input.IsActionJustPressed("MenuAltInteract" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuAltInteract");
            if (playerCapsule.OnAltInteract())
                GetViewport().SetInputAsHandled();
            return;
        }
		if (Input.IsActionJustPressed("MenuBack" +  keyboardKeyword + inputIdx)){
            GD.Print("MenuBack");
            if (playerCapsule.OnNegativeAction())
                GetViewport().SetInputAsHandled();
            return;
        }
    }
    public int GetInputIdx()
    {
        return inputIdx;
    }
    public void SetInputIdx(int newIdx)
    {
        if (newIdx < -1)
            InputGenerator.Instance.RemovePlayerMenuInput(inputIdx);
        
        inputIdx = newIdx;

        if (inputIdx == -1)
            keyboardKeyword = "Keyboard";
        else
            keyboardKeyword = "";
        
    }
}
