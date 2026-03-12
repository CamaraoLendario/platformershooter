using Godot;
using System;

public partial class Godmodeplayer : Player
{
    /* 
        Get Pilot/ship
        Get Shield
        NOCLIP
        get weapon/item menu
        Think of more stuff cuz surely theres more
    */
	Vector2 wishDir = Vector2.Zero;
	bool isNOCLIP = true;
	float NoClipSpeedMultiplier = 1;

    public override void _Ready()
    {
		SetMenuSetVariables();
		ConnectDebugSignals();
        base._Ready();
	}

	void SetMenuSetVariables()
	{
		inputIdx = -1;
		isKeyboardControlled = true;
		playerInput.keyboardKeyword = "Keyboard";
		SetColor(0);

		Game.Instance.AddPlayer(this);
		InputGenerator.Instance.GeneratePlayersInput(inputIdx);
	}

	void ConnectDebugSignals()
	{
		GODMODEPlayerInput godInput = playerInput as GODMODEPlayerInput;
		godInput.DEBUGMENUStart += OnDEBUGMENUStart;
		godInput.DEBUGMENUEnd += OnDEBUGMENUEnd;
		godInput.NOCLIPStart += OnNOCLIPStart;
		godInput.NOCLIPEnd += OnNOCLIPEnd;
		godInput.CHANGESHIELDSTATEStart += OnCHANGESHIELDSTATEStart;
		godInput.CHANGESHIELDSTATEEnd += OnCHANGESHIELDSTATEEnd;
		godInput.CHANGEPILOTSHIPSTATEStart += OnCHANGEPILOTSHIPSTATEStart;
		godInput.CHANGEPILOTSHIPSTATEEnd += OnCHANGEPILOTSHIPSTATEEnd;
		godInput.InputDirChanged += OnInputDirChanged;
		
	}

    public override void _PhysicsProcess(double delta)
    {
		if (!isNOCLIP)
		{
			base._PhysicsProcess(delta);
			return;
		}

		Position += wishDir * 300 * (float)delta * NoClipSpeedMultiplier;
	}	


	void OnNOCLIPStart()
	{

	}
	
	void OnNOCLIPEnd()
	{

	}
	
	void OnDEBUGMENUStart()
	{

	}
	
	void OnDEBUGMENUEnd()
	{

	}
	
	
	void OnCHANGESHIELDSTATEStart()
	{

	}
	
	void OnCHANGESHIELDSTATEEnd()
	{

	}
	
	void OnCHANGEPILOTSHIPSTATEStart()
	{

	}
	
	void OnCHANGEPILOTSHIPSTATEEnd()
	{

	}

	void OnInputDirChanged(float X, float Y)
	{
		wishDir = new Vector2(X, Y);
		GD.Print(wishDir);
	}
}
