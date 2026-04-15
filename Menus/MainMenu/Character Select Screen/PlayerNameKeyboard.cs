using Godot;
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class PlayerNameKeyboard : VBoxContainer
{
    [ExportGroup("Nodes")]
    [Export] PlayerMenuInput menuInput;
    [Export] Label nameDisplay;
    public Color PlayerColor
    {
        get
        {
            return playerColor;
        }
        set
        {
            playerColor = value;
            CurrentButton.SelfModulate = PlayerColor;
        }
    }
    private Color playerColor = new Color(1, 0, 1);
    string CurrentName
    {
        get
        {
            return currentName;
        }
        set
        {
            currentName = value;
            nameDisplay.Text = CurrentName;
        }
    }
    string currentName = "";
    int maxNameLength = 7;
    TextureRect CurrentButton
    {
        get
        {
            return currentButton;
        }
        set
        {
            if (currentButton != null) currentButton.SelfModulate = new Color(1,1,1);
            currentButton = value;
            currentButton.SelfModulate = PlayerColor;
        }
    }
    TextureRect currentButton = null;
    Vector2 keyboardPos = Vector2.One;

    public override void _Ready()
    {
        SetupKeys();
        menuInput.MenuWASD += OnMenuWASD;
        menuInput.AcceptStart += OnMenuAccept;
        menuInput.BackStart += OnMenuBack;
        CurrentButton = GetButton(keyboardPos);
        CurrentButton.SelfModulate = PlayerColor;
    }

    void OnMenuWASD(int XMenuInput, int YMenuInput)
    {
        CurrentButton.SelfModulate = new Color(1, 1, 1);
       
        Vector2 newKeyboardPos = keyboardPos + new Vector2(XMenuInput, YMenuInput);
        
        if (newKeyboardPos.Y > GetChildCount()) newKeyboardPos.Y = 1;
        else if (newKeyboardPos.Y == 0) newKeyboardPos.Y = GetChildCount();
        HBoxContainer line = GetChild<HBoxContainer>((int)newKeyboardPos.Y - 1);
        if (newKeyboardPos.X > 4) newKeyboardPos.X = 1;
        else if (newKeyboardPos.X > line.GetChildCount())
        {
            if (YMenuInput < 0)
                newKeyboardPos.Y -= 1;
            else if (YMenuInput > 0)
                newKeyboardPos.Y = 1;
            else
                newKeyboardPos.X = 1;
        }
        else if (newKeyboardPos.X == 0) newKeyboardPos.X = line.GetChildCount();

        TextureRect newButton = GetButton(newKeyboardPos);
        newButton.SelfModulate = PlayerColor;
        CurrentButton = newButton;
        keyboardPos = newKeyboardPos;
    }

    private void OnMenuAccept()
    {
        if (!Visible) return;
        if (CurrentName.Length >= maxNameLength) return;
        int letterNumber = (int)(keyboardPos.Y - 1)*4 + (int)keyboardPos.X;
        CurrentName += GetLetterFromInt(letterNumber);
    }

    private void OnMenuBack()
    {
        if (!Visible) return;
        if (CurrentName.Length <= 0) return;
        CurrentName = CurrentName.Remove(CurrentName.Length-1);
    }

    TextureRect GetButton(Vector2 pos)
    {
        TextureRect Button = GetChild((int)pos.Y - 1).GetChild<TextureRect>((int)pos.X - 1);
        return Button;
    }

    void SetupKeys()
    {
        int letterIdx = 0;
        LabelSettings labelSettings = new();
        labelSettings.FontSize = 44;
        foreach (Node Container in GetChildren())
        {
            if (Container is not HBoxContainer) continue;
            foreach (Node Button in Container.GetChildren())
            {
                if (Button is not TextureRect buttonRect) continue;
                letterIdx++;
                buttonRect.CustomMinimumSize = Vector2.One * 64;
                Label newKeyLable = new();
                newKeyLable.CustomMinimumSize = buttonRect.CustomMinimumSize;
                newKeyLable.LabelSettings = labelSettings;
                newKeyLable.Text = GetLetterFromInt(letterIdx);
                newKeyLable.HorizontalAlignment = HorizontalAlignment.Center;
                newKeyLable.VerticalAlignment = VerticalAlignment.Center;
                buttonRect.AddChild(newKeyLable);
            } 
        }
    }

    string GetLetterFromInt(int LetterIdx)
    {
        string test = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (LetterIdx < 0 || LetterIdx > test.Length) return "";
        return "" + test[LetterIdx - 1];
    }
    
    public void Open()
    {
        keyboardPos = Vector2.One;
        CurrentButton = GetButton(keyboardPos);
        Visible = true;
    }
    public void Close()
    {
        keyboardPos = Vector2.One;
        CurrentButton = GetButton(keyboardPos);
        Visible = false;
    }
}
