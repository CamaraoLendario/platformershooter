using Godot;
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
		for(int i = 0; i < GetChildCount(); i++)
		{
			Control menuItem = GetChild<Control>(i);
			menuItem.Position = new Vector2(
				-menuItem.Size.X/2f,
				separation * i
			);
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
		GD.Print("menu items!!:");
		foreach(MenuItem child in menuItems)
		{
			GD.Print($"{Owner.Name}: menu Item: {child.Name}");
		}
		return menuItems;
	}
	
}
