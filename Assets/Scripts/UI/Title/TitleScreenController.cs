using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>Moves between the title menu, difficulty select and options in Title.uxml.</summary>
[RequireComponent(typeof(UIDocument))]
public sealed class TitleScreenController : MonoBehaviour
{
    [SerializeField] private string playScene = "PlayScene";

    private VisualElement _titleScreen;
    private VisualElement _difficultyScreen;
    private VisualElement _settingsScreen;
    private UiMenu _titleMenu;
    private UiMenu _difficultyMenu;
    private SettingsMenu _settings;
    private UiSpinners _spinners;
    private System.Action _tick;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _titleScreen = root.Q("title-screen");
        _difficultyScreen = root.Q("difficulty-screen");
        _settingsScreen = root.Q("settings-screen");
        _spinners = new UiSpinners(root);

        _titleMenu = new UiMenu(_titleScreen, root.Q("title-cursor"));
        _titleMenu.Submitted += OnTitleSubmitted;

        _difficultyMenu = new UiMenu(_difficultyScreen, root.Q("difficulty-cursor"));
        _difficultyMenu.Submitted += _ => SceneManager.LoadScene(playScene);
        _difficultyMenu.Cancelled += ShowTitle;

        _settings = new SettingsMenu(_settingsScreen);
        _settings.Closed += ShowTitle;

        GameSettings.Apply();
        ShowTitle();
    }

    private void Update()
    {
        _spinners.Tick();
        _tick?.Invoke();
    }

    private void OnTitleSubmitted(string action)
    {
        switch (action)
        {
            case "start":
                ShowDifficulty();
                break;
            case "settings":
                ShowSettings();
                break;
            case "quit":
                Quit();
                break;
        }
    }

    private void ShowTitle()
    {
        Show(_titleScreen);
        _titleMenu.Activate();
        _tick = _titleMenu.Tick;
    }

    private void ShowDifficulty()
    {
        Show(_difficultyScreen);
        _difficultyMenu.Activate();
        _tick = _difficultyMenu.Tick;
    }

    private void ShowSettings()
    {
        Show(_settingsScreen);
        _settings.Activate();
        _tick = _settings.Tick;
    }

    private void Show(VisualElement screen)
    {
        SetVisible(_titleScreen, screen == _titleScreen);
        SetVisible(_difficultyScreen, screen == _difficultyScreen);
        SetVisible(_settingsScreen, screen == _settingsScreen);
    }

    private static void SetVisible(VisualElement element, bool visible)
    {
        element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
