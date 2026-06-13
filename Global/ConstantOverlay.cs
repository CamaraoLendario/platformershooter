using Godot;
using System;

public partial class ConstantOverlay : CanvasLayer
{
	Panel fadePanel;
	Tween tween;
    public override void _Ready()
	{
		fadePanel = GetNode<Panel>("%FadePanel");
	}

	public Tween FadeOut()
	{
		tween = CreateTween();

		tween.TweenProperty(fadePanel, "modulate:a", 1.0f, 0.15f);

		return tween;
	}

	public Tween FadeIn()
	{
		tween = CreateTween();

		tween.TweenProperty(fadePanel, "modulate:a", 0.0f, 0.15f);

		return tween;
	}
}
