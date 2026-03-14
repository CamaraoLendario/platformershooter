using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class PlayerDebugComponent : Controller
{
    [Export] Label debugInfo;
	List<string> debugInputs = new List<string>()
	{
		"DEBUGMENU",
		"NOCLIP",
		"CHANGESHIELDSTATE",
		"CHANGEPILOTSHIPSTATE",
        "GODMODE",
	};
    public override void _Ready()
    {
        base._Ready();
        PrepareForGameOverride();
    }

    void PrepareForGameOverride()
    {
        Main.inputIdx = -1;
        Main.isKeyboardControlled = true;
        Main.playerInput.keyboardKeyword = "Keyboard";
        Main.SetColor(0);
        
        Game.Instance.playerNodesByColor.Add(Main.colorIdx, Main);
		Game.Instance.playerNodesByInputIdx.Add(Main.inputIdx, Main);
        Game.Instance.alivePlayerCount += 1;
		InputGenerator.Instance.GeneratePlayersInput(Main.inputIdx);
    }

    public override void ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);
        Main.Position += inputVector * 300 * (float)delta;
    }

    void DEBUGMENUStart()
    {

    }    
   
    void NOCLIPStart()
    {
        if (Main.currentController != this)
            Main.currentController = this;
        else
        {
            if (Main.IsInPilotArea) {
                Main.currentController = Main.pilot;
            }
            else {
                Main.currentController = Main.ship;
            }
        }
        Main.Velocity *= 0;
    }
  
    void CHANGESHIELDSTATEStart()
    {
        if (Main.HasShield)
        {
            Main.TakeDamage();
        }
        else
        {
            Main.HasShield = true;
            Main.shieldCooldownTimer.Stop();
            Main.pilotShieldFlickerer.Stop();
            Main.shipShieldFlickerer.Stop();
            Main.pilotShield.Modulate = new Color(1, 1, 1, 1);
            Main.shipShield.Modulate = new Color(1, 1, 1, 1);
        }
    }

    void CHANGEPILOTSHIPSTATEStart()
    {
        if (Main.IsInPilotArea) return;
        
        if (Main.isPilot)
        {
           Main.TryGoShip(true);
        }
        else
        {
            Main.GoPilot();
        }
    }

    void GODMODEStart()
    {
        Main.godMode = !Main.godMode;
        if(Main.godMode) debugInfo.Text ="GodMode";
        else debugInfo.Text = "";
    }

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseMotion) return;
        base._Input(@event);

		foreach(string input in debugInputs)
		{
			int inputIndex = GetParent<Player>().inputIdx;
			if (Input.IsActionJustPressed(input + "Keyboard" + inputIndex))
			{
                Call(input + "Start");
			}
		}
    }

    public override void ProcessWASD(float X, float Y)
    {
        inputVector = new Vector2(X, Y);
    }

}
