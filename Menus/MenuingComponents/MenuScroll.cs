using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class MenuScroll : MenuItem
{
	[ExportToolButton("ScrollDown")]
    public Callable ScrollDownButton => Callable.From(ScrollDown);

    public void ScrollDown()
    {
		Scroll(-1);
    }
		[ExportToolButton("ScrollUp")]
    public Callable ScrollUpButton => Callable.From(scrollUp);

    public void scrollUp()
    {
		Scroll(1);
    }
	[Export] public string[] Contents
	{
		get
		{
			return contents;
		}
		set
		{
			contents = value;
			if (labelsContainer != null) SetLabels(1);
		}
	}
	protected string[] contents = [
		"Option1",
		"Option2",
		"Option3",
		"Option4",
		"Option5"
	];
	[ExportGroup("Scroller Settings")]
	[Export] int Separation
	{
		get
		{
			return separation;
		}
		set
		{
			separation = value;
			UpdateSeparation();
		}
	}
	int separation;
	[ExportGroup("Nodes")]
	[Export] Control labelsContainer;

	protected int currentIdx {get; private set;}= 0;
	int labelShowing = 2;
	Tween tween;

	public override void _Ready()
	{
		base._Ready();
		labelsContainer = GetNode<Control>("%scrollingLabels");
		currentIdx = -1;
		Scroll(1, true);
		CallDeferred(MethodName.UpdateHelperMinimumSize);
		CallDeferred(MethodName.Close);
	}

	public void UpdateHelperMinimumSize()
	{
		float maxSize = -1;
		foreach(Node child in labelsContainer.GetChildren())
		{
			if (child is not Label label) continue;
			
			if (label.Size.X > maxSize) 
				maxSize = label.Size.X; 
		}
		panelHelper.closedSettings.size.X = maxSize;
		panelHelper.openSettings.size.X = maxSize;
	}

	public void Open()
	{
		for(int i = 0; i < 4; i++)
		{
			labelsContainer.GetChild<Label>(i).Visible = true;
		}
		isActivated = true;
		panelHelper.SetPosAndSizeOpen();
		Label currentLabel = GetCurrentLabel();
		Vector2 initialLabelPos = currentLabel.Position;
		Vector2 finalLabelPos = new Vector2(
			-currentLabel.Size.X/2f,
			currentLabel.Position.Y);
		AnimateFlushPositioning(currentLabel, initialLabelPos, finalLabelPos);
	}

	public void Close()
	{
		for(int i = 0; i < 4; i++)
		{
			if (i == labelShowing)
				labelsContainer.GetChild<Label>(i).Visible = true;
			else	
				labelsContainer.GetChild<Label>(i).Visible = false;
		}
		isActivated = false;
		panelHelper.SetPosAndSizeClosed();
		isActivated = false;
		Label currentLabel = GetCurrentLabel();
		Vector2 initialLabelPos = currentLabel.Position;
		Vector2 finalLabelPos = new Vector2(
			(GetClosedSettings().size.X/2f) - currentLabel.Size.X,
			currentLabel.Position.Y);
		AnimateFlushPositioning(currentLabel, initialLabelPos, finalLabelPos);
	}

	void AnimateFlushPositioning(Label currentLabel, Vector2 initialLabelPos, Vector2 finalLabelPos)
	{
		Tween flushTween = CreateTween();
		flushTween.SetTrans(Tween.TransitionType.Sine);
		flushTween.SetEase(Tween.EaseType.InOut);

		Vector2 posDifference = finalLabelPos - initialLabelPos; 
		flushTween.TweenMethod(Callable.From((float tweenedValue) => {
			initialLabelPos = new Vector2(initialLabelPos.X, currentLabel.Position.Y);
			currentLabel.Position = initialLabelPos + (posDifference * Vector2.Right * tweenedValue);
		}),0f, 1f, 0.2);
	}

	public string GetCurrentSelection()
	{
		return contents[currentIdx];
	}

	Label GetCurrentLabel()
	{
		return labelsContainer.GetChild<Label>(labelShowing);
	}

	void SetLabels(int dir)
	{
		for(int i = 0; i < 4; i++)
		{
			int value = (dir - 1)/2;
			int labelGMIdxOffset = - 2 - value;

			Label currentLabel = labelsContainer.GetChild(i) as Label;
			
			currentLabel.Text = contents[NormalizeIdx(i + currentIdx + labelGMIdxOffset , contents.Length)];
			currentLabel.Size *= Vector2.Down;
		}	
	}

    public override bool OnInteract()
    {
		if (isActivated)
		{
			Close();
			return true;
		}
		else
		{
			Open();
			return true;
		}
    }
    public override bool OnNegativeAction()
	{
		if (isActivated)
		{
			Close();
			return true;
		}
		return false;
	}

    public override bool OnMoveAction(Vector2 dir)
	{
		if (dir.Y == 0) return false;
		if (isActivated)
		{
			Scroll((int)dir.Y);
			return true;
		}
		return false;
	}

	public (Vector2 position, Vector2 size) GetClosedSettings()
	{
		return panelHelper.closedSettings;
	}
	public (Vector2 position, Vector2 size) GetOpenSize()
	{
		return panelHelper.openSettings;
	}

	public virtual void Scroll(int direction, bool skipAnimation = false)
	{
		bool scrollDown = true;
		labelShowing = 1;
		if (direction == 1){
			scrollDown = false;
			labelShowing = 2;
		}

		currentIdx = NormalizeIdx(currentIdx + direction, contents.Length);

		SetLabels(direction);
		CallDeferred(MethodName.AnimateScrolling, scrollDown, skipAnimation);
	}

	void UpdateSeparation()
	{ //TODO: make separation work after either scroll
		for(int i = 0; i < 4; i++)
		{
			Label currentLabel = new Label();//labelsContainer.GetChild<Label>(i);
			currentLabel.Position = new Vector2(
				-currentLabel.Size.X/2f,
				separation * (i - 2) - currentLabel.Size.Y/2f
			);
		}
	}

	void AnimateScrolling(bool down = true, bool skipAnimation = false)
	{
		float animationTime = 0.2f;
		if (skipAnimation) animationTime *= 0;
		if (tween != null && tween.IsRunning()) tween.Kill();
		tween = CreateTween();
		tween.SetEase(Tween.EaseType.InOut);
		tween.SetTrans(Tween.TransitionType.Sine);
		
		List<(Label node, Vector2 initialScale, Vector2 initialPosition, float initialAlpha)> labels = [];
		
		foreach (Node node in labelsContainer.GetChildren())
		{
			if (node is not Label label) continue;

			labels.Add((label, label.Scale, label.Position, label.Modulate.A));
		}

		for(int i = 0; i < labels.Count; i++)
		{
			(Label node, Vector2 initialScale, Vector2 initialPosition, float initialAlpha) currentLabel = labels[i];
			int index = i;
			
			if (!down) index ++;
			currentLabel.initialPosition = new Vector2(
				-currentLabel.node.Size.X/2f,
				Separation * (index - 2) - currentLabel.node.Size.Y/2f
			);

			if (!down) index = 3 - index + 1;
			currentLabel.initialScale = ((index % 2 != 0) ? .75f : index / 2) * Vector2.One;
			currentLabel.initialAlpha = (index % 2 != 0) ? .5f : index / 2;
			labels[i] = currentLabel;
		}



		tween.TweenMethod(Callable.From((float tweenedValue) => {
			for(int i = 0; i < labels.Count; i++)
			{
				(Label node, Vector2 intialScale, Vector2 initialPosition, float initialAlpha) currentLabel = labels[i];
				Label node = currentLabel.node;
				Vector2 initialScale = currentLabel.intialScale;
				Vector2 initialPosition = currentLabel.initialPosition;
				float initialAlpha = currentLabel.initialAlpha;

				int index = i;
				if (!down) index = 3 - index;
				node.Scale = initialScale + ((0.75f - (0.5f * index)) * tweenedValue *  Vector2.One );
				int clampedI = Math.Clamp(index - 1, 0, 1);
				float alpha = initialAlpha + ((0.5f - clampedI) * tweenedValue);
				node.Modulate = new Color(1, 1, 1, alpha);

				if (!down)
					node.Position = initialPosition + (Vector2.Up * Separation * tweenedValue); 
				else
					node.Position = initialPosition + (Vector2.Down * Separation * tweenedValue);
			}
		}),0f, 1f, animationTime);
	}
}