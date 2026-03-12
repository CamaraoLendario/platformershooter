using Godot;
using System;

public partial class MasterPreloader : Node
{
    MasterPreloader()
	{	
		AddChild(GD.Load<PackedScene>("uid://cgs51e0ovys3i").Instantiate<Node>());	// Object Pool
		AddChild(GD.Load<PackedScene>("uid://duaoec2no4b7m").Instantiate<ShaderPreloader>());	// Shader Preloader
	}

}