using Godot;
using System;
using System.Linq;

[Tool]
public partial class MenuItemsListContainer : MenuItemsContainer
{
	[Export] int Separation
	{
		get
		{
			return separation;
		}
		set
		{
			separation = value;
			ReorganizeItems();
		}
	}
	int separation = 120;
	[Export] float Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
			ReorganizeItems();
		}
	}
	float offset = 0;
	[Export] bool horizontal;

	protected override void ReorganizeItems()
	{
		if (!Reorganizes) return;
		GetMenuItems();
		if (horizontal){
			float childCount = GetChildCount();
			float offsetValue = (Separation/2f) * (childCount-1);
			offsetValue += offset;

			for(float i = 0; i < childCount; i++)
			{
				Control node = GetChild<Control>((int)i);
				node.Position = new Vector2(
					(separation * i) - offsetValue,
					-node.Size.Y/2f
				);
				if (node is MenuItem menuItem) menuItem.SetOriginalPosition();
			}
		}
		else
		{
			for(int i = 0; i < GetChildCount(); i++)
			{
				Control Node = GetChild<Control>(i);
				Node.Position = new Vector2(
					-Node.Size.X/2f,
					separation * i
				);
				if (Node is MenuItem menuItem) menuItem.SetOriginalPosition();
			}
		}	
	}


	
}
