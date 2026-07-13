using Godot;
using System;
using static SpaceMages.SpaceMagesVars;

public partial class TeamSelectScreenPlayerIcon : MenuItem
{
	[Signal] public delegate void moveEventHandler(Vector2 dir, TeamSelectScreenPlayerIcon icon);
	[Export] StyleBoxFlat panelStyleBox;
	[Export] TextureRect pilotStill;
	[Export] Label nameLabel;
	[Export] PlayerMenuInput inputNode;
	public int currentTeam = -1;

    public override void _Ready()
	{
		inputNode.Move += (Vector2 dir) => {
			if (dir.X > 0){
				MoveRight();
				return true;
			}
			else MoveLeft();
			return true;
		};
		inputNode.isEnabled = true;
	}
	public void Setup(string playerName, int inpuIdx, int colorIdx)
	{
		nameLabel.Text = playerName;
		
		SetInputIdx(inpuIdx);
		SetPilotColor(colorIdx);
		BorderColor(colorIdx);
	}
	public void BorderColor(int colorIdx)
	{
		panelStyleBox.BorderColor = new Color(
			teamColors[colorIdx].X,
			teamColors[colorIdx].Y,
			teamColors[colorIdx].Z
		);
	}
	public void SetPilotColor(int colorIdx)
	{
		(pilotStill.Material as ShaderMaterial).SetShaderParameter("Color", teamColors[colorIdx]);
	}
	public void MoveRight()
	{
		EmitSignal(SignalName.move, Vector2.Right, this);
	}
	public void MoveLeft()
	{
		EmitSignal(SignalName.move, Vector2.Left, this);
	}

	void SetInputIdx(int inputIdx)
	{
		inputNode.SetInputIdx(inputIdx);
	}
	public int GetInputIdx()
	{
		return inputNode.GetInputIdx();
	}
}
