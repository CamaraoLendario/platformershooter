using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[Tool]
public partial class InputIcon : TextureRect
{
	[Export] string action;
	public enum ControllerUsed{
		PlayStation,
		Xbox,
		Nintendo,
		NintendoClassic
	};
	AtlasTexture texture;
	bool isPressed = false;
	ControllerUsed currentControllerUsed = ControllerUsed.Xbox;
	

    public override void _Ready()
    {
		this.Texture = this.Texture.Duplicate() as AtlasTexture; 
    	texture = Texture as AtlasTexture;

		SetDisplayedButton(action, 0, false);
    }

    public override void _Input(InputEvent @event)
	{
		string oldAction = action;
		bool oldIsPressed = isPressed;
		ControllerUsed oldControllerUsed = currentControllerUsed;
		if (@event is InputEventKey && !action.EndsWith("Keyboard")){
			GD.Print("keyboard detected!!");
			action += "Keyboard";
		}
		else if(@event is InputEventJoypadButton|| @event is InputEventJoypadMotion){
			if(action.EndsWith("Keyboard"))
				action = action.Remove(action.Length - 8);

			string controllerName = (string)Input.GetJoyInfo(@event.Device)["raw_name"];

			if (controllerName.Contains("PS"))
				currentControllerUsed = ControllerUsed.PlayStation;
			else if (controllerName.Contains("Nintendo"))
				currentControllerUsed = ControllerUsed.Nintendo;
			else if (controllerName.Contains("NES") || controllerName.Contains("Famicom"))
				currentControllerUsed = ControllerUsed.NintendoClassic;
			else currentControllerUsed = ControllerUsed.Xbox;
		}

		if (Input.IsActionPressed(action)){
			isPressed = true;
		}
		else{
			isPressed = false;
		}
		
		if (oldControllerUsed != currentControllerUsed || oldAction != action || oldIsPressed != isPressed)
			SetDisplayedButton(action, 0, isPressed, currentControllerUsed);
	}
	public void SetDisplayedButton(string action, int eventIdx = 0, bool pressed = false, ControllerUsed controllerUsed = ControllerUsed.Xbox)
	{
		SetDisplayedButton(InputMap.ActionGetEvents(action)[eventIdx], pressed, controllerUsed);
	}
	public void SetDisplayedButton(InputEvent inputEvent, bool pressed = false, ControllerUsed controllerUsed = ControllerUsed.Xbox)
	{
		if (inputEvent is InputEventKey inputEventKey)
			texture.Region = GetInputIconPos(inputEventKey.PhysicalKeycode, pressed);
		else if(inputEvent is InputEventJoypadButton inputjoybutton)
			texture.Region = GetInputIconPos(inputjoybutton.ButtonIndex, pressed, controllerUsed);
		return;
	}

    Rect2 GetInputIconPos(JoyButton input, bool pressed = false, ControllerUsed controllerUsed = ControllerUsed.Xbox){
		Vector2 tilePos;
		
		if (input == JoyButton.DpadDown || input == JoyButton.DpadUp ||
			input == JoyButton.DpadLeft || input == JoyButton.DpadRight){
			tilePos = new Vector2(17, 0) + GetDpadOffset();
		}
		else
		switch (input)
		{
			case JoyButton.X:
				tilePos = new Vector2(2, 0) + ControllerLayoutOffset(controllerUsed);
				break;	
			case JoyButton.Y:
				tilePos = new Vector2(3, 0) + ControllerLayoutOffset(controllerUsed);
				break;
			case JoyButton.A:
				tilePos = new Vector2(2, 1) + ControllerLayoutOffset(controllerUsed);
				break;	
			case JoyButton.B:
				tilePos = new Vector2(3, 1) + ControllerLayoutOffset(controllerUsed);
				break;
			case JoyButton.LeftShoulder:
				tilePos = new Vector2(8, 1);
				break;
			case JoyButton.RightShoulder:
				tilePos = new Vector2(9, 1);
				break;
			case JoyButton.Back:
				tilePos = new Vector2(10, 1);
				break;
			case JoyButton.Start:
				tilePos = new Vector2(11, 1);
				break;		
			default:
				tilePos = new Vector2(27, 23);
				pressed = false;
				break;
		}
		
		if(pressed)
			tilePos += Vector2.Down * 4;
		return new Rect2(tilePos * 16, Vector2.One * 16);
	}
	Rect2 GetInputIconPos(Key input, bool pressed)
	{
		Vector2 tilePos = new Vector2(27, 23);

		if((int)input <= 90 && (int)input >= 65)
		{
			tilePos = new Vector2(19, 10);
			int alphabetInt = (int)input - 65;
			tilePos += new Vector2(alphabetInt % 10, (int)(alphabetInt/10));
		}
		else if ((int)input <= 57 && (int)input >= 48)
		{
			tilePos = new Vector2(19, 9);
			int numberInt = (int)input - 48;
			tilePos += new Vector2(numberInt, 0);
		}
		else if ((int) input == 32)
		{
			tilePos = new Vector2(25, 12);
		}
		if (pressed)
		{
			tilePos += Vector2.Down * 4;
		}
		return new Rect2(tilePos * 16, Vector2.One * 16);
	}

	Vector2 ControllerLayoutOffset(ControllerUsed controllerUsed)
	{
		switch (controllerUsed)
		{
			case ControllerUsed.PlayStation:
				return Vector2.Right * -2;
			case ControllerUsed.Xbox:
				return Vector2.Right * 0;
			case ControllerUsed.Nintendo:
				return Vector2.Right * 2;
			case ControllerUsed.NintendoClassic:
				return Vector2.Right * 4;
		}
		return Vector2.Zero;
	}

	Vector2 GetDpadOffset()
	{
		float x = 0;

		x += Input.IsJoyButtonPressed(0, JoyButton.DpadLeft) ? -1 : 0;
		x += Input.IsJoyButtonPressed(0, JoyButton.DpadRight) ? 1 : 0;
		x += Input.IsJoyButtonPressed(0, JoyButton.DpadUp) ? -3 : 0;
		x += Input.IsJoyButtonPressed(0, JoyButton.DpadDown) ? 3 : 0;

		return new Vector2(x, 0);
	}
}
