using Godot;
using System;
using System.Threading.Tasks;

[Tool]
public partial class MenuSelectorHelper : Control
{
	MenuSelectPanel menuSelectPanel;
	public (Vector2 position, Vector2 size) openSettings = (Vector2.Zero, Vector2.Zero);
	public (Vector2 position, Vector2 size) closedSettings = (Vector2.Zero, Vector2.Zero);

    public override void _Ready()
    {
		Control openSettingsNode = GetNodeOrNull<Control>("%openSettings");
		Control closedSettingsNode = GetNodeOrNull<Control>("%closedSettings");

		if (openSettingsNode == null || closedSettingsNode == null)
		{
			GD.Print(this.Owner.Name, ": open and closed settings not found");
			openSettings = (Position, Size);
			closedSettings = openSettings;
			return;
		}

        openSettings = (openSettingsNode.Position, openSettingsNode.Size);
		if(!Engine.IsEditorHint()) openSettingsNode.Free();
		closedSettings = (closedSettingsNode.Position, closedSettingsNode.Size);
		if(!Engine.IsEditorHint()) closedSettingsNode.Free();

		CallDeferred(MethodName.SetPosAndSizeClosed);
    }

	public void SetPosAndSize(Vector2 position, Vector2 size)
    {
		if (HasPanel())
		{
			menuSelectPanel.Reparent(GetParent());
			CallDeferred(MethodName.SetHelper, position, size, menuSelectPanel);
		}
		Position = position;
		Size = size;
    }
	async void SetHelper(Vector2 position, Vector2 size, MenuSelectPanel menuSelectPanel)
	{
		menuSelectPanel.ForcePosAndSize(position, size);
		await ToSignal(menuSelectPanel.tween, Tween.SignalName.Finished);
		//menuSelectPanel.Reparent(this);
	}

	public bool HasPanel()
	{
		if (menuSelectPanel == null)
			menuSelectPanel = GetChildOrNull<MenuSelectPanel>(0);
		return menuSelectPanel != null;
	}

	public void SetPosAndSizeOpen()
    {
		SetPosAndSize(openSettings.position, openSettings.size);
    }
	public void SetPosAndSizeClosed()
    {
		SetPosAndSize(closedSettings.position, closedSettings.size);
    }
	//TODO idk if this is necessary. if it is I'll eventually find out
/* 	public void ForceSetPosAndSizeOpen(MenuSelectPanel panel)
    {
		panel.ForcePosAndSize(openSettings.position, openSettings.size);
    }
	public void ForceSetPosAndSizeClosed(MenuSelectPanel panel)
    {
		panel.ForcePosAndSize(closedSettings.position, closedSettings.size);
    } */
}
