using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class MainMenuScreenOptions : Control
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
	MenuItem[] menuItems = [];

    public override void _Ready()
	{
		ReorganizeItems();
		ChildEnteredTree += (Node node) => {ReorganizeItems();};
		ChildExitingTree += (Node node) => {ReorganizeItems();};
	}

	void ReorganizeItems()
	{
		GetMenuItems();
		if (horizontal){
			float childCount = GetChildCount();
			float offsetValue = (Separation/2f) * (childCount-1);
			offsetValue += offset;

			for(float i = 0; i < childCount; i++)
			{
				Control menuItem = GetChild<Control>((int)i);
				menuItem.Position = new Vector2(
					(separation * i) - offsetValue,
					-menuItem.Size.Y/2f
				);
			}
		}
		else
		{
			for(int i = 0; i < GetChildCount(); i++)
			{
				Control menuItem = GetChild<Control>(i);
				menuItem.Position = new Vector2(
					-menuItem.Size.X/2f,
					separation * i
				);
			}
		}	
	}

	public MenuItem[] GetMenuItems()
	{
		List<MenuItem> newMenuItems = [];

		foreach(Node child in GetChildren())
		{
			if (child is MenuItem menuItem)
				newMenuItems.Add(menuItem);
			else if (child is MainMenuScreenOptions screenOptions){
				foreach (MenuItem nestedMenuItem in screenOptions.GetMenuItems())
					newMenuItems.Add(nestedMenuItem);
			}	
		}
		menuItems = newMenuItems.ToArray();
		return menuItems;
	}
	
}
