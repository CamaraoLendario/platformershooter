using Godot;
using static SpaceMages.SpaceMagesVars;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

[Tool]
public partial class MainMenuMainScreen : MainMenuScreen
{
	Control themeCard;
	bool isAtBanner = true;
	TextureRect banner;
	Label pressAnyKey;
    public override void _Ready()
	{
		base._Ready();
		themeCard = GetNode<Control>("themeCard");
		banner = themeCard.GetNode<TextureRect>("banner");
		pressAnyKey = themeCard.GetNode<Label>("pressAnyKey");
		pressAnyKey.Visible = isAtBanner;
		Vector2 screenRez = GetScreenRez();
		for (int i = 0; i < menuOptions.Length; i++)
		{
			MenuItem node = menuOptions[i];
			
			node.SetDeferred(Control.PropertyName.Position, node.originalPosition + (Vector2.Down * screenRez));
		}

		Left += () =>
		{
			MoveBanner(true);
		};
		Entered += () =>
		{
			MoveBanner(false);
		};
	}
    public override bool OnInteract()
    {
		if (LeaveThemeCard()) return true;
		switch (menuSelectPanel.GetCurrentNodeIdx())
		{
			case 0:
				Play();
				break;
			case 1:
				Settings();
				break;
			case 2:
				Credits();
				break;
			case 3:
				Quit();
				break;
				
		}
		return false;
    }
	void Play()
	{
		Move(Vector2.Left);
		GamemodeSelectScreen gamemodeSelectScreen = GetParent().GetNode<GamemodeSelectScreen>("GamemodeSelectScreen");
		gamemodeSelectScreen.Move(Vector2.Right, true);
		GetMenuController().currentScreen = gamemodeSelectScreen;
	}
	void Settings()
	{
		Move(Vector2.Right);
		SettingsScreen settingsScreen = GetParent().GetNode<SettingsScreen>("SettingsScreen");
		settingsScreen.Move(Vector2.Left, true);
		GetMenuController().currentScreen = settingsScreen;
	}
	void Credits()
	{
		
	}
	void Quit()
	{
		if(Engine.IsEditorHint()) return;
		GetTree().Quit();
	}
    public override void Back()
	{
		LeaveThemeCard(true);
	}
	public bool LeaveThemeCard(bool reverse = false)
	{
		if (isAtBanner == reverse) return false;

		Vector2 ScreenSize = GetScreenRez();
		float initialScale = banner.Scale.X;
		float endScale = 0.5f;
		float reverseFloat = 1f;
		if (reverse){
			endScale = 1f;
			reverseFloat = 0f;
		}
		float scaleDiff = endScale - initialScale;
		pressAnyKey.Visible = reverse;

		Tween tween = CreateTween();
		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			float tempTweenedValue = (Mathf.Sin((tweenedValue - .5f) * 2 * (Mathf.Pi/2)) + 1)/2;
			banner.Scale = (initialScale + (scaleDiff * tempTweenedValue)) * Vector2.One;

			for (int i = 0; i < menuOptions.Length; i++)
			{
				MenuItem node;
				if (!reverse)
					node = menuOptions[menuOptions.Length - i - 1];
				else node = menuOptions[i];
				float optionsTweenedValue = Mathf.Clamp((tweenedValue * 1.5f) - ((1-((i + 1)/((float)menuOptions.Length)))*0.5f), 0 ,1);
				optionsTweenedValue = (Mathf.Sin((optionsTweenedValue-.5f) * 2 * (Mathf.Pi/2)) + 1)/2;
				optionsTweenedValue = Mathf.Abs(reverseFloat - optionsTweenedValue);

				node.Position = node.originalPosition + (Vector2.Down * optionsTweenedValue * ScreenSize);
			}
		}), 0f, 1f, .6f);
		isAtBanner = reverse;
		return true;
	}
	void MoveBanner(bool up = true)
	{
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.InOut);

		TextureRect banner = themeCard.GetNode<TextureRect>("banner");
		float value = 0;
		if (up)
			value = 1;

		Vector2 initialBannerPos = new Vector2(-banner.Size.X/2, 0 - banner.Size.Y * value);
		Vector2 finalBannerPos = new Vector2(-banner.Size.X/2, 0 - (banner.Size.Y * (1 - value)));;
		Vector2 differencePos = finalBannerPos - initialBannerPos;
		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			banner.Position = initialBannerPos + (differencePos*tweenedValue);
		}), 0f, 1f, .6);
	}
}
