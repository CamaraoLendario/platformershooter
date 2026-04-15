using Godot;
using static SpaceMages.SpaceMagesVars;
using System.Globalization;
using System.Linq;

public partial class CharacterCapsule : Control
{
	[Signal] public delegate void changedReadyStateEventHandler();
	[Signal] public delegate void wasActivatedEventHandler(CharacterCapsule thisCapsule);
	[Signal] public delegate void wasDeactivatedEventHandler(CharacterCapsule thisCapsule);
	[Export] public PlayerMenuInput inputNode;
	[Export] Label PlayerNameDisplay;
	[Export] Panel colorChoice;
	[Export] Panel changeName;
	[Export] Panel readyButton;
	[Export] TextureRect playerSprite;
	[Export] Panel selectedButtonIndicator;
	[Export] public VBoxContainer buttonsContainer;
	[Export] VBoxContainer hasPlayerCapsule;
	[Export] VBoxContainer playerlessCapsule;
	[Export] PlayerNameKeyboard playerNameKeyboard;
	Shader outlineShader = GD.Load<Shader>("uid://cywx8daesh6uu");
	public bool isEnabled = false;
	public int CurrentColorIdx
	{
		get
		{
			return currentColorIdx;
		}
		set
		{
			SetColor(value);
		}
	}
	private int currentColorIdx = 0;
	public bool IsReady
	{
		get
		{
			return isReady;
		}
		set
		{
			if (isReady == value) return;
			isReady = value;
			EmitSignal(SignalName.changedReadyState);
			if (value) readyButton.Modulate = new Color(0.5f, 1, 0.5f, 1);
			else readyButton.Modulate = new Color(1, 1, 1, 1);
		}
	}
	private bool isReady = false;
	public Panel SelectedButton
	{
		get
		{
			return selectedButton;
		}
		set
		{
			selectedButton = value;
			selectedButtonIndicator.Reparent(value);
			selectedButtonIndicator.Position = Vector2.Zero;
			if(isEnabled && selectedButton.GetChild(0) is Label)
            {
                GD.Print((selectedButton.GetChild(0) as Label).Text);
				GD.Print(selectedButton == readyButton);
            }
		}
	}
	private Panel selectedButton;
	public CharacterSelectScreen Main;

	public override void _Ready()
	{
		
		inputNode.MenuWASD += ProcessWASD;
		inputNode.AcceptStart += ProcessOnAccept;
		inputNode.BackStart += ProcessOnBack;
		inputNode.StartStart += processStart;

		Disable();
		SelectedButton = buttonsContainer.GetChild<Panel>(0);
	}

	void ProcessWASD(int XMenuInput, int YMenuInput)
    {
		if (playerNameKeyboard.Visible) return;
		
		if (!Main.mapSelector.isCommenced)
        	CapsuleWASD(XMenuInput, YMenuInput);
		else Main.mapSelector.ChangePlayerMapSelection(new Vector2I(XMenuInput, YMenuInput), inputNode.inputIdx);
    }

	void CapsuleWASD(int XMenuInput, int YMenuInput)
	{
		if (YMenuInput > 0)
		{
			int Idx = SelectedButton.GetIndex() + 1;
			if (Idx >= buttonsContainer.GetChildCount())
			{
				Idx = 0;
			}
			SelectedButton = buttonsContainer.GetChild<Panel>(Idx);
		}
		else if (YMenuInput < 0)
		{
			int Idx = SelectedButton.GetIndex() - 1;
			if (Idx < 0)
			{
				Idx = buttonsContainer.GetChildCount() - 1;
			}
			SelectedButton = buttonsContainer.GetChild<Panel>(Idx);
		}

		else if (XMenuInput > 0 && selectedButton == colorChoice)
		{
			ChangeColor(1);
		}
		else if (XMenuInput < 0 && selectedButton == colorChoice)
		{
			ChangeColor(-1);
		}
	}

