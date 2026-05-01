using Godot;
using System.Collections.Generic;
using System;

[Tool]
public partial class MenuItemsContainer : Control
{
	
	[Export] protected bool Reorganizes = true;
	protected MenuItem[] menuItems = [];
	public override void _Ready()
	{
		CallDeferred(MethodName.ReorganizeItems);
		ChildEnteredTree += (Node node) => {ReorganizeItems();};
		ChildExitingTree += (Node node) => {ReorganizeItems();};
		ChildOrderChanged += ReorganizeItems;
	}
	protected virtual void ReorganizeItems(){GD.PrintErr($"{this.Name}: ReorganizeItems not implemented");}
	public MenuItem[] GetMenuItems()
	{
		List<MenuItem> newMenuItems = [];

		foreach(Node child in GetChildren())
		{
			if (child is MenuItem menuItem)
				newMenuItems.Add(menuItem);
			else if (child is MenuItemsListContainer screenOptions){
				foreach (MenuItem nestedMenuItem in screenOptions.GetMenuItems())
					newMenuItems.Add(nestedMenuItem);
			}	
		}
		menuItems = newMenuItems.ToArray();
		return menuItems;
	}
}
