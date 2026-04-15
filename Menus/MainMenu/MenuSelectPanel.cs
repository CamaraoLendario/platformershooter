using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static SpaceMages.SpaceMagesVars;

public partial class MenuSelectPanel : Panel
{
	[Export] Control[] options;
	int currentButtonIdx = 0;

    public override void _Ready()
    {
		if (GetParent() is not Control)
		{
			GD.PrintErr("Pannel is not child of control. reparenting to first option");
			Reparent(options[0]);
			currentButtonIdx = 0;
			normalizeSettings();
			return;
		}

		Control currentParent = GetParent<Control>();
		for(int i = 0; i < options.Length; i++)
		{
			if (currentParent == options[i])
			{
				currentButtonIdx = i;
				break;
			}	
		}

		Reparent(options[currentButtonIdx]);
		normalizeSettings();
    }

	public int Move(Vector2 dir)
    {
		currentButtonIdx = NormalizeIdx(currentButtonIdx + (int) dir.Y, options.Length);
        Reparent(options[currentButtonIdx], true);
		AnimateSelectedButton();
		return currentButtonIdx;
    }
	public Control MoveGetNode(Vector2 dir)
    {
		currentButtonIdx = NormalizeIdx(currentButtonIdx + (int) dir.Y, options.Length);
		Control newParent = options[currentButtonIdx];
        Reparent(newParent, true);
		AnimateSelectedButton();
		return newParent;
    }
	void AnimateSelectedButton()
	{
		Tween tween = CreateTween();
			Vector2 panelInitialPosition = Position;
			Vector2 panelInitialSize = Size;
			Vector2 panelFinalSize = (GetParent() as Control).Size;
			Vector2 posDifference = Vector2.Zero - panelInitialPosition;
			Vector2 sizeDifference = panelFinalSize - panelInitialSize;

		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			Position = panelInitialPosition + (posDifference * tweenedValue);
			Size = panelInitialSize + (sizeDifference * tweenedValue);
		}), 0f, 1f, 0.1);
	}
	void normalizeSettings()
	{
		Control parent = GetParent() as Control;
		Position = Vector2.Zero;
		Size = parent.Size; 
	}

	static public MenuSelectPanel GetNewPannel(Control[] Options)
	{
        MenuSelectPanel newPanel = GD.Load<PackedScene>("uid://4s3fc21oaq4i").Instantiate<MenuSelectPanel>();
        newPanel.options = Options;
        return newPanel;
	}
}
