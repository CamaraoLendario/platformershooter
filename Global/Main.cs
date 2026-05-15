using Godot;
using System.Collections.Generic;

public partial class Main : Node
{
    public override void _Ready()
    {
        Game.Instance.main = this;
    }

    public void StartGame()
	{
		Game.Instance.mainMenu.QueueFree();
        PackedScene overWorld = GD.Load<PackedScene>("uid://blk3xlkdst7il");
        AddChild(overWorld.Instantiate<OverWorld>());

        CallDeferred(MethodName.EmitGameStart);
	}
    void EmitGameStart()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameStarted);
    }
}
