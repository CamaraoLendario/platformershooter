using Godot;
using System;
[Tool]
public partial class MapWatcher : Camera2D
{
	[Export] TextureRect texrect;
	SubViewport subViewport;
	public override void _Ready() {
		Enabled = true;
		subViewport = GetParent<SubViewport>();
		subViewport.World2D = GetViewport().World2D;
		GD.Print(subViewport.Size);
GD.Print(subViewport.World2D);
GD.Print(subViewport.GetTexture().GetSize());
	}

    public override void _Process(double delta)
	{
		(texrect.Material as ShaderMaterial).SetShaderParameter("tex", subViewport.GetTexture());
	}

}
