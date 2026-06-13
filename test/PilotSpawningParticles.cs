using Godot;
using System;

[Tool]
public partial class PilotSpawningParticles : GpuParticles2D
{
	AnimatedSprite2D sprite;
    [ExportToolButton("Click me!")]
    public Callable ClickMeButton => Callable.From(ClickMe);

    public void ClickMe()
    {
		ShaderMaterial material = ProcessMaterial as ShaderMaterial;
		AtlasTexture spriteAtlas = sprite.SpriteFrames.GetFrameTexture(sprite.Animation, sprite.Frame) as AtlasTexture;
		Vector2 textureSize = spriteAtlas.Atlas.GetSize();
		Vector2 atlasSize = spriteAtlas.Region.Size;
		Vector2 atlasPosition = spriteAtlas.Region.Position;
		
        material.SetShaderParameter("mainTexture", spriteAtlas);
		material.SetShaderParameter("sheetSize", textureSize/ atlasSize);
		material.SetShaderParameter("frame", (int)((atlasPosition.X/atlasSize.X) + ((atlasPosition.Y/atlasSize.Y)*(atlasSize.X/textureSize.X))));
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetChild<AnimatedSprite2D>(0);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
