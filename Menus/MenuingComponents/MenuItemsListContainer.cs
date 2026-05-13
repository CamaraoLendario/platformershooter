using Godot;
using System;
using System.ComponentModel;
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
	[Export] bool horizontal = false;

	protected override void ReorganizeItems()
	{
		if (!Reorganizes) return;
		GetMenuItems();
		float childCount = GetChildCount();
		if (horizontal){
			float totalSeparation = Separation * (childCount-1);
			float accomulatedSize = 0;
			for(float i = 0; i < childCount; i++)
			{
				Control node = GetChild<Control>((int)i);
				node.Position = new Vector2(
					(separation * i) + accomulatedSize,
					-node.Size.Y/2f
				);
				accomulatedSize += node.Size.X;
			}
			for(int i = 0; i < childCount; i++)
			{
				Control node = GetChild<Control>(i);
				node.Position -= Vector2.Right * (((totalSeparation + accomulatedSize)/2) + offset);
				if (node is MenuItem menuItem) menuItem.SetOriginalPosition();
			}
		}
		else
		{
			float totalSeparation = Separation * (childCount-1);
			float accomulatedSize = 0;
			for(int i = 0; i < childCount; i++)
			{
				Control Node = GetChild<Control>(i);
				Node.Position = new Vector2(
					-Node.Size.X/2f,
					separation * i + accomulatedSize
				);
				accomulatedSize += Node.Size.Y;
			}
			for(int i = 0; i < childCount; i++)
			{
				Control Node = GetChild<Control>(i);
				Node.Position -= Vector2.Down * (((totalSeparation + accomulatedSize)/2) + offset);
				if (Node is MenuItem menuItem) menuItem.SetOriginalPosition();
			}
		}	
	}


	
}