	void ProcessOnAccept()
    {
		if (playerNameKeyboard.Visible) return;
		if (!Main.mapSelector.isCommenced)
        	OnAccept();
		else
            Main.mapSelector.PlayerAccept(inputNode.inputIdx);
    }
	
	void OnAccept()
	{
		if (selectedButton == readyButton)
		{
			IsReady = !IsReady;
		}
		else if (selectedButton == changeName)
		{
			playerNameKeyboard.Open();
		}
	}

	void ProcessOnBack()
    {
		if (playerNameKeyboard.Visible) return;
		if (Main.mapSelector.isCommenced)
			Main.mapSelector.PlayerBack(inputNode.inputIdx);
		else if (IsReady)
			IsReady = false;
        else Leave();
    }
	void processStart()
	{
		if (playerNameKeyboard.Visible)
		{
			playerNameKeyboard.Close();
			return;
		}
	}

	public void Leave()
    {
		Main.OnDisabledCapsule(this);
		Disable();
		GetParent().MoveChild(this, -1);
	}

	public void SetPlayerName(string newName)
    {
		PlayerNameDisplay.Text = newName;
		Name = newName + "'s capsule";
	}
	public string GetPlayerName()
    {
		if (PlayerNameDisplay.Text != "")
        	return PlayerNameDisplay.Text;
		else return teamColorsDict.ElementAt(CurrentColorIdx).Key;
	}
	void ChangeColor(int direction)
	{
		if (direction >= 0)
			direction = 1;
		else direction = -1;

		int index = CurrentColorIdx;
		Main.colorAvaliability[index] = false;
		for (int i = 0; i < teamColors.Length; i++)
		{
			if (index + direction >= teamColors.Length)
			{
				index = 0;
			}
			else if (index + direction < 0)
			{
				index = teamColors.Length - 1;
			}
			else index += direction;

			if (!Main.colorAvaliability[index])
			{
				CurrentColorIdx = index;
				Main.PrintColorAvaliability();

				break;
			}
		}
	}

	void SetColor(int idx)
	{
		Vector3 newColor = teamColors[idx];
		(playerSprite.Material as ShaderMaterial).SetShaderParameter("Color", newColor);
		(colorChoice.GetChild(0).GetChild(1) as Label).Text = teamColorsDict.Keys.ElementAt(idx);
		playerNameKeyboard.PlayerColor = new Color(newColor.X, newColor.Y, newColor.Z);
		Main.colorAvaliability[idx] = true;
		currentColorIdx = idx;
	}

	public void Enable(int inputIdx, int colorIdx = -1)
	{
		InputGenerator.Instance.GeneratePlayersMenuInput(inputIdx);
		inputNode.inputIdx = inputIdx;
		GD.Print("new capsule input = " + inputNode.inputIdx);
		isEnabled = true;
		if (inputIdx == -1) inputNode.IsKeyboardControlled = true;
		else inputNode.IsKeyboardControlled = false;

		SelectedButton = colorChoice;
		hasPlayerCapsule.Visible = true;
		playerlessCapsule.Visible = false;

		if (playerSprite.Material == null)
		{
			GD.Print("Loading player outline shader...");
            ShaderMaterial newShaderMaterial = new()
            {
                Shader = outlineShader
            };
            playerSprite.Material = newShaderMaterial;
		}
		CurrentColorIdx = teamColors.Length - 1;

		if (colorIdx < 0 || colorIdx >= teamColors.Length)
		{
			ChangeColor(1);
		}
		else SetColor(colorIdx);
	}

	public void Disable()
	{
		InputGenerator.Instance.ClearMenuInput(inputNode.inputIdx);
		inputNode.inputIdx = -2;
		isEnabled = false;

		hasPlayerCapsule.Hide();
		playerlessCapsule.Show();
		Main.colorAvaliability[CurrentColorIdx] = false;
		IsReady = false;
		GD.Print("Capsule number " + GetIndex() + " has been disabled");
	}
}