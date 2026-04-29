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
	[Export] MainMenuScreenOptions menuScreenOptions;
	[Export] Label nameLabel;
	[Export] TextureRect pilotTexture;
	[Export] Control enabledCapsule;
	[Export] Control disabledCapsule;
	[Export] public PlayerMenuInput menuInput;
	[Export] ShaderMaterial pilotOutlineShaderMaterial;
	[Export] StyleBoxFlat pannelTheme;
	[Export] Color readyBGcolor = new Color(0.161f, 1.0f, 0.161f, 0.396f);
	[Export] Color unreadyBGcolor = new Color(0.161f, 0.161f, 0.161f, 0.396f);
	#endregion
	public NewCharacterSelectScreen characterSelectScreen;
	public bool isEnabled = false;
	public bool isReady = false;
	public int colorIdx = -1;

	void SetPlayerName(string newName)
	{
		nameLabel.Text = newName;
		Name = newName + "'s capsule";
	}
	string GetPlayerName()
	{
		return nameLabel.Text;
	}
	void SetColor(int idx)
	{
		if (idx < 0)
		{
			pilotTexture.Material = null;
			colorIdx = -1;
			return;
		}
		if (pilotTexture.Material == null) 
			pilotTexture.Material = pilotOutlineShaderMaterial.Duplicate() as ShaderMaterial;
		int dir = 1;
		if (idx < colorIdx){dir = -1;}
		for (int i = 0; i < teamColors.Length; i++)
		{
			int currentColoridx = NormalizeIdx(idx + (i*dir), teamColors.Length);
			if (characterSelectScreen.IsColorAvaliable(currentColoridx)){
				Vector3 newColor = teamColors[currentColoridx];
				(pilotTexture.Material as ShaderMaterial).SetShaderParameter("Color", newColor);
			}
		}
		colorIdx = idx;
	}
	public void Enable(int inputIdx)
	{
		if(isEnabled)
			GD.PrintErr(this.Name, " is already enabled!");

		enabledCapsule.Show();
		disabledCapsule.Hide();
		isEnabled = true;
		SetColor(0);
		menuInput.inputIdx = inputIdx;
	}
	void Disable()
	{
		if (!isEnabled)
			GD.PrintErr(this.Name, " is already disabled!");

		SetColor(-1);
		if(!Engine.IsEditorHint()) InputGenerator.Instance.ClearMenuInput(menuInput.inputIdx);
		disabledCapsule.Show();
		enabledCapsule.Hide();
		isEnabled = false;
		menuInput.inputIdx = -2;
		EmitSignal(SignalName.Disabled, this);
	}

	bool ReadyUp() // Note: name can't be "Ready" cuz of Node.Ready
	{ 
		if (!isReady){
			GD.PrintErr(this.Name, " is already ready!");
			return false;
		}
		isReady = true;
		pannelTheme.BgColor = readyBGcolor;
		menuScreenOptions.Hide();
		EmitSignal(SignalName.Readied, this);

		return true;
	}
	void UnReady()
	{
		if (!isReady){
			GD.PrintErr(this.Name, " is already NOT ready!");
			return;
		}
		isReady = false;
		pannelTheme.BgColor = unreadyBGcolor;
		EmitSignal(SignalName.UnReadied, this);
	}

    public override bool OnInteract()
	{
		return ReadyUp();
	}

    public override bool OnNegativeAction()
    {
        if (isReady)
			UnReady();
		else
		{
			//goback to gamemode select screen
		}
		return true;	
	}

}
