using Godot;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

[Tool]
public partial class RenameKeyboard : Control
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
	[Export] Label affectedLabel;
	[Export] LabelSettings letterSettings;
	MenuItemsGridContainer optionsContainer;
	Script menuItemScript = GD.Load<Script>("uid://dhewsc5ayax3f");
	public bool keyboardControlled = false;
	MenuSelectPanel menuSelectPanel;
	string typedString = "";
	string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
    public override void _Ready()
	{
		optionsContainer = GetNode<MenuItemsGridContainer>("OptionsContainer");
		Refresh();
		CallDeferred(MethodName.SpawnMenuSelectPanel);
		if (affectedLabel != null) typedString = affectedLabel.Text;
		
	}
	void Refresh()
	{
		foreach(Node node in optionsContainer.GetChildren())
		{
			if (node is not Panel && node is not MenuItemsListContainer) node.QueueFree();
		}

		Node[] newLetters = new Node[letters.Length];
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
			newLetters[i] = control; 
			//optionsContainer.CallDeferred(MethodName.AddChild, control);
			control.CallDeferred(MethodName.SetScript, menuItemScript);
			control.CallDeferred(MethodName.AddChild, newLetterButton);
		}

		optionsContainer.QueueMassAddChildren(newLetters);
	}
	protected void SpawnMenuSelectPanel()
	{
		MenuItem[] menuOptions = optionsContainer.GetMenuItems();
		menuSelectPanel = MenuSelectPanel.GetNewPannel(menuOptions);
		menuSelectPanel.ZIndex = 0;
		menuOptions[0].AddChild(menuSelectPanel);
	}
    public bool OnMoveAction(Vector2 dir)
	{
		if(keyboardControlled) return false;
		int dist = (int)dir.X + ((int)dir.Y * 4);
		menuSelectPanel.Move(Vector2.Down * dist);
		return true;
	}
    public string OnInteract()
	{
		if(keyboardControlled) return typedString;
		char letter = letters[menuSelectPanel.GetCurrentNodeIdx()];
		return AddLetter(letter);
	}
    public string OnNegativeAction()
	{
		if (keyboardControlled){
			Close();
			return typedString;
		}
		return RemoveLetter();
	}
    public bool OnAltInteract()
    {
		if (keyboardControlled) return false;
		return OnAccept();
    }
    public bool OnAccept()
    {
		Close();
		return true;
    }
    public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey eventKey || !Visible || !keyboardControlled || @event.IsReleased()) return;

		if (eventKey.KeyLabel == Key.Backspace)
		{
			RemoveLetter();
		}
		else if ((int)eventKey.Keycode <= 90 && (int)eventKey.Keycode >= 65){
			AddLetter(eventKey.KeyLabel.ToString()[0]);
			menuSelectPanel.SetGetSelectedNode((int)eventKey.Keycode - 65);
		}
		else if ((int)eventKey.Keycode == 32){
			AddLetter(" "[0]);
			menuSelectPanel.SetGetSelectedNode(optionsContainer.menuItems.Length - 1);
		}
	}
	public void Toggle()
	{
		if (Visible) Close();
		else Open();
	}
	public void Open()
	{	
		Show();
		menuSelectPanel.SetGetSelectedNode(0);
	}
	public void Close()
	{
		Hide();
	}
	public void SetText(string newText)
	{
		if (affectedLabel != null)
			affectedLabel.Text = newText;
		typedString = newText;
	}
	string AddLetter(char letter)
	{
		if (affectedLabel != null)
			affectedLabel.Text += letter;
		return typedString + letter;
	}
	string RemoveLetter()
	{
		if (affectedLabel != null && affectedLabel.Text != "")
			affectedLabel.Text = affectedLabel.Text.Remove(affectedLabel.Text.Length - 1);
		if (typedString != "") 
			typedString = typedString.Remove(typedString.Length - 1);
		return typedString;
	}
}
