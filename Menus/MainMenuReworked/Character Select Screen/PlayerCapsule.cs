using Godot;
using Microsoft.VisualBasic;
using SpaceMages;
using System;
using System.Linq;
using System.Threading;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class PlayerCapsule : MenuItem
{
	[Signal] public delegate void ReadiedEventHandler(PlayerCapsule playerCapsule);
	[Signal] public delegate void UnReadiedEventHandler(PlayerCapsule playerCapsule);
	[Signal] public delegate void DisabledEventHandler(PlayerCapsule playerCapsule);
	#region Exports
	[ExportGroup("Nodes and References")]
	[Export] MenuItemsListContainer menuScreenOptions;
	[Export] Label nameLabel;
	[Export] TextureRect pilotTexture;
	[Export] Control enabledCapsule;
	[Export] Label arrows;
	[Export] Control disabledCapsule;
	[Export] RenameKeyboard renameKeyboard;
	[Export] public PlayerMenuInput inputNode;
	[Export] ShaderMaterial pilotOutlineShaderMaterial;
	[Export] StyleBoxFlat pannelTheme;
	[Export] Color readyBGcolor = new Color(0.161f, 1.0f, 0.161f, 0.396f);
	[Export] Color unreadyBGcolor = new Color(0.161f, 0.161f, 0.161f, 0.396f);
	#endregion
	InputIcon[] inputIcons = [];
	public CharacterSelectScreen characterSelectScreen;
	public bool isEnabled = false;
	public bool isReady = false;
	public int colorIdx = -1;

    public override void _Ready()
    {
        base._Ready();
		Disable();

		inputNode.Move += OnMoveAction;
		inputNode.Interact += OnInteract;
		inputNode.AltInteract += OnAltInteract;
		inputNode.NegativeAction += OnNegativeAction;

		inputIcons = GetInputIcons(this);
    }

	public void SetPlayerName(string newName)
	{
		renameKeyboard.SetText(newName);
		Name = newName + "'s capsule";
	}
	public string GetPlayerName()
	{
		return nameLabel.Text;
	}
	public void SetColor(int idx)
	{
		if (pilotTexture.Material == null) 
			pilotTexture.Material = pilotOutlineShaderMaterial.Duplicate() as ShaderMaterial;
		if (idx == colorIdx) return;
		int dir = 1;
		if (idx < colorIdx){dir = -1;}
		for (int i = 0; i < teamColors.Length; i++)
		{
			int currentColoridx = NormalizeIdx(idx + (i*dir), teamColors.Length);
			//GD.Print("trying to set color to: ", idx);
			if (characterSelectScreen.IsColorAvaliable(currentColoridx)){
				Vector3 newColor = teamColors[currentColoridx];
				(pilotTexture.Material as ShaderMaterial).SetShaderParameter("Color", newColor);
				colorIdx = currentColoridx;
				break;
			}
		}
		//GD.Print($"Color set to {colorIdx}!");
	}
	void ClearColor()
	{
		pilotTexture.Material = null;
		colorIdx = -1;
		return;
	}
	public void Enable(int inputIdx)
	{
		// if(isEnabled)
		// 	GD.Print(this.Name, " is already enabled!");

		enabledCapsule.Show();
		disabledCapsule.Hide();
		renameKeyboard.Close();
		isEnabled = true;
		inputNode.isEnabled = true;
		SetColor(0);
		InputGenerator.Instance.GeneratePlayerMenuInput(inputIdx);
		SetInputIdx(inputIdx);
	}
	public void Disable()
	{
		// if (!isEnabled)
		// 	GD.Print(this.Name, " is already disabled!");
		UnReady();
		SetPlayerName("");
		ClearColor();
		disabledCapsule.Show();
		enabledCapsule.Hide();
		renameKeyboard.Close();
		isEnabled = false;
		inputNode.isEnabled = false;
		EmitSignal(SignalName.Disabled, this);
		SetInputIdx(-2);
	}

	bool ReadyUp() // Note: name can't be "Ready" cuz of Node.Ready
	{ 
		if (isReady)
			return false;
		arrows.Hide();
		isReady = true;
		pannelTheme.BgColor = readyBGcolor;
		menuScreenOptions.Hide();
		EmitSignal(SignalName.Readied, this);

		return true;
	}
	bool UnReady()
	{
		if (!isReady)
			return false;
		isReady = false;
		arrows.Show();
		pannelTheme.BgColor = unreadyBGcolor;
		menuScreenOptions.Show();
		EmitSignal(SignalName.UnReadied, this);
		return true;
	}

    public override bool OnInteract()
	{
		if (renameKeyboard.Visible)
		{
			renameKeyboard.OnInteract();
			return true;
		}
		return ReadyUp();
	}
    public override bool OnAltInteract()
    {
		if (isReady) return false;
		if (renameKeyboard.Visible)
		{
			renameKeyboard.OnAltInteract();
			return true;
		}
		else
		{
			renameKeyboard.Open();
		}

        return false;
    }
    public override bool OnMoveAction(Vector2 dir)
	{
		if (isReady) return false;
		if (renameKeyboard.Visible)
		{
			renameKeyboard.OnMoveAction(dir);
			return true;
		}
		SetColor(colorIdx + (int)dir.X);
		return true;
	}

    public override bool OnNegativeAction()
    {
		if (renameKeyboard.Visible)
		{
			renameKeyboard.OnNegativeAction();
			return true;
		}	
      	if (UnReady()) return true;
		Disable();
		return true;
	}
    public override bool OnAccept()
    {
		if (renameKeyboard.Visible)
		{
			renameKeyboard.OnAccept();
			return true;
		}
		return false;
    }
	public int GetInputIdx()
	{
		return inputNode.GetInputIdx();
	}
	public int GetColorIdx()
	{
		return colorIdx;
	}
	public void SetInputIdx(int newInputIdx)
	{
		inputNode.SetInputIdx(newInputIdx);
		renameKeyboard.keyboardControlled = newInputIdx == -1;
		foreach(InputIcon inputIcon in inputIcons)
			inputIcon.SetInputIdx(newInputIdx);
	}
	InputIcon[] GetInputIcons(Node from)
	{
		InputIcon[] inputIcons = [];
		foreach (Node node in from.GetChildren())
		{
			if (node is InputIcon inputIcon)
				inputIcons = inputIcons.Append(inputIcon).ToArray();
            foreach (InputIcon nestedInputIcon in GetInputIcons(node))
                inputIcons = inputIcons.Append(nestedInputIcon).ToArray();
		}
		return inputIcons;
	}
}
