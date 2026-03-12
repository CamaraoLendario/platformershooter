using Godot;

[Tool]
public partial class DDRInputsParent : Node2D
{
	[Signal] public delegate void endedEventHandler();
	[Export] int DDRLength = 5;
	[Export] Texture2D InputButtonTexture;
	[Export] bool NextArrow
	{
		get
		{
			return false;
		}
		set
		{
			TurnNextArrow();
		}
	}
	public Player frozenPlayer;
	(JoyButton button, Vector2 inputVec)[] actions = {
		(JoyButton.Y, Vector2.Up), 
		(JoyButton.B, Vector2.Right), 
		(JoyButton.A, Vector2.Down), 
		(JoyButton.X, Vector2.Left)};
	int separation = 11;
	int currentDDRStep = 0;
	Sprite2D currentDDR;
	Tween shakeTween;
	public override void _Ready()
	{
		Reset();
	}

	void Reset()
	{
		if (GetChildCount() > 0)
		{
			foreach(Node2D child in GetChildren())
			{
				child.QueueFree();
			}
		}

		GenerateDDRArrows();

		int childCount = GetChildCount();
		for (int i = 0; i < childCount; i++)
		{
			GetChild<Sprite2D>(i).Position = Vector2.Right * separation * i;
		}

		currentDDRStep = -1;

		TurnNextArrow();
	}

	void GenerateDDRArrows()
	{
		for (int i = 0; i < DDRLength; i++)
		{
            Sprite2D newDDRArrow = new()
            {
                Texture = InputButtonTexture,
                Rotation = (Mathf.Pi / 2) * GD.RandRange(0, 3),
				ZIndex = 1,
            };
            AddChild(newDDRArrow);
		}
	}

	void CheckInput(Vector2 inputVec)
	{
		if (inputVec == actions[(int)(currentDDR.Rotation/(Mathf.Pi/2))].inputVec)
		{
			TurnNextArrow();
			ShakeCharSprite(inputVec);
		} 
	}

	void TurnNextArrow()
	{
		currentDDRStep ++;
		if (currentDDRStep >= DDRLength)
		{
			End();
			return;
		}
		currentDDR = GetChild<Sprite2D>(currentDDRStep);
		foreach(Node2D sprite in GetChildren())
		{
			sprite.SelfModulate = new Color(1, 1, 1, 0);
		}
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.InOut);
		tween.TweenMethod(Callable.From((float tweenFactor) => AnimateNextArrow(tweenFactor)), 0f, 1f, 0.08);
	}

	void ShakeCharSprite(Vector2 inputVec)
	{
		if (shakeTween != null && shakeTween.IsRunning()) shakeTween.Pause();
		AnimatedSprite2D playerSprite = frozenPlayer.pilot.sprite;
		playerSprite.Offset = inputVec * 5;
		
		shakeTween = CreateTween();
		shakeTween.SetTrans(Tween.TransitionType.Spring);
		shakeTween.SetEase(Tween.EaseType.Out);
		shakeTween.TweenProperty(playerSprite, "offset", Vector2.Zero, 0.2);
		shakeTween.Play();
	}

	void AnimateNextArrow(float tweenFactor)
	{
		Position = Vector2.Right * -separation * ((float)(currentDDRStep - 1) + tweenFactor);
		Position += Vector2.Up * 16;
		float halfTweenFactor = tweenFactor / 2;
		currentDDR.SelfModulate = new Color(1, 1, 1, 0.5f + halfTweenFactor);
		currentDDR.Scale = Vector2.One * (0.5f + halfTweenFactor);
		
		if((currentDDRStep - 2) >= 0)
		{
			float selfFactorOld = 0.5f - halfTweenFactor;
			Sprite2D currentSpriteOld = GetChild<Sprite2D>(currentDDRStep - 2);
			currentSpriteOld.SelfModulate = new Color(1, 1, 1, selfFactorOld);
			currentSpriteOld.Scale = Vector2.One * selfFactorOld;
		}

		if((currentDDRStep - 1) >= 0)
		{
			float selfFactorLeft = 1 - halfTweenFactor;
			Sprite2D currentSpriteLeft = GetChild<Sprite2D>(currentDDRStep - 1);
			currentSpriteLeft.SelfModulate = new Color(1, 1, 1, selfFactorLeft);
			currentSpriteLeft.Scale = Vector2.One * selfFactorLeft;
		}
		
		if((currentDDRStep + 1) < DDRLength)
		{
			float selfFactorRight = 0 + halfTweenFactor;
			Sprite2D currentSpriteRight = GetChild<Sprite2D>(currentDDRStep + 1);
			currentSpriteRight.SelfModulate = new Color(1, 1, 1, selfFactorRight);
			currentSpriteRight.Scale = Vector2.One * selfFactorRight;
		}
	}

    public override void _Input(InputEvent @event)
    {
		if (frozenPlayer.isKeyboardControlled)
		{
			string[] keyboardDirInputs = {"Up", "Right", "Down", "Left"};
			for(int i = 0; i < 4; i++)
			{
				if (Input.IsActionJustPressed(keyboardDirInputs[i]+"Keyboard-1"))
				{				
					CheckInput(actions[i].inputVec);
					return;
				}
			}
		}
		else
		{
			foreach((JoyButton button, Vector2 inputVec) action in actions)
			{
				if (Input.IsJoyButtonPressed(frozenPlayer.inputIdx, action.button))
				{				
					CheckInput(action.inputVec);
					return;
				}
			}
    	}
	}

	Vector2 CleanUpZeroes(Vector2 inputVec)
	{
		if (inputVec.X < 0.0001 && inputVec.X > -0.0001) inputVec.X = 0; 
		if (inputVec.Y < 0.0001 && inputVec.Y > -0.0001) inputVec.Y = 0; 
		return inputVec;
	}

	void End()
	{
		EmitSignal(SignalName.ended);
	}
}
