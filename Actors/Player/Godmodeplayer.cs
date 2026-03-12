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
    public override void _Ready()
    {
        base._Ready();
		ConnectDebugSignals();
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
		godInput.CHANGEPILOTSHIPSTATEStart += CHANGEPILOTSHIPSTATEStart;
		godInput.CHANGEPILOTSHIPSTATEEnd += CHANGEPILOTSHIPSTATEEnd;

		
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
	
	void CHANGEPILOTSHIPSTATEStart()
	{

	}
	
	void CHANGEPILOTSHIPSTATEEnd()
	{

	}
}
