using Godot;
using System;

public partial class HorizontalMenu : MenuItem
{
	[Export] public string SettingName
	{
		get
		{
			return settingName;
		}
		set
		{
			settingName = value;
			if (leftLabel != null) leftLabel.Text = $"{value}:";
		}
	}
	string settingName;
	[Export] protected Vector2 TotalSize
	{
		get
		{
			return totalSize;
		}
		set
		{
			totalSize = value;
			if (Engine.IsEditorHint())
			{
				CallDeferred(MethodName.DoFormatting);
			}
		}
	}
	Vector2 totalSize = new Vector2(
		1720.0f,
		165.0f
	);
	[Export] protected float borderSize = 50;
	protected Label leftLabel = null;
    public override void _Ready()
    {
        base._Ready();
		leftLabel = GetNode<Label>("leftLabel");
		panelHelper.Position = -totalSize/2f;
		panelHelper.Size = totalSize;
		leftLabel.Text = $"{SettingName}:";
    }
	protected virtual void DoFormatting()
	{
		if (leftLabel == null) leftLabel = GetNode<Label>("leftLabel");
		panelHelper.Position = -TotalSize/2f;
		panelHelper.Size = TotalSize;
		Vector2 BordedTotalSize = TotalSize - (Vector2.Right * borderSize);
		
		leftLabel.Size = new Vector2(leftLabel.Size.X, BordedTotalSize.Y);
		leftLabel.Position = BordedTotalSize * new Vector2(-0.5f, -0.5f);
	}
}
