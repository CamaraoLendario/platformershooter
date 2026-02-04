using Godot;
using System;

public partial class PlayerSprite : AnimatedSprite2D
{
	[Export] protected Player Main;
	protected float turnSpeed = 0.3f;
}
