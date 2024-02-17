using System;
using Game.Data;
using Game.Players;
using Game.UI;
using Godot;

public class Settings {

    public static Settings CurrentSettings = new();

    static Settings() {
        var data = (Godot.Collections.Array) dataSaver.LoadValue();
        
        if (data.ToString() == "[]") return;

        CurrentSettings = new() {
            languageSelected = (int) data[0]
        };
    }

    public Godot.Collections.Array Serialize() {
        Godot.Collections.Array array = new() {
            languageSelected,
        };
        return array;
    }

    public static readonly string[] languages = new string[] {
        "en", "fr"
    };

    public int languageSelected = 0;

    static readonly DataSaver dataSaver = new(() => new("Settings", CurrentSettings.Serialize()));
}

public partial class SettingsPage : Control, IMenu {
    [Export]
    private Button closeButton;
    [Export]
    private OptionButton languageChoice;

    public Action Disable {get; set;}
    public override void _Ready() {
        closeButton.Pressed += () => Disable?.Invoke();

        languageChoice.ItemSelected += OnLanguageItemSelected; 
        languageChoice.Select(Settings.CurrentSettings.languageSelected);
    }

    private void OnLanguageItemSelected(long longIndex) {
        int index = (int) longIndex;
        if (index < 0) return;

        Settings.CurrentSettings.languageSelected = index;
        TranslationServer.SetLocale(Settings.languages[index]);
    }
    
    public void Enable(Player player) {
        Visible = true;
    }

    public void Close() {
        Visible = false;
    }
}