using Godot;
using System.Collections.Generic;
using static SpaceMages.SpaceMagesVars;

 
public partial class CapsuleCharacterSprite : TextureRect
{
	static Texture2D[] characterSpriteSheets = [
		GD.Load<Texture2D>("uid://de15nxjq8mt4s"), // Red
		GD.Load<Texture2D>("uid://de15nxjq8mt4s"), // Purple
		GD.Load<Texture2D>("uid://de15nxjq8mt4s"), // Blue
		GD.Load<Texture2D>("uid://de15nxjq8mt4s"), // Green
		GD.Load<Texture2D>("uid://de15nxjq8mt4s"), // Yellow
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