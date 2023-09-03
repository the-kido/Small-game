using System;
using System.Collections.Generic;
using Game.Players;
using Game.UI;
using Godot;

public class Settings {
    string language;
}

public partial class SettingsPage : Control, IMenu {
    [Export]
    private Button closeButton;
    [Export]
    private OptionButton languageChoice;

    public event Action Disable;

    public override void _Ready() {
        closeButton.Pressed += () => Disable?.Invoke();

        languageChoice.ItemSelected += OnLanguageItemSelected; 
    }
    static readonly string[] languages = new string[] {
        "en", "fr"
    };

    private void OnLanguageItemSelected(long index) {
        
        if (index < 0) return;
        
        TranslationServer.SetLocale(languages[index]);
    }
    
    public void Enable(Player player) {
        Visible = true;
    }

    public void Close() {
        Visible = false;
    }
}