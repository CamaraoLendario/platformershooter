using Godot;
using System;

public partial class Pickup : Area2D
{
	protected string itemName = "nameless";
	protected Color itemColor = new Color(1f, 1f, 1f, 1f);
	PackedScene notificationScene = GD.Load<PackedScene>("uid://cfs2cqcfhf2js");

    protected void SummonNotification(Player player)
	{
		ItemPickupNotification notification = notificationScene.Instantiate<ItemPickupNotification>();
		notification.Setup(itemName, itemColor);

		notification.Position = player.Position;
		Game.Instance.world.AddChild(notification);
	}

}
