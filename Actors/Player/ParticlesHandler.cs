using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using static SpaceMages.SpaceMagesVars;
public partial class ParticlesHandler : Node2D
{
	
	[Export] public GpuParticles2D shipExplosionParticles;
	[Export] public GpuParticles2D shipReconstructionParticles;
	public GpuParticles2D currentShipReconstructionParticles;
	[Export] public GpuParticles2D pilotDeathParticles;
	[Export] public GpuParticles2D pilotShieldBreak;
	[Export] GpuParticles2D shipShieldBreak;
	[Export] GpuParticles2D spawningParticles;
	Player main;
	Timer startAssemblingShipTimer = new Timer() {OneShot = true};
	Timer cooldownStartAssemblingShipTimer = new Timer() {OneShot = true};
	
	bool isTurningToShip = false;
	Vector2 playerPosition
	{
		get
		{
			return main.Position;
		}
		set
		{
			return;
		}
	}
    public override void _Ready()
    {
        base._Ready();
		main = Owner as Player;
		main.WentPilotArea += CancelTurningShip;
		main.WentShipArea += () => startAssemblingShipTimer.Start(Player.TIMETOSHIP - shipReconstructionParticles.Lifetime);
		AddChild(startAssemblingShipTimer);
		AddChild(cooldownStartAssemblingShipTimer);	
		main.LostShip += () =>
		{
			ExplodeShip();
			cooldownStartAssemblingShipTimer.Start(Player.SHIPCOOLDOWNTIME - shipReconstructionParticles.Lifetime);
		};
		startAssemblingShipTimer.Timeout += PlayParticlesForTryGoShip;
		cooldownStartAssemblingShipTimer.Timeout += PlayParticlesForTryGoShip;
		main.Reseting += OnReseting;
		pilotShieldBreak.ProcessMaterial = pilotShieldBreak.ProcessMaterial.Duplicate(false) as ShaderMaterial;
	}

	void OnReseting()
	{
		CallDeferred(MethodName.EmitSpawnParticles);
	}
	

