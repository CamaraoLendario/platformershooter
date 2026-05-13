using Godot;
using System.Collections.Generic;
using System;

[Tool]
public partial class MenuItemsContainer : Control
{
	
	[Export] protected bool Reorganizes = true;
	[Export] bool childrenAreScreenAnimated = true;
	public MenuItem[] menuItems = [];
	public override void _Ready()
	{
		CallDeferred(MethodName.ReorganizeItems);
		ChildEnteredTree += ReorganizeItems;
		ChildExitingTree += ReorganizeItems;
		ChildOrderChanged += ReorganizeItems;
	}
	protected virtual void ReorganizeItems(){GD.PrintErr($"{this.Name}: ReorganizeItems not implemented");}
	public MenuItem[] GetMenuItems()
	{
		List<MenuItem> newMenuItems = [];

		if (!childrenAreScreenAnimated) return newMenuItems.ToArray();

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

	public void QueueMassAddChildren(Node[] Children)
	{
		MassAddChildren(Children);
	}
	public void MassAddChildren(Node[] Children)
	{
		ChildEnteredTree -= ReorganizeItems;
		foreach(Node child in Children){
			AddChild(child);
		}
		ReorganizeItems();
		ChildEnteredTree += ReorganizeItems;
	}

	void ReorganizeItems(Node node)
	{
		ReorganizeItems();
	}
}
