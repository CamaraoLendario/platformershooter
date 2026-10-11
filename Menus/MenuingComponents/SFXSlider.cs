using Godot;
using System;

[Tool]
public partial class SFXSlider : HorizontalTextAndSlider
{
    public override bool OnMoveAction(Vector2 dir)
    {
        if(!base.OnMoveAction(dir)) return false;

        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("SFX"), ((float)SliderValue / (float)maxValue) * Game.Instance.masterVolume);
		return true;
    }
}
