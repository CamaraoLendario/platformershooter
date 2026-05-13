using Godot;
using System;
using System.Linq;

[Tool]
public partial class MenuItemsGridContainer : MenuItemsContainer
{
	[ExportToolButton("Reorganize")]
	public Callable ReorganizeButton => Callable.From(ReorganizeItems);
	[Export] int lineLength = 4;
	[Export] int columnLength = 4;
	[Export] bool usesColumn = false;

    /* public override void _Ready()
	{
		CallDeferred(MethodName.ReorganizeItems);
	} */

	protected override void ReorganizeItems()
	{

		Control[] controlNodes = GetControlNodes();
		Vector2 growDir = Vector2.Down;
		Vector2 altGrowDir = Vector2.Right;
		int limiter = lineLength;
		if (usesColumn) {
			growDir = Vector2.Right;
			altGrowDir = Vector2.Down;
			limiter = columnLength;
		}

		Vector2 nextPos = Vector2.Zero;
		Vector2 maxOffset = Vector2.Zero;
		float TotalSizeX = 0, TotalSizeY = 0;
		for (int i = 0; i < controlNodes.Length; i++)
		{
			Control menuItem = controlNodes[i];
			menuItem.Position = nextPos;
			TotalSizeX = Mathf.Max(TotalSizeX, menuItem.Position.X + menuItem.Size.X);
			TotalSizeY = Mathf.Max(TotalSizeY, menuItem.Position.Y + menuItem.Size.Y);

			if ((menuItem.Size * growDir).LengthSquared() > maxOffset.LengthSquared()){
                maxOffset = menuItem.Size * growDir;
            }
			

			if(i%limiter == limiter-1){
				nextPos += maxOffset;
				nextPos *= growDir;
				maxOffset = Vector2.Zero;
			}
			else {
				nextPos += menuItem.Size * altGrowDir;
			}
		}
		Vector2 totalSize = new Vector2(TotalSizeX, TotalSizeY);
		Size = totalSize;
	}

	Control[] GetControlNodes()
	{
		Control[] controlNodes = [];
		foreach(Node child in GetChildren()){
			if (child is Control controlNode){
				controlNodes = controlNodes.Append(controlNode).ToArray();
			}
		}
		return controlNodes;
	}
}
