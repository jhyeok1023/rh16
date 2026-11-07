using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>Drives Settings.uxml. Shared by the title scene and the pause menu.</summary>
public sealed class SettingsMenu
{
    private const string TickOnClass = "tick--on";

    private readonly UiMenu _menu;
    private readonly List<VisualElement> _bgmTicks;
    private readonly List<VisualElement> _seTicks;
    private readonly Label _bgmValue;
    private readonly Label _seValue;
    private readonly Label _screenValue;
    private readonly Label _resolutionValue;
    private readonly Label _vSyncValue;

    public event Action Closed;

    public SettingsMenu(VisualElement root)
    {
        _menu = new UiMenu(root, root.Q("settings-cursor"));
        _bgmTicks = root.Q("bgm-ticks").Query(className: "tick").ToList();
        _seTicks = root.Q("se-ticks").Query(className: "tick").ToList();
        _bgmValue = root.Q<Label>("bgm-value");
        _seValue = root.Q<Label>("se-value");
        _screenValue = root.Q<Label>("screen-value");
        _resolutionValue = root.Q<Label>("resolution-value");
        _vSyncValue = root.Q<Label>("vsync-value");

        _menu.Submitted += OnSubmitted;
        _menu.Adjusted += OnAdjusted;
        _menu.Cancelled += Close;
    }

    public void Activate()
    {
        _menu.Activate();
        Refresh();
    }

    public void Tick()
    {
        _menu.Tick();
    }

    private void OnSubmitted(string action)
    {
        switch (action)
        {
            case "reset":
                GameSettings.ResetToDefaults();
                Refresh();
                break;
            case "back":
                Close();
                break;
        }
    }

    private void OnAdjusted(int delta)
    {
        switch (_menu.CurrentName)
        {
            case "bgm":
                GameSettings.Bgm += delta;
                break;
            case "se":
                GameSettings.Se += delta;
                break;
            case "screen":
                GameSettings.ScreenIndex += delta;
                break;
            case "resolution":
                GameSettings.ResolutionIndex += delta;
                break;
            case "vsync":
                GameSettings.VSync += delta;
                break;
            default:
                return;
        }

        GameSettings.Apply();
        Refresh();
    }

    private void Close()
    {
        Closed?.Invoke();
    }

    private void Refresh()
    {
        PaintTicks(_bgmTicks, GameSettings.Bgm);
        PaintTicks(_seTicks, GameSettings.Se);
        _bgmValue.text = GameSettings.Bgm.ToString();
        _seValue.text = GameSettings.Se.ToString();
        _screenValue.text = GameSettings.ScreenLabels[GameSettings.ScreenIndex];
        _resolutionValue.text = GameSettings.ResolutionLabels[GameSettings.ResolutionIndex];
        _vSyncValue.text = GameSettings.SwitchLabels[GameSettings.VSync];
    }

    private static void PaintTicks(List<VisualElement> ticks, int value)
    {
        for (int i = 0; i < ticks.Count; i++)
        {
            ticks[i].EnableInClassList(TickOnClass, i < value);
        }
    }
}