	public void CreateDeathParticles(Player damager)
	{
		GpuParticles2D particles = GPUParticlesPool.GetClonedParticles(pilotDeathParticles);
		(particles.ProcessMaterial as ShaderMaterial).SetShaderParameter("initialDir", (main.Position - damager.Position).Normalized());
		//(particles.ProcessMaterial as ShaderMaterial).SetShaderParameter("rotation", Main.pilotSprite.Rotation);
		particles.Position = playerPosition;
		particles.Restart();
	}
	public void SetColor(int colorIdx)
	{
		(pilotDeathParticles.ProcessMaterial as ShaderMaterial).SetShaderParameter("outlineColor", teamColors[colorIdx]);
	}
	public void ExplodeShip()
	{
		GpuParticles2D newShipExplosionParticles = GPUParticlesPool.GetClonedParticles(shipExplosionParticles);
		newShipExplosionParticles.Position = playerPosition;
		newShipExplosionParticles.Restart();
	}
	public void SummonShieldBreakParticles(bool isPilot)
	{
		GpuParticles2D particlesEmitter;
		if (isPilot)
		{
			particlesEmitter = pilotShieldBreak;
		}
		else
		{
			particlesEmitter = shipShieldBreak;
		}
		
		//GpuParticles2D newParticlesEmitter = particlesEmitter.Duplicate() as GpuParticles2D;
		GpuParticles2D newParticlesEmitter = GPUParticlesPool.GetClonedParticles(particlesEmitter);
		(newParticlesEmitter.ProcessMaterial as ShaderMaterial).SetShaderParameter("Color", teamColors[Game.GetGamemodeLogic().teams[Game.GetGamemodeLogic().playerTeamByInputIdx[main.GetInputIdx()]].teamColorIdx]); //TODO: make this take team color instead
		newParticlesEmitter.Position = playerPosition;
		newParticlesEmitter.OneShot = true;
		newParticlesEmitter.Restart();
//		thisissofuckingweirdwhatdemonhaspocessedthisgameatleastitworksIguessbutatwhatcost(newParticlesEmitter);
	}
	async void thisissofuckingweirdwhatdemonhaspocessedthisgameatleastitworksIguessbutatwhatcost(GpuParticles2D newParticlesEmitter)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
	}
	public void PlayParticlesForTryGoShip()
	{
		//GD.Print("Playing Particles For Ship Reconstruction...");
		
		if (isTurningToShip || !main.isPilot) return;
		isTurningToShip = true;

		GpuParticles2D particles = GPUParticlesPool.GetClonedParticles(shipReconstructionParticles);
		currentShipReconstructionParticles = particles;

		particles.Position = Vector2.Zero;
		particles.Visible = true;
		ShaderMaterial particlesMaterial = particles.ProcessMaterial as ShaderMaterial;
		particlesMaterial.SetShaderParameter("emitterRotationOffset", GD.Randf());
		Action process;
		process = () => {
			particlesMaterial.SetShaderParameter("rotation", main.shipSprite.Rotation);
			particlesMaterial.SetShaderParameter("globalPosition", main.Position);
		};
		process();
		GetTree().ProcessFrame += process;

		Callable OnParticlesFinished = Callable.From(() =>
		{
			GetTree().ProcessFrame -= process;
			//GD.Print("rebuildFinished");
			isTurningToShip = false;
			RemoveParticles(particles);
			currentShipReconstructionParticles = null;
		});
		particles.Connect(GpuParticles2D.SignalName.Finished, OnParticlesFinished, (uint)ConnectFlags.OneShot);
		particles.Restart();
	}

	void RemoveParticles(GpuParticles2D particles)
	{
		if (particles == null) return;
		particles.Visible = false;
		particles.Emitting = false;
		GPUParticlesPool.Return(particles);
	}


	public void CancelTurningShip()
	{
//		if (!isTurningToShip) return;
		startAssemblingShipTimer.Stop();
		cooldownStartAssemblingShipTimer.Stop();
		isTurningToShip = false;
		//TODO maybe change this so it dissipates the particles instead?
		RemoveParticles(currentShipReconstructionParticles);
		currentShipReconstructionParticles = null;
	}
	void EmitSpawnParticles()
	{
		
		AnimatedSprite2D mainSprite = main.GetCurrentSprite();
		mainSprite.Hide();
		main.pilotShieldFlickerer.Play("shieldRegeneration");

		ShaderMaterial particlesMaterial = spawningParticles.ProcessMaterial as ShaderMaterial;
		
		AtlasTexture spriteAtlas = mainSprite.SpriteFrames.GetFrameTexture(mainSprite.Animation, mainSprite.Frame) as AtlasTexture;
		particlesMaterial.SetShaderParameter("mainTexture", spriteAtlas);
		particlesMaterial.SetShaderParameter("outlineColor", teamColors[main.colorIdx]);
		Vector2 textureSize = spriteAtlas.Atlas.GetSize();
		Vector2 atlasSize = spriteAtlas.Region.Size;
		Vector2 atlasPosition = spriteAtlas.Region.Position;

		Action process;
		process = () => {
			mainSprite.Hide();
			particlesMaterial.SetShaderParameter("rotation", mainSprite.Rotation);
			particlesMaterial.SetShaderParameter("sheetSize", textureSize/ atlasSize);
			particlesMaterial.SetShaderParameter("frame", (int)((atlasPosition.X/atlasSize.X) + ((atlasPosition.Y/atlasSize.Y) * (atlasSize.X/textureSize.X))));	
			particlesMaterial.SetShaderParameter("flipped", mainSprite.FlipH);
		};
		process();
		GpuParticles2D newSpawningParticles = GPUParticlesPool.GetClonedParticles(spawningParticles);
		GetTree().ProcessFrame += process;
/*
		GD.Print("----- playercolor ", main.colorIdx, " -----");
		GD.Print("textureSize", atlasSize);
		GD.Print("mainTexture", spriteAtlas);
		GD.Print("outlineColor", teamColors[main.colorIdx]);
		GD.Print("rotation", mainSprite.Rotation);
		GD.Print("sheetSize", textureSize / atlasSize);
		GD.Print("frame", (int)((atlasPosition.X / atlasSize.X) + ((atlasPosition.Y / atlasSize.Y) * (atlasSize.X / textureSize.X))));
		GD.Print("flipped", mainSprite.FlipH);
		GD.Print("particlesCount", (int) (atlasSize.X * atlasSize.Y));
*/	
		Callable onParticlesFinished;
		onParticlesFinished = Callable.From(() =>
		{
			newSpawningParticles.Hide();
			mainSprite.Show();
			GetTree().ProcessFrame -= process;
		});
		
		newSpawningParticles.Amount = (int) (atlasSize.X * atlasSize.Y); 
		newSpawningParticles.Connect(GpuParticles2D.SignalName.Finished, onParticlesFinished, (uint)ConnectFlags.OneShot);
		newSpawningParticles.GlobalPosition = main.GlobalPosition;
		newSpawningParticles.Restart();
	}
    public override void _ExitTree()
	{
		base._ExitTree();
		startAssemblingShipTimer.Timeout -= PlayParticlesForTryGoShip;
		cooldownStartAssemblingShipTimer.Timeout -= PlayParticlesForTryGoShip;
	}
}