using Godot;
using System;

[Tool]
public partial class MasterVolumeSlider : HorizontalTextAndSlider
{
    public override bool OnMoveAction(Vector2 dir)
    {
        if(!base.OnMoveAction(dir)) return false;

        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Master"),(float)SliderValue / (float)maxValue);
        
		return true;
    }

}
