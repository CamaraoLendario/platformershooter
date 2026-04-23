using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class HorizontalTextAndSlider : HorizontalMenu
{
	ProgressBar slider;
	[Export] int SliderValue
	{
		get
		{
			return sliderValue;
		}
		set
		{
			sliderValue = value;
			if (slider != null) slider.Value = (float) sliderValue / (float) maxValue;
		}
	}
	int sliderValue;
	[Export] int maxValue = 100;
	public override void _Ready()
    {
		base._Ready();
		slider = GetNode<ProgressBar>("Slider");
		DoFormatting();
    }

    public override bool OnMoveAction(Vector2 dir)
	{
		if (dir.X == 0) return false;
		
		GD.Print(dir);
		SliderValue += (int)dir.X;
		return true;
	}

    protected override void DoFormatting()
    {
        base.DoFormatting();
		Vector2 BordedTotalSize = TotalSize - (new Vector2(1, 1.5f) * borderSize);

		slider.Position = BordedTotalSize * new Vector2(0, -0.5f) + new Vector2(borderSize/2, 0);
		slider.Size = (BordedTotalSize * new Vector2(0.5f, 1)) - new Vector2(borderSize/2, 0); 
    }
}
