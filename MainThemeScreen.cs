using Godot;
using System;

[Tool]
public partial class MainThemeScreen : MainMenuScreen
{
	[Export] public TextureRect banner;

    public override void _Ready()
    {
        base._Ready();

		MainMenuMainScreen mainMenuMain = GetParent().GetNode<MainMenuMainScreen>("MainMenuMainScreen");
		mainMenuMain.Entered += () => {
			if (GetMenuController().currentScreen != this)
				Movebanner(false);
		};
		mainMenuMain.Left += () => {
			if (GetMenuController().currentScreen != this)
				Movebanner(true);
		};
    }

	public override bool OnPositiveAction()
    {
		Move(Vector2.Down);
		ScaleBanner();
		MainMenuMainScreen mainMenuMain = GetParent().GetNode<MainMenuMainScreen>("MainMenuMainScreen");
		mainMenuMain.Move(Vector2.Down, true);
		GetNode<MainMenuController>("%MainMenuController").currentScreen = mainMenuMain;

		return true;
    }

    public override void Move(Vector2 dir, bool reverse = false, bool skipAnimation = false)
    {
		ScaleBanner(!reverse);

        base.Move(dir, reverse, skipAnimation);
    }

	void ScaleBanner(bool up = true)
	{
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.InOut);

		float initialScale = 0.5f;
		float finalScale = 1.0f;

		if (up)
		{
			initialScale = 1.0f;
			finalScale = 0.5f;
		}

		float differenceScale = finalScale - initialScale;

		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			banner.Scale = (initialScale + (differenceScale * tweenedValue))  * Vector2.One;
		}), 0f, 1f, .6);
	}

	void Movebanner(bool up = true)
	{
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.InOut);

		MainThemeScreen mainThemeScreen = GetParent().GetNode<MainThemeScreen>("MainThemeScreen");
		TextureRect banner = mainThemeScreen.banner;
		float value = 0;
		if (up)	
		{
			value = 1;
		}

		Vector2 initialBannerPos = new Vector2(0, 0 - banner.Size.Y * value);
		Vector2 finalBannerPos = new Vector2(0, 0 - (banner.Size.Y * (1 - value)));;
		Vector2 differencePos = finalBannerPos - initialBannerPos;
		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			banner.Position = initialBannerPos + (differencePos*tweenedValue);
		}), 0f, 1f, .6);
	}
/* 

	| <-- lil nek
	O <-- big chingus

*/
}
