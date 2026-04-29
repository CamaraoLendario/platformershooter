using Godot;
using System.Collections.Generic;
using System.ComponentModel;
using static SpaceMages.SpaceMagesVars;
[Tool]
public partial class HorizontalTextAndText : HorizontalMenu
{
	TextOptions TextOptions;

    public override void _Ready()
    {
		base._Ready();
		TextOptions = GetNode<TextOptions>("textOptions");

		CallDeferred(MethodName.DoFormatting);
		originalPosition = Position;
    }

    public override bool OnInteract()
    {
		return TextOptions.OnInteract();
    }
	protected override void DoFormatting()
	{
		base.DoFormatting();

		if (TextOptions == null){
			TextOptions = GetNode<TextOptions>("textOptions");
		}

		Vector2 BordedTotalSize = TotalSize - (Vector2.Right * borderSize);
		
		TextOptions.Size = new Vector2(leftLabel.Size.X, BordedTotalSize.Y);
		TextOptions.Position = (BordedTotalSize * new Vector2(0.5f, -0.5f)) - TextOptions.Size * Vector2.Right;
	}
}
