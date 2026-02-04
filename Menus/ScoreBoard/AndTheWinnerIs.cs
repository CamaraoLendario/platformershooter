using Godot;
using System;
using System.Collections.Generic;

public partial class AndTheWinnerIs : Control
{
	[Export] AnimationPlayer announcingAnimation;
	[Export] AudioStreamPlayer colorAnnouncer;

	List<string> announcementColors = [
		"uid://dxche3ctkq2gg",	// Red
		"uid://7h7xfwryvpd8", 	// Purple
		"uid://c2xntvlpsy3tb",	// Blue
		"uid://chr6xtlx00rv1",	// Green
		"uid://dfntm7v0yb8ax",	// Yellow
		"uid://dneth5f1yjliw",	// Orange
	];  

	public void AnnounceWinner(int colorIdx)
	{
		(GetNode<TextureRect>("winnerSprite").Material as ShaderMaterial).SetShaderParameter("Color", SpaceMages.SpaceMagesVars.teamColors[colorIdx]);
		colorAnnouncer.Stream = GD.Load<AudioStreamOggVorbis>(announcementColors[colorIdx]);
		announcingAnimation.Play("Base");
	}
}
