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
		ConnectSignals();
		Game.Instance.NewRoundStarted += HideMenu;

		foreach (Node possibleButton in buttonsContainer.GetChildren())
        {
            if (possibleButton is not Button) continue;

			currentlyAvaliableButtons.Add(possibleButton as Button);
        }
		GD.Print("ingamemenu is now ready");
		currentlyAvaliableButtons[currentlySelectedButtonIdx].AddThemeStyleboxOverride("normal", hoveredStyleBox);
	}

	void Pause(Player playerPausedBy, bool triggeredByDisconnect = false)
	{
		inputIdx = playerPausedBy.inputIdx;
		if (playerPausedBy.isKeyboardControlled) keyboardKeyword = "Keyboard";
		else keyboardKeyword = "";
		
		GD.Print("GAME PAUSED");
		if (!triggeredByDisconnect) gamePausedBy.Text = "GAME PAUSED by " + playerPausedBy.Name;
		else gamePausedBy.Text = playerPausedBy.Name + "'s Controller Disconnected";
		buttonsContainer.Show();
		isHidden = false;
		Game.Instance.PauseGame();
	}

	void UnPause()
    {
        Game.Instance.UnPauseGame();
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
		UnPause();
		QueueFree();
		Game.Instance.CallDeferred(Game.MethodName.BackToMapSelector);
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

	private void ConnectSignals()
	{
		inputNode.InGameMenuWASD += OnMenuWASD;
		inputNode.InGameMenuAccept += OnMenuAccept;
		inputNode.InGameMenuBack += OnMenuBack;
		Input.JoyConnectionChanged += OnJoyConnectionChanged;	
		Game.Instance.PlayerAdded += OnPlayerAdded;
	}

    private void OnJoyConnectionChanged(long device, bool connected)
    {
		GD.Print("device connection changed: " +  device);
		for (int i = 0; i < Game.Instance.playerNodesByInputIdx.Count; i++)
		{
			int key = Game.Instance.playerNodesByInputIdx.Keys.ToList()[i];
			GD.Print(key, Game.Instance.playerNodesByInputIdx[key].Name);
		}
        if (!connected)
		{
			Pause(Game.Instance.playerNodesByInputIdx[(int)device], true);
		}
		else if (GetTree().Paused && device == inputIdx)
		{
			OnResumePressed();
		}
    }

    void OnPlayerAdded(Player player)
	{
		player.playerInput.PauseRequest += Pause;
		GD.Print("connected pause to player " + player);
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

		GD.Print(currentlySelectedButtonIdx);
		Button newSelectedButton = currentlyAvaliableButtons[currentlySelectedButtonIdx];
		newSelectedButton.AddThemeStyleboxOverride("normal", hoveredStyleBox);
    }

    private void OnMenuBack()
    {
        OnResumePressed();
    }

    private void OnMenuAccept()
    {
        currentlyAvaliableButtons[currentlySelectedButtonIdx].EmitSignal(Button.SignalName.Pressed);
    }

    public override void _ExitTree()
    {
        inputNode.InGameMenuWASD -= OnMenuWASD;
		inputNode.InGameMenuAccept -= OnMenuAccept;
		inputNode.InGameMenuBack -= OnMenuBack;
		Game.Instance.PlayerAdded -= OnPlayerAdded;
		Game.Instance.NewRoundStarted -= HideMenu;
		foreach(Player player in Game.Instance.playerNodesByColor.Values)
		{
			player.playerInput.PauseRequest -= Pause;
			GD.Print("connected pause to player " + player);
		};
		base._ExitTree();
    }

}

