using Godot;
using System;

[Tool]
public partial class BorderShower : ColorRect
{
	// TODO change this so it works with the player
	ShaderMaterial shaderMaterial;
    public override void _Ready()
    {
        shaderMaterial = Material as ShaderMaterial;
		
    }

    public override void _Process(double delta)
    {
        shaderMaterial.SetShaderParameter("position", Position);

		#if TOOLS
        	shaderMaterial.SetShaderParameter("rectSize", Size);
        # endif
    }

}
