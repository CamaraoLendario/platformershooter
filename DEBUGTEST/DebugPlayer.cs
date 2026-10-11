using Godot;
using System;
using System.Linq;

public partial class DebugPlayer : Player
{
    public override void _Ready()
    {
        base._Ready();
		Game.Instance.players = [this];
    }

}
