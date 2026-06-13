using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class InGameMenu : CanvasLayer
{
	[Export] Label gamePausedBy;
	[Export] InGameMenuInput inputNode;
	[Export] VBoxContainer buttonsContainer;
	[Export] AnimationPlayer animationPlayer;
	[Export] StyleBoxFlat notHoveredStyleBox;
	[Export] StyleBoxFlat hoveredStyleBox;
	public int inputIdx = -2;
	public string keyboardKeyword = "";
	public bool isHidden = true;
	List<Button> currentlyAvaliableButtons = [];
	int currentlySelectedButtonIdx = 0;

	public override void _Ready()
	{
		SignalBus.Instance.GameStarted += ConnectSignals;
		SignalBus.Instance.GameExited += (MainMenuScreen exitedTo) => {DisconnectSignals();};

		foreach (Node possibleButton in buttonsContainer.GetChildren())
        {
            if (possibleButton is not Button) continue;

			currentlyAvaliableButtons.Add(possibleButton as Button);
        }
		currentlyAvaliableButtons[currentlySelectedButtonIdx].AddThemeStyleboxOverride("normal", hoveredStyleBox);
	}

	void Pause(Player playerPausedBy, bool triggeredByDisconnect = false)
	{
		inputIdx = playerPausedBy.GetInputIdx();
		if (playerPausedBy.GetInputIdx() == -1) keyboardKeyword = "Keyboard";
		else keyboardKeyword = "";
		
		GD.Print("GAME PAUSED");
		if (!triggeredByDisconnect) gamePausedBy.Text = "GAME PAUSED by " + playerPausedBy.Name;
		else gamePausedBy.Text = playerPausedBy.Name + "'s Controller Disconnected";
		buttonsContainer.Show();
		isHidden = false;
		Game.PauseGame();
	}

	void UnPause()
    {
        Game.UnPauseGame();
    }
	
	void OnResumePressed()
	{
		HideMenu();
		animationPlayer.Play("resumeGameCountdown");
	}

	void OnSettingsPressed()
	{
		
	}

	void OnBackToCharacterSelectScreenPressed()
	{
		HideMenu();
		Game.Instance.main.CallDeferred(Main.MethodName.BackToCharacterSelectScreen);
	}

	void OnQuitPressed()
	{
		GetTree().Quit();
	}

	void HideMenu()
	{
		buttonsContainer.Hide();
		isHidden = true;
	}

	void ConnectSignals()
	{
		inputNode.InGameMenuWASD += OnMenuWASD;
		inputNode.InGameMenuInteract += OnMenuInteract;
		inputNode.InGameMenuBack += OnMenuBack;
		Input.JoyConnectionChanged += OnJoyConnectionChanged;	
		SignalBus.Instance.PauseRequest += Pause;
		SignalBus.Instance.NewRoundStart += HideMenu;
	}
	void DisconnectSignals()
	{
		inputNode.InGameMenuWASD -= OnMenuWASD;
		inputNode.InGameMenuInteract -= OnMenuInteract;
		inputNode.InGameMenuBack -= OnMenuBack;
		Input.JoyConnectionChanged -= OnJoyConnectionChanged;	
		SignalBus.Instance.PauseRequest -= Pause;
		SignalBus.Instance.NewRoundStart -= HideMenu;
	}

    private void OnJoyConnectionChanged(long device, bool connected)
    {
		GD.Print("device connection changed: " +  device);
        if (!connected)
		{
			Pause(Game.Instance.GetPlayerFromInputIdx((int)device), true);
		}
		else if (GetTree().Paused && device == inputIdx)
		{
			OnResumePressed();
		}
    }

    private void OnMenuWASD(float x, float y)
    {
        if (y == 0) return;
		Button oldSelectedButton = currentlyAvaliableButtons[currentlySelectedButtonIdx];
		oldSelectedButton.AddThemeStyleboxOverride("normal", notHoveredStyleBox);

		if (y > 0)
			currentlySelectedButtonIdx ++;
		else
			currentlySelectedButtonIdx --;

		if (currentlySelectedButtonIdx < 0)
        {
            currentlySelectedButtonIdx = currentlyAvaliableButtons.Count - 1;
        }
		else if (currentlySelectedButtonIdx > currentlyAvaliableButtons.Count - 1)
        {
            currentlySelectedButtonIdx = 0;
        }

		Button newSelectedButton = currentlyAvaliableButtons[currentlySelectedButtonIdx];
		newSelectedButton.AddThemeStyleboxOverride("normal", hoveredStyleBox);
    }

    private void OnMenuBack()
    {
        OnResumePressed();
    }

    private void OnMenuInteract()
    {
        currentlyAvaliableButtons[currentlySelectedButtonIdx].EmitSignal(Button.SignalName.Pressed);
    }
}

