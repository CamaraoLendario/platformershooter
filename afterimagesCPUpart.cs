using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO optimize information transfered to the GPU there's a lot of optimization to be done here
[Tool]
public partial class afterimagesCPUpart : ColorRect
{
	[Export] bool enabled = false;
	[Export] int framesDelay = 1;
	ShaderMaterial afterShader;
	Vector2[] positions = new Vector2[5];
	float[] rotations = new float[5];
	int[] frames = new int[5];
	Array<bool> isFrameFlipped = [false, false, false, false, false];
	Vector2 oldPos;
	float oldScale = 1;
	AnimatedSprite2D sprite;
	AtlasTexture spriteAtlasTex;
	Vector2 spriteSheetSize;
	Vector2 sheetSize;
	int frameNum = 0;
	Timer disableTimer = new Timer(){OneShot = true};
    public override void _Ready()
    {
        base._Ready();

		AddChild(disableTimer);
		disableTimer.Timeout += Disable;

		var parent = GetParent();
		
		if (parent is not AnimatedSprite2D)
		{
			GD.PrintErr("ERROR: after images parent is not AnimatedSprite2D");
			return;
		}
		
		sprite = parent as AnimatedSprite2D;
		spriteAtlasTex = sprite.SpriteFrames.GetFrameTexture(sprite.Animation, sprite.Frame) as AtlasTexture;
		spriteSheetSize = spriteAtlasTex.Atlas.GetSize();
		sheetSize = spriteSheetSize / spriteAtlasTex.Region.Size;


		afterShader = Material as ShaderMaterial;
		afterShader.SetShaderParameter("texSize", spriteAtlasTex.Region.Size);
		afterShader.SetShaderParameter("textureSheet", spriteAtlasTex.Atlas);
		afterShader.SetShaderParameter("sheetSize", sheetSize);
		
		oldPos = sprite.GlobalPosition;
		for(int i = 0 ; i < positions.Length; i++)
			positions[i] = Vector2.Zero;
	}
	 
    public override void _Process(double delta)
    {
		if (frameNum >= framesDelay) {
			spriteAtlasTex = sprite.SpriteFrames.GetFrameTexture(sprite.Animation, sprite.Frame) as AtlasTexture;
			frameNum = 0;
		};
	
		if (frameNum != 0)
			for (int i = 0; i < positions.Length-1; i++)
			{
				positions[i] -= oldPos - sprite.GlobalPosition;
			}
		else
			for (int i = 0; i < positions.Length-1; i++)
			{
				positions[i] = positions[i+1] - (oldPos - sprite.GlobalPosition);
				rotations[i] = rotations[i+1];
				frames[i] = frames[i+1];
				isFrameFlipped[i] = isFrameFlipped[i+1];
		}

		if(frameNum == 0) {
			Vector2 framePos = spriteAtlasTex.Region.Position / spriteAtlasTex.Region.Size;
			int spriteFrame = (int)((framePos.Y * sheetSize.X) + framePos.X);
			if (enabled){
				positions[4] = Vector2.Zero;
				rotations[4] = sprite.Rotation; 
				frames[4] = spriteFrame;
				isFrameFlipped[4] =  sprite.FlipH;
			}
			else
			{
				positions[4] = Vector2.One * 10000;
			}
		}
		Rotation = -sprite.Rotation;
		
		Vector2 farthestPos = Vector2.Zero;
		for (int i = 0; i < positions.Length-1; i++){
			if(farthestPos.LengthSquared() < positions[i].LengthSquared()){
				farthestPos = positions[i];
			}
		}

		frameNum += 1;
		oldPos = sprite.GlobalPosition;
		Vector2 texSize = spriteAtlasTex.Region.Size;
		if (enabled) afterShader.SetShaderParameter("scale", farthestPos.Length()/(Mathf.Min(texSize.X, texSize.Y)/4f));
		afterShader.SetShaderParameter("pixelsOffset", positions);
		afterShader.SetShaderParameter("rotations", rotations);
		afterShader.SetShaderParameter("frames", frames);
		afterShader.SetShaderParameter("flip", isFrameFlipped);
    }

	public void EnableForTime(float seconds)
	{
		Enable();
		disableTimer.Start(seconds);
	}
	public void Enable()
	{
		enabled = true;
	}
	public void Disable()
	{
		enabled = false;
		disableTimer.Stop();
	}
}
