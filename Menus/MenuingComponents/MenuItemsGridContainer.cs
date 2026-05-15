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
	[Export] Vector2 padding = Vector2.Zero;
	[Export] bool centered = false;

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
			Vector2 itemSize = menuItem.Size + padding;
			menuItem.Position = nextPos;
			TotalSizeX = Mathf.Max(TotalSizeX, menuItem.Position.X + itemSize.X);
			TotalSizeY = Mathf.Max(TotalSizeY, menuItem.Position.Y + itemSize.Y);

			if ((itemSize * growDir).LengthSquared() > maxOffset.LengthSquared()){
                maxOffset = itemSize * growDir;
            }
			
			if(i % limiter == limiter - 1) {
				nextPos += maxOffset;
				nextPos *= growDir;
				maxOffset = Vector2.Zero;
			}
			else {
				nextPos += itemSize * altGrowDir;
			}
		}
		Vector2 totalSize = new Vector2(TotalSizeX, TotalSizeY) - padding;
		if (!centered) Size = totalSize;
		else Size *= 0;

		for (int i = 0; i < controlNodes.Length; i++)
		{
			Control menuItem = controlNodes[i];
			menuItem.Position -= totalSize/2;
		}

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
