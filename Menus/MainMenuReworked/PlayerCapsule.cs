using Godot;
using Microsoft.VisualBasic;
using SpaceMages;
using System;
using System.Linq;
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
	[Export] Control disabledCapsule;
	[Export] public PlayerMenuInput inputNode;
	[Export] ShaderMaterial pilotOutlineShaderMaterial;
	[Export] StyleBoxFlat pannelTheme;
	[Export] Color readyBGcolor = new Color(0.161f, 1.0f, 0.161f, 0.396f);
	[Export] Color unreadyBGcolor = new Color(0.161f, 0.161f, 0.161f, 0.396f);
	[Export] InputIcon RenameInputIcon;
	[Export] InputIcon ReadyUpInputIcon;
	#endregion
	public NewCharacterSelectScreen characterSelectScreen;
	public bool isEnabled = false;
	public bool isReady = false;
	public int colorIdx = -1;

    public override void _Ready()
    {
        base._Ready();
		Disable();
    }


	void SetPlayerName(string newName)
	{
		nameLabel.Text = newName;
		Name = newName + "'s capsule";
	}
	string GetPlayerName()
	{
		return nameLabel.Text;
	}
	public void SetColor(int idx)
	{
		GD.Print("Setting color..");
		GD.Print("Current color: ", colorIdx);
		GD.Print("Setting color to: ", idx);
		if (pilotTexture.Material == null) 
			pilotTexture.Material = pilotOutlineShaderMaterial.Duplicate() as ShaderMaterial;
		int dir = 1;
		if (idx < colorIdx){dir = -1;}
		for (int i = 0; i < teamColors.Length; i++)
		{
			int currentColoridx = NormalizeIdx(idx + (i*dir), teamColors.Length);
			GD.Print("trying to set color to: ", idx);
			if (characterSelectScreen.IsColorAvaliable(currentColoridx)){
				Vector3 newColor = teamColors[currentColoridx];
				(pilotTexture.Material as ShaderMaterial).SetShaderParameter("Color", newColor);
				colorIdx = currentColoridx;
				break;
			}
		}
		GD.Print($"Color set to {colorIdx}!");
	}
	void ClearColor()
	{
		pilotTexture.Material = null;
		colorIdx = -1;
		return;
	}
	public void Enable(int inputIdx)
	{
		if(isEnabled)
			GD.PrintErr(this.Name, " is already enabled!");

		enabledCapsule.Show();
		disabledCapsule.Hide();
		isEnabled = true;
		SetColor(0);
		InputGenerator.Instance.GeneratePlayerMenuInput(inputIdx);
		SetInputIdx(inputIdx);
	}
	public void Disable()
	{
		if (!isEnabled)
			GD.PrintErr(this.Name, " is already disabled!");
		UnReady();
		ClearColor();
		disabledCapsule.Show();
		enabledCapsule.Hide();
		isEnabled = false;
		EmitSignal(SignalName.Disabled, this);
		SetInputIdx(-2);
	}

	bool ReadyUp() // Note: name can't be "Ready" cuz of Node.Ready
	{ 
		if (isReady)
			return false;
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
		pannelTheme.BgColor = unreadyBGcolor;
		menuScreenOptions.Show();
		EmitSignal(SignalName.UnReadied, this);
		return true;
	}

    public override bool OnInteract()
	{
		return ReadyUp();
	}
    public override bool OnAltInteract()
    {
		if (isReady) return false;
        return base.OnAltInteract();
    }
    public override bool OnMoveAction(Vector2 dir)
	{
		if (isReady) return false;
		if (dir.X == 0)
			return false;
		SetColor(colorIdx + (int)dir.X);
		return true;
	}

    public override bool OnNegativeAction()
    {
      	if (UnReady()) return true;
		Disable();
		return true;
	}
	public int GetInputIdx()
	{
		return inputNode.GetInputIdx();
	}
	void SetInputIdx(int newInputIdx)
	{
		inputNode.SetInputIdx(newInputIdx);
		RenameInputIcon.SetInputIdx(newInputIdx);
		ReadyUpInputIcon.SetInputIdx(newInputIdx);
	}
}
