using Godot;
using System.Collections.Generic;
using static SpaceMages.SpaceMagesVars;

 
public partial class CapsuleCharacterSprite : TextureRect
{
	static Texture2D[] characterSpriteSheets = [
		GD.Load<Texture2D>("uid://cnhqpyavnjoy3"), // Red
		GD.Load<Texture2D>("uid://bj0hyauepqdma"), // Purple
		GD.Load<Texture2D>("uid://cbrfa11pgicrc"), // Blue
		GD.Load<Texture2D>("uid://bwklvmphetcaq"), // Green
		GD.Load<Texture2D>("uid://oouemkrjk5kp"), // Yellow
		GD.Load<Texture2D>("uid://nlcey2h7ycho"), // Orange "Dinoxaurr"
	];

	public void SetTexture(ColorNames colorName)
	{
		SetTexture((int)colorName);
	}
	public void SetTexture(int colorName)
	{
		Texture = GetStatic(colorName);
	}
	
	static Texture2D GetStatic(ColorNames colorName)
	{
		return GetStatic((int)colorName);
	}
	static Texture2D GetStatic(int colorName)
	{
		return characterSpriteSheets[colorName];
	}
}