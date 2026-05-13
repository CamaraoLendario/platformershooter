using Godot;
using System;
using static SpaceMages.SpaceMagesVars;

public partial class TeamSelectScreenPlayerIcon : MenuItem
{
	[Signal] public delegate void moveLeftEventHandler(Vector2 dir, TeamSelectScreenPlayerIcon icon);
	[Signal] public delegate void moveRightEventHandler(Vector2 dir, TeamSelectScreenPlayerIcon icon);
	[Export] StyleBoxFlat panelStyleBox;
	[Export] TextureRect pilotStill;
	[Export] Label nameLabel;
	[Export] PlayerMenuInput inputNode;

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
	void MoveRight()
	{
		EmitSignal(SignalName.moveRight, Vector2.Right, this);
	}
	void MoveLeft()
	{
		EmitSignal(SignalName.moveLeft, Vector2.Left, this);
	}
	void SetInputIdx(int inputIdx)
	{
		inputNode.SetInputIdx(inputIdx);
	}
}
