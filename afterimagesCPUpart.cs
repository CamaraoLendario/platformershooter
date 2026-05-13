using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

[Tool]
public partial class afterimagesCPUpart : TextureRect
{
	[Export] int framesDelay = 1;
	[Export] int ImagesCount
	{
		get
		{
			return imagesCount;
		}
		set
		{
			imagesCount = value;
		}
	}
	int imagesCount = 5;
	ShaderMaterial afterShader;
	Vector2[] positions = new Vector2[5];
	Vector2 oldPos;
	float oldScale = 1;
    public override void _Ready()
    {
        base._Ready();
		afterShader = Material as ShaderMaterial;
		afterShader.SetShaderParameter("texSize", Texture.GetSize());
		oldPos = Position;
		for(int i = 0 ; i < positions.Length; i++)
			positions[i] = Vector2.Zero;
	}

	int frameNum = 0;
    public override void _Process(double delta)
    {
		if (frameNum >= framesDelay) frameNum = 0;
		for (int i = 0; i < positions.Length-1; i++){
			if (frameNum != 0){
				positions[i] -= oldPos - Position;
			}
			else positions[i] = positions[i+1] - (oldPos - Position);
		}
		
		if (frameNum == 0) positions[4] = Position-oldPos;
		
		Vector2 farthestPos = Vector2.Zero;
		for (int i = 0; i < positions.Length-1; i++){
			if(farthestPos.LengthSquared() < positions[i].LengthSquared()){
				farthestPos = positions[i];
			}
		}
		
		Vector2[] UVPositions = new Vector2[positions.Length];
		for (int i = 0; i < positions.Length-1; i++){
			UVPositions[i] = positions[i] / farthestPos.Length();
		}
		

		frameNum += 1;
		oldPos = Position;
		afterShader.SetShaderParameter("scale", farthestPos.Length()/(Texture.GetSize().X/4f));
		afterShader.SetShaderParameter("afterImagePos", UVPositions);
    }


}
