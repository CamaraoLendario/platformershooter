using Godot;
using System;

public partial class MenuController : Node
{
	protected Vector2 inputVec = Vector2.Zero;
	protected Timer inputSpammerDelay = new Timer()
	{
		OneShot = true
	};
	bool keepSpammingInputs = false;

    public override void _Ready()
	{
		AddChild(inputSpammerDelay);
		inputSpammerDelay.Timeout += OnSpammerDelayTimeout;
	}

    public Vector2 GetInputVectorNotNormalized(string negX, string posX, string negY, string posY)
	{
		Vector2 inputVec = Vector2.Zero;
		if (Input.IsActionPressed(negX)) inputVec += Vector2.Left; 
		if (Input.IsActionPressed(posX)) inputVec += Vector2.Right; 
		if (Input.IsActionPressed(negY)) inputVec += Vector2.Up; 
		if (Input.IsActionPressed(posY)) inputVec += Vector2.Down; 
		return inputVec;
	}

	public Vector2 GetMenuDirFromJoyStick(InputEventJoypadMotion motion)
	{
		int x = (int)(Input.GetJoyAxis(motion.Device, JoyAxis.LeftX) * 2f);
		int y = (int)(Input.GetJoyAxis(motion.Device, JoyAxis.LeftY) * 2f);
		int rightX = (int)(Input.GetJoyAxis(motion.Device, JoyAxis.RightX) * 2f);
		int rightY = (int)(Input.GetJoyAxis(motion.Device, JoyAxis.RightY) * 2f);

		if (Mathf.Abs(x) < Mathf.Abs(rightX)) x = rightX;
		if (Mathf.Abs(y) < Mathf.Abs(rightY)) y = rightY;
		
		Vector2 joyVector = new (x, y);

		Vector2 inputVector = Vector2.Zero;
		if (joyVector.Length() > 1){
			if (joyVector.X > 0.55)
				inputVector += new Vector2(1, 0);
			if (joyVector.Y > 0.55)
				inputVector += new Vector2(0, 1);
			if (joyVector.X < -0.55)
				inputVector += new Vector2(-1, 0);
			if (joyVector.Y < -0.55)
				inputVector += new Vector2(0, -1);
		}

		return inputVector;
	}
	public virtual void UpdateInputVec(Vector2 newinputVec)
	{
		if (inputVec == newinputVec) return;
		inputVec = newinputVec;

		inputSpammerDelay.Stop();
		keepSpammingInputs = false;
		if (inputVec == Vector2.Zero)
			return;
		inputSpammerDelay.Start(0.6);

		OnMoveAction(inputVec);
	}
	public void OnSpammerDelayTimeout()
	{
		keepSpammingInputs = true;
		StartInputSpam();
	}

	public async void StartInputSpam()
	{
		if (!keepSpammingInputs) return;
		OnMoveAction(inputVec);
		await ToSignal(GetTree().CreateTimer(0.2f), Timer.SignalName.Timeout);
		StartInputSpam();
	}
	public virtual void OnMoveAction(Vector2 inputvec)
	{
		GD.PrintErr("OnMoveAction() not implemented");
	}
}
