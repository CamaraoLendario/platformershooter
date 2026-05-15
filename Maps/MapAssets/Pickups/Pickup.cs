using Godot;
using System;

public partial class Pickup : Area2D
{
	protected string itemName = "nameless";
	protected Color itemColor = new Color(1f, 1f, 1f, 1f);
	PackedScene notificationScene = GD.Load<PackedScene>("uid://cfs2cqcfhf2js");
	World world;
    public override void _Ready()
    {
        world = GetTree().GetFirstNodeInGroup("World") as World;
    }

    protected void SummonNotification(Player player)
	{
		ItemPickupNotification notification = notificationScene.Instantiate<ItemPickupNotification>();
		notification.Setup(itemName, itemColor);

		notification.Position = player.Position;
		world = GetTree().GetFirstNodeInGroup("World") as World;
		world.AddChild(notification);
	}
}
