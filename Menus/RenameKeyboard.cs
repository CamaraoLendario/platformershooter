using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class RenameKeyboard : MenuItem
{
	[Export] Vector2 letterSize = new Vector2(40, 40);
	[Export] int LineLength
	{
		get
		{
			return lineLength;
		}
		set
		{
			lineLength = value;
			if (Engine.IsEditorHint() && IsNodeReady()) Refresh();
		}
	}
	int lineLength = 4;
	[ExportGroup("References")]
	[Export] LabelSettings letterSettings;
	[Export] MenuItemsGridContainer optionsContainer;
	[Export] Script menuItemScript;
	MenuSelectPanel menuSelectPanel;

	string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
    public override void _Ready()
	{
		Refresh();
		CallDeferred(MethodName.SpawnMenuSelectPanel);
	}
	void Refresh()
	{
		foreach(Node node in optionsContainer.GetChildren())
		{
			if (node is not Panel && node is not MenuItemsListContainer) node.QueueFree();
		}

		for(int i = 0; i < letters.Length; i++)
		{
			Control control = new Control()
			{
				Size = new Vector2(40, 40),
			};
			Label newLetterButton = new Label()
			{
				Size = new Vector2(40, 40),
				Text = letters[i].ToString(),
				LabelSettings = letterSettings,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
			};
			optionsContainer.CallDeferred(MethodName.AddChild, control);
			control.CallDeferred(MethodName.SetScript, menuItemScript);
			control.CallDeferred(MethodName.AddChild, newLetterButton);
		}
		optionsContainer.CallDeferred(MenuItemsGridContainer.MethodName.ReorganizeItems);
	}
	protected void SpawnMenuSelectPanel()
	{
		MenuItem[] menuOptions = optionsContainer.GetMenuItems();
		menuSelectPanel = MenuSelectPanel.GetNewPannel(menuOptions);
		menuOptions[0].AddChild(menuSelectPanel);
	}

    public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse || @event is InputEventJoypadButton || @event is InputEventJoypadMotion || @event.IsReleased()) return;
		OnMoveAction(Input.GetVector("MenuLeftKeyboard", "MenuRightKeyboard", "MenuUpKeyboard", "MenuDownKeyboard"));
	}

    public override bool OnMoveAction(Vector2 dir)
	{
		int dist = (int)dir.X + ((int)dir.Y * 4);
		menuSelectPanel.Move(Vector2.Down * dist);
		return true;
	}

}
