using Godot;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters;

[Tool]
public partial class HorizontalTextAndScroll : HorizontalMenu
{
	
	[Signal] public delegate void settingChangedEventHandler(int newSetting);
	[Export] public string[] Contents
	{
		get
		{
			return contents;
		}
		set
		{
			contents = value;
			if (menuScroll != null)
				menuScroll.Contents = value;
			CallDeferred(MethodName.DoFormatting);
		}
	}
	private string[] contents = [
		"Option1",
		"Option2",
		"Option3",
		"Option4",
		"Option5"
	];
	MenuScroll menuScroll;

    public override void _Ready()
    {
        base._Ready();
		menuScroll = GetNode<MenuScroll>("Scroller");
        menuScroll.Contents = Contents;
		menuScroll.UpdateHelperMinimumSize();
		DoFormatting();
		originalPosition = Position;
    }

    public override bool OnInteract()
    {
		bool result = menuScroll.OnInteract();
		if (!menuScroll.isActivated)
		{
			EmitSignal(SignalName.settingChanged);
		}
        return result;
    }

    public override bool OnNegativeAction()
    {
		bool result = menuScroll.OnNegativeAction();
		if (!menuScroll.isActivated)
		{
			EmitSignal(SignalName.settingChanged);
		}
        return result;
    }

    public override bool OnMoveAction(Vector2 dir)
    {
        return menuScroll.OnMoveAction(dir);
    }

	protected override void DoFormatting()
	{
		base.DoFormatting();
		if (menuScroll == null)
		{
			menuScroll = GetNode<MenuScroll>("Scroller");
		}

		Vector2 BordedTotalSize = TotalSize - (Vector2.Right * borderSize);
		menuScroll.Position = (BordedTotalSize * new Vector2(0.5f, 0)) - menuScroll.GetClosedSettings().size.X / 2f * Vector2.Right;
	}
}
