using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class MenuSelectPanel : Panel
{
	[Export] MenuItem[] options;
	int currentOptionIdx = 0;
	public Tween tween;
	public Node oldParent;

	void onTreeEntered()
	{
		SetDeferred(PropertyName.oldParent, GetParent());
	}
    public override void _Ready()
    {
		if (GetParent() is not Control)
		{
			GD.PrintErr("Pannel is not child of control. reparenting to first option");
			Reparent(options[0]);
			currentOptionIdx = 0;
			normalizeSettings();
			return;
		}
		TreeEntered += onTreeEntered;
		Control currentParent = GetParent<Control>();
		for(int i = 0; i < options.Length; i++)
		{
			if (currentParent == options[i])
			{
				currentOptionIdx = i;
				break;
			}
		}

		Reparent(options[currentOptionIdx]);
		normalizeSettings();
    }

	public int Move(Vector2 dir)
    {
		MoveGetSelectedNode(dir);
		return currentOptionIdx;
    }
	public MenuItem MoveGetSelectedNode(Vector2 dir)
    {
		return SetGetSelectedNode(currentOptionIdx + (int) dir.Y);
    }
	public MenuItem SetGetSelectedNode(int Pos)
	{
		MenuItem oldParent = options[currentOptionIdx];
		if (Pos == currentOptionIdx) return oldParent;
		currentOptionIdx = NormalizeIdx(Pos, options.Length);
		MenuItem newParent = options[currentOptionIdx];
        Reparent(newParent, true);
		AnimateSelectedButton(newParent, oldParent);
		return newParent;
	}
	public MenuItem GetCurrentNode()
	{
		return options[currentOptionIdx];
	}
	public int GetCurrentNodeIdx()
	{
		return currentOptionIdx;
	}
	public void ForcePosAndSize(Vector2 position, Vector2 size, bool animate = true)
	{
		if (animate){
			ForceAnimateSelectedButton(position, size);
		}
		else
		{
			Position = position;
			Size = size;
		}
	}
	void AnimateSelectedButton(MenuItem newParent, MenuItem oldParent)
	{
		if (tween != null && tween.IsRunning())
		{
			tween.Kill();
		}
		tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		
		Vector2 panelInitialPosition = Position;
		Vector2 panelInitialSize = Size;
		Vector2 panelFinalSize = (GetParent() as Control).Size;
		Vector2 posDifference = Vector2.Zero - panelInitialPosition;
		Vector2 sizeDifference = panelFinalSize - panelInitialSize;
		Vector2 scaleDifference = Vector2.Zero;
		Vector2 initialScale = Scale;
		
		if(GetParent<MenuItem>().GetParent() is MenuItemsListContainer screenOptions)
		{
			scaleDifference = screenOptions.Scale - Scale;
			initialScale = Scale / screenOptions.Scale;
		}

		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			Position = panelInitialPosition + (posDifference * tweenedValue);
			Size = panelInitialSize + (sizeDifference * tweenedValue);
			Scale = initialScale + (scaleDifference * tweenedValue);
		}), 0f, 1f, 0.1);
	}
	
	void normalizeSettings()
	{
		Control parent = GetParent() as Control;
		Position = Vector2.Zero;
		Size = parent.Size; 
	}

	void ForceAnimateSelectedButton(Vector2 finalPosition, Vector2 finalSize)
	{
		tween = CreateTween();
			Vector2 panelInitialPosition = Position;
			Vector2 panelInitialSize = Size;
			Vector2 posDifference = finalPosition - panelInitialPosition;
			Vector2 sizeDifference = finalSize - panelInitialSize;

		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			Position = panelInitialPosition + (posDifference * tweenedValue);
			Size = panelInitialSize + (sizeDifference * tweenedValue);
		}), 0f, 1f, 0.1);
	}

	static public MenuSelectPanel GetNewPannel(MenuItem[] Options)
	{
        MenuSelectPanel newPanel = GD.Load<PackedScene>("uid://4s3fc21oaq4i").Instantiate<MenuSelectPanel>();
        newPanel.options = Options;
        return newPanel;
	}
}
